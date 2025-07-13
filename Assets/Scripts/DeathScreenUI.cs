using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Show()
    {
        _canvasGroup.blocksRaycasts = true;
    }
}
