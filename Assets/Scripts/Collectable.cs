using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Collectable : NetworkBehaviour
{
    public abstract void OnCollect(Player player);
}
