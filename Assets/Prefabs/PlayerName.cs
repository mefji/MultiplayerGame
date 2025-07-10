using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Player _player;
    private static readonly string[] _firstParts = { "Alex", "Chris", "John", "Max", "Sam", "Luke", "Jake", "Ben", "Tom", "Leo" };
    private static readonly string[] _secondParts = { "son", "ster", "ley", "ton", "win", "den", "ford", "man", "well", "rick" };
    // private NetworkVariable<FixedString128Bytes> _playerName;

    // public FixedString128Bytes PlayerNameValue => _playerName.Value;

    public override void OnNetworkSpawn()
    {
        _player.PlayerNameChanged += OnPlayerNameChanged;

        if (IsOwner)
        {
            StartCoroutine(SetName());
        }
        else
        {
            _nameText.text = _player.PlayerName.ToString();
        }
    }

    private void OnPlayerNameChanged(string name)
    {
        _nameText.text = name;
    }

    private void OnValidate()
    {
        _player = GetComponent<Player>();
    }

    private IEnumerator SetName()
    {
        yield return null;

        string generatedName;

        do
        {
            string first = _firstParts[Random.Range(0, _firstParts.Length)];
            string second = _secondParts[Random.Range(0, _firstParts.Length)];
            generatedName = first + second;

        } while (Player.Players.Values.Any(p => p.PlayerName == generatedName));

        _player.PlayerName = _nameText.text = generatedName;

    }
    /* public override void OnNetworkSpawn()
     {
         playerName.OnValueChanged += (oldName, newName) =>
         {
             _nameText.text = newName;
         };

         _nameText.text = playerName.Value;
     }*/
}
