using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private NetworkObject _coinPrefab;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerCoinTracker _coinTracker;
    [SerializeField] private LayerMask _collectiblesLayerMask;
    [SerializeField] private float _collectibleCheckRadius = 0.5f;
    [SerializeField] private DeathScreenUI _deathScreenPrefab;
    private int _rocketCount = 0;
    private Vector3 _mapCenterPosition;
    private DeathScreenUI _deathScreenInstance;
    private CanvasGroup _deathScreenGroup;
    private NetworkVariable<int> _healthPoints = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<FixedString128Bytes> _playerName = new NetworkVariable<FixedString128Bytes>("1", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Animator _animator;
    private bool _isDead = false;

    public const int MaxHealth = 3;
    public FixedString128Bytes PlayerName { get => _playerName.Value; set => _playerName.Value = value; }

    public int HealthPoints => _healthPoints.Value;
    public static Dictionary<ulong, Player> Players { get; } = new Dictionary<ulong, Player>();

    public event System.Action<string> PlayerNameChanged;
    public event System.Action<Player> PlayerSpawned;

    private void OnValidate()
    {
        _characterController = GetComponent<CharacterController>();
        _coinTracker = GetComponent<PlayerCoinTracker>();
    }
    public override void OnNetworkSpawn()
    {
        PositionOnGround();

        if (IsOwner)
        {
            PlayerCameraFollow cameraFollow = GameObject.FindObjectOfType<PlayerCameraFollow>();
            cameraFollow.SetTarget(transform);
            _deathScreenInstance = Instantiate(_deathScreenPrefab);
        }

        base.OnNetworkSpawn();
        Players.Add(this.OwnerClientId, this);
        _playerName.OnValueChanged += OnPlayerNameChanged;
        _healthPoints.OnValueChanged += OnHealthChanged;
        PlayerSpawned.Invoke(this);
    }

    public override void OnNetworkDespawn()
    {
        _playerName.OnValueChanged -= OnPlayerNameChanged;
        _healthPoints.OnValueChanged -= OnHealthChanged;
    }

    private void OnPlayerNameChanged(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        PlayerNameChanged?.Invoke(newValue.ConvertToString());
    }

    private void PositionOnGround()
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        CharacterController characterController = _characterController;
        Vector3 rayStart = transform.position + Vector3.up * 10f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, 20f, groundMask))
        {
            float capsuleCenterOffsetY = characterController.center.y;
            float capsuleHalfHeight = characterController.height / 2f;
            float totalOffsetY = capsuleHalfHeight - capsuleCenterOffsetY;
            float newY = hit.point.y + totalOffsetY;

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            Debug.Log($"PositionOnGround Player positioned to Y = {newY}, Hit = {hit.point.y}, CapsuleCollider = {characterController.height}");
        }
    }

    public void AddRockets(int amount)
    {
        _rocketCount += amount;
        Debug.Log($"[PLAYER] Получено ракет: {amount}, всего: {_rocketCount}");
    }

    public bool HasRockets()
    {
        return _rocketCount > 0;
    }

    public void UseRocket()
    {
        _rocketCount--;
        Debug.Log($"[PLAYER] Ракета использована. Осталось: {_rocketCount}");
    }
    private void CheckCollectibles()
    {
        if (_isDead)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, _collectibleCheckRadius, _collectiblesLayerMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Collectable>(out var collectable))
            {
                collectable.OnCollect(this);
            }
        }
    }

    public void TakeDamage()
    {
        if (_healthPoints.Value > 0)
        {
            _healthPoints.Value--;
        }
    }

    public static Player FindPlayerByName(string name, System.StringComparison comparison)
    {
        foreach (Player player in Players.Values)
        {
            if (player.PlayerName.ToString().Equals(name, comparison))
            {
                return player;
            }
        }

        return null;
    }

    private void OnHealthChanged(int previous, int current)
    {
        Debug.Log($"Player is dead.. {current}, {IsServer}, {_isDead}");

        if (_isDead)
        {
            return;
        }

        if (current <= 0)
        {
            _isDead = true;

            if (IsOwner)
            {
                ShowDeathScreen();
            }

            if (IsServer)
            {
                Debug.Log($"{OwnerClientId} dropping coins on death.. ");
                _coinTracker.DropCoinsOnDeath(transform.position);

                Debug.Log($"{OwnerClientId} starting respawn coroutine");
                StartCoroutine(RespawnAfterDelay());
            }
        }

    }
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        _characterController.enabled = false;
        transform.position = _mapCenterPosition;
        _characterController.enabled = true;

        _healthPoints.Value = 3;
        _isDead = false;

        if (IsOwner)
        {
            HideDeathScreen();
        }
    }

    private void ShowDeathScreen()
    {
        _deathScreenInstance.Show();
    }

    private void HideDeathScreen()
    {
        _deathScreenInstance.Hide();
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        if (IsServer)
        {
            CheckCollectibles();
        }

        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            NetworkObject coin = Instantiate(_coinPrefab);
            coin.transform.position = transform.position + transform.forward * 3;
            coin.Spawn(true);
            // _healthPoints.Value--;
            // SayHelloServerRpc("Test string");
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0f, vertical);
        // transform.position += move * _moveSpeed * Time.deltaTime;
        _characterController.Move(move * _moveSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        SetSpeedServerRpc(move.magnitude);
    }

    [ServerRpc]
    private void SetSpeedServerRpc(float speed)
    {
        // _animator.SetFloat("Speed", speed);
        SetSpeedClientRpc(speed);
    }

    [ClientRpc]
    private void SetSpeedClientRpc(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }
}
