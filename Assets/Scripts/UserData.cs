using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UserData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI diamondText;


    private int Diamonds
    {
        get
        {
            return diamonds;
        }
        set
        {
            diamonds = value;
            ZPlayerPrefs.SetInt(AppData.keyDiamond, diamonds);
            diamondText.text = diamonds.ToString();
        }
    }
    private int diamonds;

    private void Awake()
    {
        ZPlayerPrefs.Initialize(SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier);
        BallController.OnAddDiamond += OnAddDiamond;
    }

    private void Start()
    {
        Diamonds = ZPlayerPrefs.GetInt(AppData.keyDiamond, 0);
    }

    private void OnDestroy()
    {
        BallController.OnAddDiamond -= OnAddDiamond;
    }

    private void OnAddDiamond()
    {
        Diamonds++;
    }
}