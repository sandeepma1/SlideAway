using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopBallItem : MonoBehaviour
{
    public Action<int> OnButtonClicked;
    private Button button;
    private TextMeshProUGUI valueText;
    private int id;
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        valueText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void InitButton(int id, string value)
    {
        this.id = id;
        valueText.text = value;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        print(id);
        OnButtonClicked?.Invoke(id);
    }
}