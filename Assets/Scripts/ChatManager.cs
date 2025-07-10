using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
//using UnityEditor.VersionControl;
using UnityEngine;

public partial class ChatManager : NetworkBehaviour
{
    private bool _inputActive = false;
    private readonly static Regex _privateMessageRegex = new Regex("^@(\\w+)\\s+(.*)");

    public struct MessageData : INetworkSerializable
    {
        public FixedString128Bytes PlayerName;
        public FixedString128Bytes Text;

        public override string ToString()
        {
            return $"{PlayerName}: {Text}";
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Text);
        }
    }
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!_inputActive && Input.GetKeyDown(KeyCode.T))
        {
            _inputActive = true;
            GlobalChat.Instance.ChatBackground.SetActive(true);
            GlobalChat.Instance.ChatInput.ActivateInputField();
        }

        else if (_inputActive && Input.GetKeyDown(KeyCode.Escape))
        {
            _inputActive = false;
            GlobalChat.Instance.ChatInput.DeactivateInputField();
            GlobalChat.Instance.ChatBackground.SetActive(false);
        }

        if (_inputActive && Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrWhiteSpace(GlobalChat.Instance.ChatInput.text))
            {
                SubmitMessage(GlobalChat.Instance.ChatInput.text);
                GlobalChat.Instance.ChatInput.text = "";
            }

            GlobalChat.Instance.ChatInput.DeactivateInputField();
            GlobalChat.Instance.ChatBackground.gameObject.SetActive(false);
            _inputActive = false;
        }
    }

    private void Start()
    {
        GlobalChat.Instance.ChatBackground.gameObject.SetActive(false);
    }

    private void SubmitMessage(string message)
    {
        if (message.StartsWith("@"))
        {
            SendMessageServerRpc(message, default);
            GlobalChat.Instance.ChatLog.text += $"\n{message}";
            return;
        }

        SendChatMessageServerRpc(message);
    }

    [ServerRpc]
    private void SendChatMessageServerRpc(FixedString128Bytes message, ServerRpcParams rpcParams = default)
    {
        BroadcastMessageClientRpc(message);
        Debug.Log("ServerRpc: " + message);
    }

    [ClientRpc]
    private void BroadcastMessageClientRpc(FixedString128Bytes message)
    {
        GlobalChat.Instance.ChatLog.text += $"\n{message}";
        Debug.Log("ClientRpc: " + message);
    }

    [ServerRpc]
    private void SendMessageServerRpc(FixedString128Bytes message, ServerRpcParams rpcParams)
    {
        Debug.Log("SendMessageServerRpc invoked");

        Player senderPlayer = Player.Players[rpcParams.Receive.SenderClientId];

        if (ParsePlayerName(message.ConvertToString(), out string playerName, out string playerMessage))
        {
            Player targetPlayer = Player.FindPlayerByName(playerName, System.StringComparison.OrdinalIgnoreCase);

            if (targetPlayer == null)
            {
                Debug.LogError("targetPlayer is null");
                return;
            }

            List<ulong> ids = new List<ulong>() { targetPlayer.OwnerClientId };
            MessageData messageData = new MessageData() { PlayerName = senderPlayer.PlayerName, Text = playerMessage };
            SendMessageClientRpc(messageData, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ids } });
        }
        else
        {
            Debug.LogError("Failed to parse message");
        }
    }

    [ClientRpc]
    private void SendMessageClientRpc(MessageData message, ClientRpcParams clientRpcParams)
    {
        GlobalChat.Instance.ChatLog.text += $"\n{message}";
        Debug.Log($"SendMessageClientRpc: {message}");
    }

    private static bool ParsePlayerName(string textMessage, out string playerName, out string message)
    {
        Match match = _privateMessageRegex.Match(textMessage);

        if (match.Success)
        {
            playerName = match.Groups[1].Value;
            message = match.Groups[2].Value;
            return true;
        }
        else
        {
            playerName = null;
            message = null;
            return false;
        }
    }
}
