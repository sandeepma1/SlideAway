using UnityEngine;
using System;

public class UiGemsSpawnCanvas : MonoBehaviour
{
    public static Action<Transform> OnSpawnGem;
    public static Action OnUpdateGems;
    [SerializeField] private RectTransform gemEndPosition;
    [SerializeField] private UiGemSpawn uiGemSpawnPrefab;
    private Camera mainCamera;
    private Canvas mainCanvas;

    private void Awake()
    {
        OnSpawnGem += SpawnGem;
        mainCanvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        OnSpawnGem -= SpawnGem;
    }

    private void SpawnGem(Transform worldTransform)
    {
        UiGemSpawn uiResourceSpawn = Instantiate(uiGemSpawnPrefab, transform);
        uiResourceSpawn.InitItem(worldTransform, mainCanvas, gemEndPosition.localPosition, mainCamera);
        AppData.gems++;
        OnUpdateGems?.Invoke();
    }
}