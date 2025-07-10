using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Coin : Collectable
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private AudioClip _pickupSound;
    [SerializeField] private int _value = 1;
    private bool _collected = false;
    private float _rotationSpeed = 300f;
    private float _currentAngle = 0f;


    private void Update()
    {
        _currentAngle += _rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, _currentAngle, 0f);
    }

    private IEnumerator FlyUpAndShrink()
    {
        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 3f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = _curve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, endPos, easedT);
            transform.localScale = Vector3.Lerp(startScale, endScale, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        GetComponent<NetworkObject>().Despawn();
    }

    public override void OnCollect(Player player)
    {
        if (!IsServer || !IsSpawned || _collected)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(_pickupSound, transform.position);  
        StartCoroutine(FlyUpAndShrink());
        _collected = true;
    }
}