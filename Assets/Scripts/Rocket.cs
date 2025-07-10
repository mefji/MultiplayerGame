using Unity.Netcode;
using UnityEngine;

public class Rocket : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private LayerMask _layerMask;

    public ulong ownerId;
    private Player _target;
    private Vector3 _defaultDirection;

    private void Start()
    {
        Invoke(nameof(DestroyRocket), _lifeTime);
        _defaultDirection = transform.forward;
        FindTarget();
    }

    private void Update()
    {
        Vector3 moveDirection;

        if (_target == null)
        {
            moveDirection = _defaultDirection;
            transform.position += moveDirection * _speed * Time.deltaTime;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);

        if (distanceToTarget < 0.5f)
        {
            ApplyDamageServerRpc(_target.OwnerClientId);
            DestroyRocket();
            return;
        }

        moveDirection = (_target.transform.position - transform.position).normalized;

        transform.position += moveDirection * _speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(moveDirection);

        Vector3 capsuleDirection = transform.forward;

        float capsuleRadius = _collider.radius;
        float capsuleHalfHeight = _collider.height / 2f;
        float capsuleLength = capsuleHalfHeight;

        Vector3 start = transform.position - capsuleDirection * capsuleLength;
        Vector3 end = transform.position + capsuleDirection * capsuleLength;
        

        Collider[] hits = Physics.OverlapCapsule(start, end, capsuleRadius, _layerMask, QueryTriggerInteraction.Ignore);

        foreach (var hit in hits)
        {
            if (GetComponent<Player>() == null)
            {
                DestroyRocket();
                return;
            }
        }
    }

    private void FindTarget()
    {
        float minDistance = float.MaxValue;

        foreach (var player in Player.Players.Values)
        {
            if (player.OwnerClientId == ownerId) continue;

            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < minDistance && distance <= _detectionRadius)
            {
                minDistance = distance;
                _target = player;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyDamageServerRpc(ulong targetId)
    {
        if (Player.Players.TryGetValue(targetId, out Player targetPlayer))
        {
            targetPlayer.TakeDamage();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<NetworkObject>().Spawn(true);

            if (IsSpawned)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    private void DestroyRocket()
    {
        if (IsServer)
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<NetworkObject>().Spawn(true);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
