using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Dropdown _fpsDropdown;
    [SerializeField] private Toggle _vsyncToggle;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _cancelButton;

    private int _currentFPS;
    private bool _currentVSync;

    private void OnEnable()
    {
        LoadSettings();

        _saveButton.onClick.AddListener(SaveSettings);
        _cancelButton.onClick.AddListener(HideMenu);
    }

    private void OnDisable()
    {
        _saveButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
    }

    private void LoadSettings()
    {
        _currentFPS = GameSettings.Instance.FPS;
        _currentVSync = GameSettings.Instance.VSync;

        _fpsDropdown.value = _currentFPS switch
        {
            30 => 0,
            60 => 1,
            120 => 2,
            _ => 1
        };

        _vsyncToggle.isOn = _currentVSync;
    }

    private void SaveSettings()
    {
        _currentFPS = _fpsDropdown.value switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            _ => 60
        };

        _currentVSync = _vsyncToggle.isOn;

        GameSettings.Instance.FPS = _currentFPS;
        GameSettings.Instance.VSync = _currentVSync;
        GameSettings.Instance.Save();

        ApplySettings();
        HideMenu();
    }

    private void ApplySettings()
    {
        Application.targetFrameRate = _currentFPS;
        QualitySettings.vSyncCount = _currentVSync ? 1 : 0;
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
    }
}


