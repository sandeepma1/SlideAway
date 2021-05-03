using UnityEngine;
using System;
using System.Collections;

public class UiGemsSpawnCanvas : MonoBehaviour
{
    public static Action<Transform> OnSpawnSingleGem3D;
    public static Action<Vector2> OnSpawnSingleGem2d;
    public static Action<int> OnSpawnMultipleGem2d;
    [SerializeField] private RectTransform gemEndPosition;
    [SerializeField] private RectTransform gemSpawnRect;
    [SerializeField] private UiGemSpawn uiGemSpawnPrefab;
    private Canvas mainCanvas;

    private void Awake()
    {
        mainCanvas = GetComponent<Canvas>();
        OnSpawnSingleGem3D += SpawnSingleGem;
        OnSpawnSingleGem2d += SpawnSingleGem2d;
        OnSpawnMultipleGem2d += SpawnMultipleGems2D;
    }

    private void OnDestroy()
    {
        OnSpawnSingleGem3D -= SpawnSingleGem;
        OnSpawnSingleGem2d -= SpawnSingleGem2d;
        OnSpawnMultipleGem2d -= SpawnMultipleGems2D;
    }

    private void SpawnMultipleGems2D(int amount)
    {
        StartCoroutine(SpawnMultipleGems2DSub(amount));
    }

    private IEnumerator SpawnMultipleGems2DSub(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPoint = gemSpawnRect.GetRandomPointInRectTransform();
            UiGemSpawn uiGemSpawn = Instantiate(uiGemSpawnPrefab, transform);
            uiGemSpawn.InitItem2d(randomPoint, gemEndPosition.localPosition, AppData.gemsAnimSpeed);
            yield return new WaitForEndOfFrame();
        }
        Player.IncrementGems(amount);
        StartCoroutine(UpdateGemAmountDelay());
    }

    private void SpawnSingleGem(Transform worldTransform)
    {
        UiGemSpawn uiGemSpawn = Instantiate(uiGemSpawnPrefab, transform);
        uiGemSpawn.InitItem(worldTransform, mainCanvas, gemEndPosition.localPosition, AppData.gemsAnimSpeed);
        Player.IncrementGems(1);
        StartCoroutine(UpdateGemAmountDelay());
    }

    private void SpawnSingleGem2d(Vector2 position)
    {
        UiGemSpawn uiGemSpawn = Instantiate(uiGemSpawnPrefab, transform);
        uiGemSpawn.InitItem2d(position, gemEndPosition.localPosition, AppData.gemsAnimSpeed);
        Player.IncrementGems(1);
        StartCoroutine(UpdateGemAmountDelay());
    }

    private IEnumerator UpdateGemAmountDelay()
    {
        yield return new WaitForSeconds(AppData.gemsAnimSpeed);
        UiPlayerDataHud.OnUpdateGemsValue?.Invoke();
    }
}