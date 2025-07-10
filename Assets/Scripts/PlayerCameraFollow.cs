using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;

    private Transform _target;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position + _offset;
        transform.LookAt(_target);
    }

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }
}
