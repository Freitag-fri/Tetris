using Assets.Scripts;
using DG.Tweening;
using UnityEngine;

public class SwipeHint : MonoBehaviour
{
    [SerializeField] private int maxShows = 3;
    [SerializeField] private float delay = 2.5f;
    private Tween tween;

    // Show after delay, but only the first few times
    public void Show()
    {
        tween?.Kill();
        gameObject.SetActive(false);

        if (SaveData.SwipeHintShowCount >= maxShows)
            return;

        tween = DOVirtual.DelayedCall(delay, () => gameObject.SetActive(true));
    }

    public void HideAfterSwipe()
    {
        tween?.Kill();

        if (gameObject.activeSelf)
            SaveData.SwipeHintShowCount++;

        gameObject.SetActive(false);
    }

    public void Hide()
    {
        tween?.Kill();
        gameObject.SetActive(false);
    }
}
