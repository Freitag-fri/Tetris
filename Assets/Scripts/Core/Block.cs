using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    public class Block : MonoBehaviour
    {
        public YieldInstruction DestroyBlock()
        {
            return transform
                .DOScale(0, 0.25f)
                .SetEase(Ease.InExpo)
                .OnComplete(() => {
                    Destroy(gameObject);
                })
                .WaitForCompletion();
        }
    }
}