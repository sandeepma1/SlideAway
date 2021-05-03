using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UiLoadingCanvas : Singleton<UiLoadingCanvas>
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animation anim;
    private const float animSpeed = 0.5f;
    private bool isLoadingAnimationDone = false;

    protected override void Awake()
    {
        base.Awake();
        loadingText.gameObject.SetActive(false);
        mainPanel.gameObject.SetActive(true);
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
    }

    private void Start()
    {
        anim.Play();
        if (Player.isPlayerDataLoaded)
        {
            StartCoroutine(StartGameWithDeay());
        }
    }

    private void OnDestroy()
    {
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
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
        yield return new WaitForSeconds(3f);
        loadingText.gameObject.SetActive(true);
    }
}