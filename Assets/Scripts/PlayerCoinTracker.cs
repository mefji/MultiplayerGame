using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Runtime.CompilerServices;

public class PlayerCoinTracker : NetworkBehaviour
{
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private GameObject _coinPrefab;
    private ulong _clientId;

    public NetworkVariable<int> CoinCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        var player = GetComponent<Player>();
        player.PlayerSpawned += OnPlayerSpawned;
    }

    public void DropCoinsOnDeath(Vector3 dropPosition)
    {
        Debug.Log($"Dropping coins at position {dropPosition}");

        var coins = CoinCount.Value;

        if (coins <= 0)
        {
            Debug.LogWarning($"DropCoinsOnDeath player {OwnerClientId} found but has 0 coins");
            return;
        }

        Debug.Log($"Found {coins} coins from {OwnerClientId}");

        for (int i = 0; i < coins; i++)
        {
            Debug.Log($"Spawning {i + 1} in {dropPosition}");
            GameObject coin = Instantiate(_coinPrefab, dropPosition, Quaternion.identity);
            coin.GetComponent<NetworkObject>().Spawn(true);
            Debug.Log($"[DropCoinsOnDeath] spawned coin {i + 1}");
        }

       CoinCount.Value = 0;
    }


    public void OnPlayerSpawned(Player player)
    {
        CoinCount.OnValueChanged += OnCoinCountChanged;

        if (!IsServer)
        {
            return;
        }
    }

    public void OnCoinCountChanged(int previous, int current)
    {
        _coinText.text = $"{current}";
    }

    public void OnCoinCollected()
    {
        Debug.Log("OnCoinCollected invoked");

        CoinCount.Value++;

        if (CoinCount.Value >= 3)
        {
            Debug.Log($"Player {CoinCount} completed task");
        }
    }

   
}
