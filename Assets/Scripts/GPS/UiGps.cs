using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiGps : MonoBehaviour
{
    private void Start()
    {
        GpsSignIn();
    }

    public void GpsSignIn()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Hud.SetHudText?.Invoke("Started Sign in");
        //This should work :(
        //PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
        //{
        //    Debug.LogError("Install GPS please ask user poup ");
        //});
        Social.localUser.Authenticate(OnAuthenticate);
    }

    private void OnAuthenticate(bool success)
    {
        if (success)
        {
            GPG_CloudSaveSystem.Instance.LoadFromCloud();
        }
    }

    public void OpenSave()
    {
        GPG_CloudSaveSystem.Instance.showUI();
    }
}
