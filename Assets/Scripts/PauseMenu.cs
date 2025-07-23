using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private OptionsMenu _optionsMenuPrefab;
    private OptionsMenu _optionsMenuInstatiate;
    public bool IsActive => gameObject.activeSelf;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseMenu();
        }
    }

    private void Awake()
    {
        _optionsMenuInstatiate = Instantiate(_optionsMenuPrefab);
        _optionsMenuInstatiate.gameObject.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        gameObject.SetActive(true);
    }

    public void HidePauseMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _resumeButton.onClick.AddListener(HidePauseMenu);
        _optionsButton.onClick.AddListener(OptionsMenu);
        _exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        _resumeButton.onClick.RemoveListener(HidePauseMenu);
        _optionsButton.onClick.RemoveListener(OptionsMenu);
        _exitButton.onClick.RemoveListener(ExitGame);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void OptionsMenu()
    {
        _optionsMenuInstatiate.gameObject.SetActive(true);
    }
}
