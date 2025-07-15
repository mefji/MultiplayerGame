using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private void OnValidate()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }
}
