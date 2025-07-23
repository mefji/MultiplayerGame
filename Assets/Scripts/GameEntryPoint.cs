using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEntryPoint
{
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Main()
    {
        Debug.Log("GameEntryPoint.Main()");
        GameSettings.Instance.Apply();
    }
}