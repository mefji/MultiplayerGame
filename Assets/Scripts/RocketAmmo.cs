using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RocketAmmo : Collectable
{
    [SerializeField] private int _rocketAmount = 1;
    private float _rotationSpeed = 300f;
    private float _currentAngle = 0f;

    private void Update()
    {
        _currentAngle += _rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
    }

    public override void OnCollect(Player player)
    {
        Debug.Log($"OnCollect called for player {player.OwnerClientId}");
        player.AddRockets(_rocketAmount);
        Debug.Log($"[RocketAmmo] Player {player.OwnerClientId} collected {_rocketAmount} rockets");
        Destroy(gameObject);
    }
}
