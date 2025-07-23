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
        GameSettings.Instance.Load();

        _fpsDropdown.value = GameSettings.Instance.FPS switch
        {
            30 => 0,
            60 => 1,
            120 => 2,
            _ => 1
        };

        _vsyncToggle.isOn = GameSettings.Instance.VSync;
    }

    private void SaveSettings()
    {
        GameSettings.Instance.FPS = _fpsDropdown.value switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            _ => 60
        };

        GameSettings.Instance.VSync = _vsyncToggle.isOn;

        GameSettings.Instance.Save();         
        GameSettings.Instance.Apply();
        HideMenu();                            
    }


    private void HideMenu()
    {
        gameObject.SetActive(false);
    }
}


