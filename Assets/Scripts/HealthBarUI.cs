using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _healthFill;

    private void Update()
    {
        if (_player == null || !_player.IsSpawned)
        {
            return;
        }

        int currentHealth = _player.HealthPoints;
        float fillAmount = (float)currentHealth / Player.MaxHealth;
        _healthFill.fillAmount = fillAmount;
    }
}
