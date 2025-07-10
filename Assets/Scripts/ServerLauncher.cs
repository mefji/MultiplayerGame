using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerLauncher : MonoBehaviour
{
    [SerializeField] private Button _buttonHost;
    [SerializeField] private Button _buttonClient;
    [SerializeField] private Button _buttonServer;

    private void Awake()
    {
        _buttonHost.onClick.AddListener(OnHostButtonClick);
        _buttonClient.onClick.AddListener(OnClientButtonClick);
        _buttonServer.onClick.AddListener(OnServerButtonClick);
    }

    private void OnHostButtonClick()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnClientButtonClick()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnServerButtonClick()
    {
        NetworkManager.Singleton.StartServer();
    }
}
