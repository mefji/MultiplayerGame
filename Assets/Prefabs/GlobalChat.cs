using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlobalChat : MonoBehaviour
{
    [SerializeField] private TMP_InputField _chatInput;
    [SerializeField] private TMP_Text _chatLog;
    [SerializeField] private GameObject _chatBackground;

    public TMP_InputField ChatInput => _chatInput;
    public TMP_Text ChatLog => _chatLog;
    public GameObject ChatBackground => _chatBackground;
    public static GlobalChat Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddMessage(string message)
    {
        _chatLog.text += message + "\n";
    }
}
