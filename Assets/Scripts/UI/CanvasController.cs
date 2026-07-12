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
        [SerializeField] private GameObject resoultGameCanvas; // for resoult game screen
        [SerializeField] private GameObject shadowCanvas; // for shadowing background when pause or resoult game screen is active
        [SerializeField] private GameObject resoultGamePanel;
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
        private const float ResoultPanelAnimDuration = 0.2f;

        public void SetStartGameCanvas()
        {
            SetActiveCanvases(game: true, pause: false, resoult: false);
            HideShadow();
        }

        public void SetPauseGameCanvas()
        {
            SetActiveCanvases(game: false, pause: true, resoult: false);
            ShowShadow();
        }

        public void SetResoultGameCanvas(StatisticParams statisticParams)
        {
            finaleScore.text = $"Score: {statisticParams.score}";
            finaleLevel.text = $"Level: {statisticParams.gameLevel}";
            finaleLines.text = $"Lines: {statisticParams.totalNumberCleanLines}";

            SetActiveCanvases(game: false, pause: false, resoult: true);
            PlayResoultPanelAnimation();
        }

        public void SetStatisticParams(StatisticParams statisticParams)
        {
            textScore.text = statisticParams.score.ToString();
            textTotalNumberCleanLines.text = statisticParams.totalNumberCleanLines.ToString();
            textGameLevel.text = statisticParams.gameLevel.ToString();
        }

        private void SetActiveCanvases(bool game, bool pause, bool resoult)
        {
            gameCanvas.SetActive(game);
            pauseGameCanvas.SetActive(pause);
            resoultGameCanvas.SetActive(resoult);
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

        private void PlayResoultPanelAnimation()
        {
            DOTween.Sequence()
                .Append(resoultGamePanel.transform
                    .DOScale(Vector3.one, ResoultPanelAnimDuration)
                    .From(Vector3.one * 0.5f)
                    .SetEase(Ease.OutBack))
                .Append(newGameButton.transform
                    .DOScale(Vector3.one, ResoultPanelAnimDuration)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack));
        }
    }
}