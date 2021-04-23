using UnityEngine;
using System;
using System.Collections;

public class UiGemsSpawnCanvas : MonoBehaviour
{
    public static Action<Transform> OnSpawnGem;
    public static Action<Vector2> OnSpawnGem2d;
    public static Action OnUpdateGems;
    [SerializeField] private RectTransform gemEndPosition;
    [SerializeField] private UiGemSpawn uiGemSpawnPrefab;
    private Canvas mainCanvas;
    private const float animSpeed = 1;

    private void Awake()
    {
        OnSpawnGem += SpawnGem;
        OnSpawnGem2d += SpawnGem2d;
        mainCanvas = GetComponent<Canvas>();
    }

    private void OnDestroy()
    {
        OnSpawnGem -= SpawnGem;
        OnSpawnGem2d -= SpawnGem2d;
    }

    private void SpawnGem(Transform worldTransform)
    {
        UiGemSpawn uiGemSpawn = Instantiate(uiGemSpawnPrefab, transform);
        uiGemSpawn.InitItem(worldTransform, mainCanvas, gemEndPosition.localPosition, animSpeed);
        PlayerDataManager.Instance.IncrementGems(1);
        StartCoroutine(UpdateGemAmountDelay());
    }

    private void SpawnGem2d(Vector2 position)
    {
        UiGemSpawn uiGemSpawn = Instantiate(uiGemSpawnPrefab, transform);
        uiGemSpawn.InitItem2d(position, gemEndPosition.localPosition, animSpeed);
        PlayerDataManager.Instance.IncrementGems(1);
        StartCoroutine(UpdateGemAmountDelay());
    }

    private IEnumerator UpdateGemAmountDelay()
    {
        yield return new WaitForSeconds(animSpeed);
        OnUpdateGems?.Invoke();
    }
}