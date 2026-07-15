using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject gameCanvas; // for game over screen
        [SerializeField] private GameObject pauseGameCanvas; // for pause game screen
        [SerializeField] private GameObject resultGameCanvas; // for result game screen
        [SerializeField] private GameObject shadowCanvas; // for shadowing background when pause or result game screen is active
        [SerializeField] private GameObject resultGamePanel;
        [SerializeField] private GameObject newGameButton;

        [SerializeField] private Image shadowObject;

        [SerializeField] private TMP_Text textScore;
        [SerializeField] private TMP_Text textTotalNumberCleanLines;
        [SerializeField] private TMP_Text textGameLevel;
        [SerializeField] private TMP_Text finaleScore;
        [SerializeField] private TMP_Text finaleLevel;
        [SerializeField] private TMP_Text finaleLines;

        private const float ShadowFadeInDuration = 0.15f;
        private const float ShadowFadeOutDuration = 0.1f;
        private const float ShadowFadeInStartAlpha = 0.5f;
        private const float ShadowFadeInEndAlpha = 0.8f;
        private const float ResultPanelAnimDuration = 0.2f;

        public void SetStartGameCanvas()
        {
            SetActiveCanvases(game: true, pause: false, result: false);
            HideShadow();
        }

        public void SetPauseGameCanvas()
        {
            SetActiveCanvases(game: false, pause: true, result: false);
            ShowShadow();
        }

        public void SetResultGameCanvas(StatisticParams statisticParams)
        {
            finaleScore.text = $"Score: {statisticParams.score}";
            finaleLevel.text = $"Level: {statisticParams.gameLevel}";
            finaleLines.text = $"Lines: {statisticParams.totalNumberCleanLines}";

            SetActiveCanvases(game: false, pause: false, result: true);
            PlayResultPanelAnimation();
        }

        public void SetStatisticParams(StatisticParams statisticParams)
        {
            textScore.text = statisticParams.score.ToString();
            textTotalNumberCleanLines.text = statisticParams.totalNumberCleanLines.ToString();
            textGameLevel.text = statisticParams.gameLevel.ToString();
        }

        private void SetActiveCanvases(bool game, bool pause, bool result)
        {
            gameCanvas.SetActive(game);
            pauseGameCanvas.SetActive(pause);
            resultGameCanvas.SetActive(result);
        }

        private void ShowShadow()
        {
            if (shadowCanvas.activeInHierarchy)
                return;

            shadowObject
                .DOFade(ShadowFadeInEndAlpha, ShadowFadeInDuration)
                .From(ShadowFadeInStartAlpha)
                .OnStart(() => shadowCanvas.SetActive(true));
        }

        private void HideShadow()
        {
            if (!shadowCanvas.activeInHierarchy)
                return;

            shadowObject
                .DOFade(0f, ShadowFadeOutDuration)
                .OnComplete(() => shadowCanvas.SetActive(false));
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