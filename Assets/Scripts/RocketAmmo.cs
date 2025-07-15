using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using UnityEngine;
using Unity.Netcode;

public class RocketAmmo : Collectable
{
    [SerializeField] private int _rocketAmount = 1;

    public override void OnCollect(Player player)
    {
        player.AddRockets(_rocketAmount);
        Debug.Log($"[RocketAmmo] ����� {player.OwnerClientId} �������� {_rocketAmount} �����");
        Destroy(gameObject);
    }
}
