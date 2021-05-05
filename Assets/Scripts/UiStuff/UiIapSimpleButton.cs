using System;
using UnityEngine;
using UnityEngine.UI;

public class UiIapSimpleButton : MonoBehaviour
{
    public Action<int> OnIapButtonPressed;
    [HideInInspector] private Button button;
    public int id;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnIapButtonPressed(id));
    }

    private void OnDestroy()
    {
        button.onClick.AddListener(() => OnIapButtonPressed(id));
    }
}