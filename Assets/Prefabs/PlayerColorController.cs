using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorController : NetworkBehaviour
{
    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _blueMaterial;
    [SerializeField] private float _animationSpeed = 3f;
    [SerializeField] private Renderer[] _renderers;

    private void OnValidate()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        if (IsOwnedByServer)
        {
            foreach (Renderer rend in _renderers)
            {
                rend.material = _redMaterial;
            }
        }
        else
        {
            foreach (Renderer rend in _renderers)
            {
                rend.material = _blueMaterial;
            }
        }

        if (IsOwner)
        {
            foreach (Renderer rend in _renderers)
            {
                StartCoroutine(ChangeColor(rend.material));
            }
        }
    }

    private IEnumerator ChangeColor(Material mat)
    {
        Color baseColor = mat.GetColor("_BaseColor");
        Color additionalColor = Color.Lerp(Color.black, baseColor, 0.5f);

        while (enabled)
        {
            float time = Time.time * _animationSpeed;
            time = Mathf.Sin(time) * 0.5f + 0.5f;
            mat.SetColor("_BaseColor", Color.Lerp(baseColor, additionalColor, time));

            yield return null;
        }
    }
}

