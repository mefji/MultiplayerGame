using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _camTransform;

    private void Awake()
    {
        _camTransform = Camera.main.transform;
    }
    private void LateUpdate()
    {
        Vector3 camForward = Camera.main.transform.forward;
        transform.rotation = Quaternion.LookRotation(camForward);
    }
}
