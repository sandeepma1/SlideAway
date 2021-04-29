using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UiLoadingCanvas : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animation anim;
    private const float animSpeed = 0.5f;
    private bool isLoadingAnimationDone = false;

    private void Awake()
    {
        mainPanel.gameObject.SetActive(true);
        PlayerDataManager.OnPlayerDataLoaded += OnPlayerDataLoaded;
    }

    private void Start()
    {
        anim.Play();
        if (PlayerDataManager.Instance.isPlayerDataLoaded)
        {
            StartCoroutine(StartGameWithDeay());
        }
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnPlayerDataLoaded -= OnPlayerDataLoaded;
    }

    private void OnPlayerDataLoaded()
    {
        StartCoroutine(StartGameWithDeay());
    }

    private IEnumerator StartGameWithDeay()
    {
        if (isLoadingAnimationDone)
        {
            yield return null;
        }
        isLoadingAnimationDone = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.5f);
        canvasGroup.DOFade(0, animSpeed).OnComplete(() => mainPanel.SetActive(false));
    }
}