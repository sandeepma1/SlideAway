using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiShopBallItem : MonoBehaviour
{
    public Action<string> OnButtonClicked;
    private Button button;
    private TextMeshProUGUI valueText;
    private RectTransform rect;
    private string ballId;

    public RectTransform GetRect()
    {
        return rect;
    }

    public void InitButton(string id, string value)
    {
        rect = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        valueText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnButtonClick);
        ballId = id;
        valueText.text = value;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        OnButtonClicked?.Invoke(ballId);
    }
}