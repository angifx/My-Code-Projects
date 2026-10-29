using UnityEngine;
using System.Collections;

public class TutorialHint : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isFading = false;
    private bool listenForInput = false;
    private bool isVisible = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (!listenForInput || isFading) return;

        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f;
        bool isJumping = Input.GetButtonDown("Jump");

        if (isMoving || isJumping)
        {
            HideHint();
        }
    }

    public void ToggleHint()
    {
        if (isVisible)
        {
            HideHint();
        }
        else
        {
            ShowHint();
        }
    }

    private void ShowHint()
    {
        StopAllCoroutines();
        isFading = false;
        isVisible = true;
        listenForInput = true;
        StartCoroutine(Fade(1f));
    }

    private void HideHint()
    {
        StopAllCoroutines();
        isFading = true;
        isVisible = false;
        listenForInput = false;
        StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        isFading = false;
    }
}