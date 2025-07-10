using Unity.Netcode;
using UnityEngine;

public class RocketSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private NetworkVariable<int> _rocketsLeft = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("RocketSpawner: Space key pressed by owner");

            if (_rocketsLeft.Value > 0)
            {
                Debug.Log("RocketSpawner: Requesting rocket spawn to server...");
                RequestSpawnRocketServerRpc(OwnerClientId);
            }
            else
            {
                Debug.Log("RocketSpawner: No rockets left!");
            }
        }
    }

    [ServerRpc]
    private void RequestSpawnRocketServerRpc(ulong ownerId)
    {
        if (_rocketsLeft.Value <= 0)
        {
            Debug.LogWarning("ServerRpc: No rockets left on server for client " + ownerId);
            return;
        }

        Debug.Log("ServerRpc: Spawning rocket for client " + ownerId);

        Vector3 pos = transform.position + transform.forward * 2f;
        Quaternion rot = transform.rotation;

        GameObject rocket = Instantiate(_rocketPrefab, pos, rot);
        rocket.GetComponent<Rocket>().ownerId = ownerId;
        rocket.GetComponent<NetworkObject>().Spawn();

        _rocketsLeft.Value--;
        Debug.Log("ServerRpc: Rocket spawned. Rockets remaining: " + _rocketsLeft.Value);
    }
}



