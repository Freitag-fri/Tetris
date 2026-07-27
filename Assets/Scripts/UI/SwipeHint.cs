using Assets.Scripts;
using DG.Tweening;
using UnityEngine;

public class SwipeHint : MonoBehaviour
{
    [SerializeField] private int maxShows = 3;
    [SerializeField] private float delay = 2.5f;
    [SerializeField] private Animator animator;
    private Tween tween;

    // Show after delay, but only the first few times (maxShows)
    public void Show()
    {
        Hide();

        if (SaveData.SwipeHintShowCount >= maxShows)
            return;

        tween = DOVirtual.DelayedCall(delay, () => {
            gameObject.SetActive(true);
            animator.SetBool("bStartAnimation", true);
        });
    }

    public void HideAfterSwipe()
    {
        if (gameObject.activeSelf)
            SaveData.SwipeHintShowCount++;
        Hide();
    }

    public void Hide()
    {
        tween?.Kill();
        animator.SetBool("bStartAnimation", false);
        gameObject.SetActive(false);
    }
}
