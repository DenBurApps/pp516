using UnityEngine;
using DG.Tweening;

public static class MainScreenTravelsViewExtensions
{
    public static void EnableWithAnimation(this MainScreenTravelsView view, float duration, Ease ease)
    {
        if (view == null) return;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return;

        view.gameObject.SetActive(true);

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, duration).SetEase(ease)
            .OnComplete(() => visibilityHandler.EnableScreen());
    }

    public static Tween AnimateFadeIn(this MainScreenTravelsView view, float duration, Ease ease)
    {
        if (view == null) return null;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return null;

        view.gameObject.SetActive(true);

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        return canvasGroup.DOFade(1f, duration).SetEase(ease)
            .OnComplete(() => visibilityHandler.EnableScreen());
    }

    public static Tween AnimateFadeOut(this MainScreenTravelsView view, float duration, Ease ease)
    {
        if (view == null) return null;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return null;

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        return canvasGroup.DOFade(0f, duration).SetEase(ease)
            .OnComplete(() =>
            {
                visibilityHandler.DisableScreen();
                view.gameObject.SetActive(false);
            });
    }

    public static void ToggleCreateTripButtonWithAnimation(this MainScreenTravelsView view, bool show, float duration,
        Ease ease)
    {
        if (view == null) return;

        Transform buttonTransform = view.transform.Find("CreateTripButton");
        if (buttonTransform == null) return;

        CanvasGroup buttonCanvasGroup = buttonTransform.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null) return;

        float targetAlpha = show ? 1f : 0f;
        buttonCanvasGroup.DOFade(targetAlpha, duration).SetEase(ease)
            .OnComplete(() =>
            {
                buttonCanvasGroup.interactable = show;
                buttonCanvasGroup.blocksRaycasts = show;
            });
    }

    public static void EnableEmptyHistoryWindowWithAnimation(this MainScreenTravelsView view, float duration, Ease ease)
    {
        if (view == null) return;

        Transform emptyWindowTransform = view.transform.Find("EmptyHistoryWindow");
        if (emptyWindowTransform == null) return;

        var visibilityHandler = emptyWindowTransform.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return;

        emptyWindowTransform.gameObject.SetActive(true);

        CanvasGroup windowCanvasGroup = emptyWindowTransform.GetComponent<CanvasGroup>();
        if (windowCanvasGroup == null) windowCanvasGroup = emptyWindowTransform.gameObject.AddComponent<CanvasGroup>();

        windowCanvasGroup.alpha = 0f;

        RectTransform rectTransform = emptyWindowTransform as RectTransform;
        if (rectTransform != null)
        {
            Vector3 originalScale = rectTransform.localScale;
            rectTransform.localScale = originalScale * 0.8f;

            Sequence sequence = DOTween.Sequence();
            sequence.Join(windowCanvasGroup.DOFade(1f, duration).SetEase(ease));
            sequence.Join(rectTransform.DOScale(originalScale, duration).SetEase(ease));
            sequence.OnComplete(() => visibilityHandler.EnableScreen());
            sequence.Play();
        }
        else
        {
            windowCanvasGroup.DOFade(1f, duration).SetEase(ease)
                .OnComplete(() => visibilityHandler.EnableScreen());
        }
    }

    public static void DisableEmptyHistoryWindowWithAnimation(this MainScreenTravelsView view, float duration,
        Ease ease)
    {
        if (view == null) return;

        Transform emptyWindowTransform = view.transform.Find("EmptyHistoryWindow");
        if (emptyWindowTransform == null) return;

        var visibilityHandler = emptyWindowTransform.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return;

        CanvasGroup windowCanvasGroup = emptyWindowTransform.GetComponent<CanvasGroup>();
        if (windowCanvasGroup == null) windowCanvasGroup = emptyWindowTransform.gameObject.AddComponent<CanvasGroup>();

        windowCanvasGroup.DOFade(0f, duration).SetEase(ease)
            .OnComplete(() =>
            {
                visibilityHandler.DisableScreen();
                emptyWindowTransform.gameObject.SetActive(false);
            });
    }
}

public static class MainScreenNotesViewExtensions
{
    public static void EnableWithAnimation(this MainScreenNotesView view, float duration, Ease ease)
    {
        if (view == null) return;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return;

        view.gameObject.SetActive(true);

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, duration).SetEase(ease)
            .OnComplete(() => visibilityHandler.EnableScreen());
    }

    public static Tween AnimateFadeIn(this MainScreenNotesView view, float duration, Ease ease)
    {
        if (view == null) return null;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return null;

        view.gameObject.SetActive(true);

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        return canvasGroup.DOFade(1f, duration).SetEase(ease)
            .OnComplete(() => visibilityHandler.EnableScreen());
    }

    public static Tween AnimateFadeOut(this MainScreenNotesView view, float duration, Ease ease)
    {
        if (view == null) return null;

        var visibilityHandler = view.GetComponent<ScreenVisabilityHandler>();
        if (visibilityHandler == null) return null;

        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = view.gameObject.AddComponent<CanvasGroup>();

        return canvasGroup.DOFade(0f, duration).SetEase(ease)
            .OnComplete(() =>
            {
                visibilityHandler.DisableScreen();
                view.gameObject.SetActive(false);
            });
    }
}

public static class FilledTripDataWindowExtensions
{
    public static void EnableWithAnimation(this FilledTripDataWindow window, float duration, Ease ease, float slideDistance)
    {
        if (window == null) return;

        window.Enable();

        RectTransform rectTransform = window.transform as RectTransform;
        if (rectTransform == null) return;

        Vector2 originalPosition = rectTransform.anchoredPosition;
        
        rectTransform.anchoredPosition = new Vector2(originalPosition.x - slideDistance, originalPosition.y);

        Sequence sequence = DOTween.Sequence();

        // Slide in
        sequence.Join(rectTransform.DOAnchorPos(originalPosition, duration)
            .SetEase(ease));

        rectTransform.localScale = Vector3.one * 0.9f;
        sequence.Join(rectTransform.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack));

        sequence.Play();
    }

    public static void DisableWithAnimation(this FilledTripDataWindow window, float duration, Ease ease)
    {
        if (window == null) return;

        RectTransform rectTransform = window.transform as RectTransform;
        if (rectTransform == null)
        {
            window.Disable();
            return;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + 200f, duration)
            .SetEase(ease));

        sequence.Join(rectTransform.DOScale(Vector3.one * 0.8f, duration)
            .SetEase(Ease.InBack));

        sequence.OnComplete(() => window.Disable());

        sequence.Play();
    }
}