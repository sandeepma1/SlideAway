using UnityEngine;
using TMPro;

public class UserData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI diamondText;

    private int diamonds;
    private int Diamonds
    {
        get
        {
            return diamonds;
        }
        set
        {
            diamonds = value;
            diamondText.text = diamonds.ToString();
        }
    }

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

    private void OnApplicationQuit()
    {
        SaveGameUserData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameUserData();
        }
    }

    private void SaveGameUserData()
    {
        ZPlayerPrefs.SetInt(AppData.keyDiamond, diamonds);
    }
}