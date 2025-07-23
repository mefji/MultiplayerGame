using MirzaBeig.CinematicExplosionsFree;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameSettings
{
    public int FPS { get; set; }
    public bool VSync { get; set; }

    private static GameSettings _instance;

    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameSettings();
                _instance.Load();
            }

            return _instance;
        }
    }

    public void Load()
    {
        FPS = PlayerPrefs.GetInt(nameof(FPS), 60);
        VSync = PlayerPrefs.GetInt(nameof(VSync), 0) == 1;
    }

    public void Save()
    {
        PlayerPrefs.GetInt(nameof(FPS), FPS);
        PlayerPrefs.SetInt(nameof(VSync), VSync ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Apply()
    {
        Application.targetFrameRate = FPS;
        QualitySettings.vSyncCount = VSync ? 1 : 0;
    }
}
