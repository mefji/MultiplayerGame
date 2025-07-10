using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinCount = 10;
    [SerializeField] private Vector2 spawnAreaX = new Vector2(-10, 10);
    [SerializeField] private Vector2 spawnAreaZ = new Vector2(-10, 10);
    [SerializeField] private float noiseScale = 3f;
    [SerializeField] private float threshold = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer || !IsOwner)
        {
            return;
        }

        List<Vector3> usedPositions = new List<Vector3>();

        int spawned = 0;
        int attempts = 0;

        while (spawned < coinCount && attempts < 1000)
        {
            attempts++;

            float x = Random.Range(spawnAreaX.x, spawnAreaX.y);
            float z = Random.Range(spawnAreaZ.x, spawnAreaZ.y);

            float noiseValue = Mathf.PerlinNoise(x / noiseScale, z / noiseScale);

            if (noiseValue > threshold && IsFarEnough(new Vector3(x, 0, z), usedPositions))
            {
                Vector3 position = new Vector3(x, 0, z);
                usedPositions.Add(position);
                Vector3 spawnPosition = position + Vector3.up * 0.7f;
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                GameObject coin = Instantiate(coinPrefab, spawnPosition, rotation);
                coin.GetComponent<NetworkObject>().Spawn(true);
                spawned++;
            }
        }
    }

    private bool IsFarEnough(Vector3 position, List<Vector3> positions)
    {
        float minDistance = 1f;

        foreach (var pos in positions)
        {
            if (Vector3.Distance(position, pos) < minDistance)
            {
                return false;
            }
        }

        return true;
    }
}
