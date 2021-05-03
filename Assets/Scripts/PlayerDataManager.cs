using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    [SerializeField] private bool showAnalyticsResults;

    protected override void Awake()
    {
        base.Awake();
        Shop.LoadShopDatabse();
        Player.InitPlayer();
        AnalyticsManager.showAnalyticsResults = showAnalyticsResults;
    }

    #region Exit or Pause
    private void OnApplicationQuit()
    {
        Player.SaveGameUserData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) { Player.SaveGameUserData(); }
    }
    #endregion
}
