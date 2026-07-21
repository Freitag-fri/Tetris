using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private GameObject resultGamePanel;
        [SerializeField] private GameObject newGameButton;
        [SerializeField] private Image shadowObject;

        [SerializeField] private TMP_Text textScore;
        [SerializeField] private TMP_Text textTotalNumberClearLines;
        [SerializeField] private TMP_Text textGameLevel;
        [SerializeField] private TMP_Text finaleScore;
        [SerializeField] private TMP_Text finaleLevel;
        [SerializeField] private TMP_Text finaleLines;

        private const float ShadowFadeInDuration = 0.15f;
        private const float ShadowFadeOutDuration = 0.1f;
        private const float ShadowFadeInStartAlpha = 0.5f;
        private const float ShadowFadeInEndAlpha = 0.8f;
        private const float ResultPanelAnimDuration = 0.2f;

        public void ShowGameScreen()
        {
            HideShadow();
            resultGamePanel.SetActive(false);
        }

        public void ShowPauseScreen()
        {
            ShowShadow();
        }

        public void ShowResultScreen(StatisticParams statisticParams, bool isNewRecord)
        {
            if(isNewRecord)
                finaleScore.text = $"New Record: {statisticParams.score}";
            else
                finaleScore.text = $"Score: {statisticParams.score}";

            finaleLevel.text = $"Level: {statisticParams.gameLevel}";
            finaleLines.text = $"Lines: {statisticParams.totalNumberClearLines}";

            resultGamePanel.SetActive(true);
            PlayResultPanelAnimation();
        }

        public void SetStatisticParams(StatisticParams statisticParams)
        {
            textScore.text = statisticParams.score.ToString();
            textTotalNumberClearLines.text = statisticParams.totalNumberClearLines.ToString();
            textGameLevel.text = statisticParams.gameLevel.ToString();
        }

        private void ShowShadow()
        {
            shadowObject
                .DOFade(ShadowFadeInEndAlpha, ShadowFadeInDuration)
                .From(ShadowFadeInStartAlpha)
                .OnStart(() => shadowObject.gameObject.SetActive(true));
        }

        private void HideShadow()
        {
            shadowObject
                .DOFade(0f, ShadowFadeOutDuration)
                .OnComplete(() => shadowObject.gameObject.SetActive(false));
        }

        private void PlayResultPanelAnimation()
        {
            DOTween.Sequence()
                .Append(resultGamePanel.transform
                    .DOScale(Vector3.one, ResultPanelAnimDuration)
                    .From(Vector3.one * 0.5f)
                    .SetEase(Ease.OutBack))
                .Append(newGameButton.transform
                    .DOScale(Vector3.one, ResultPanelAnimDuration)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack));
        }
    }
}