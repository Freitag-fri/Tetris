using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Menu")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject volumeSlider;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject mainMenuButton;
        [SerializeField] private GameObject restartButton;

        [Header("Result Game")]
        [SerializeField] private GameObject resultGamePanel;
        [SerializeField] private GameObject newGameButton;
        [SerializeField] private TMP_Text finaleScore;
        [SerializeField] private TMP_Text finaleLevel;
        [SerializeField] private TMP_Text finaleLines;

        [Header("Game Statistics")]
        [SerializeField] private TMP_Text textScore;
        [SerializeField] private TMP_Text textTotalNumberClearLines;
        [SerializeField] private TMP_Text textGameLevel;

        [Space]
        [SerializeField] private Image shadowObject;


        private const float ShadowFadeInDuration = 0.15f;
        private const float ShadowFadeOutDuration = 0.1f;
        private const float ShadowFadeInStartAlpha = 0.5f;
        private const float ShadowFadeInEndAlpha = 0.8f;
        private const float ResultPanelAnimDuration = 0.25f;

        public void ChangeSceneToMainMenu()
        {
            SceneManager.LoadScene("MenuScene");
        }

        public void ShowGameScreen()
        {
            HideShadow();
            HideSettingsMenu();
            resultGamePanel.SetActive(false);
        }

        public void ShowPauseScreen()
        {
            ShowShadow();
            ShowSettingsMenu();
        }

        public void ShowResultScreen(StatisticParams statisticParams, bool isNewRecord)
        {
            ShowShadow();
            if(isNewRecord)
                finaleScore.text = $"New Record: {statisticParams.score}";
            else
                finaleScore.text = $"Score: {statisticParams.score}";

            finaleLevel.text = $"Level: {statisticParams.gameLevel}";
            finaleLines.text = $"Lines: {statisticParams.totalNumberClearLines}";

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
            shadowObject.gameObject.SetActive(true);
            shadowObject
                .DOFade(ShadowFadeInEndAlpha, ShadowFadeInDuration)
                .From(ShadowFadeInStartAlpha);
        }

        private void HideShadow()
        {
            if (!shadowObject.gameObject.activeSelf) return;

            shadowObject
                .DOFade(0f, ShadowFadeOutDuration)
                .OnComplete(() => shadowObject.gameObject.SetActive(false));
        }

        private void PlayResultPanelAnimation()
        {
            resultGamePanel.SetActive(true);
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

        private void ShowSettingsMenu()
        {
            DOTween.Sequence()
                .Append(ShowPanel(menuPanel))
                .Append(ShowUiElement(continueButton))
                .Join(ShowUiElement(mainMenuButton))
                .Join(ShowUiElement(restartButton))
                .Join(ShowSlider(volumeSlider));
        }

        private void HideSettingsMenu()
        {
            DOTween.Sequence()
                .Append(HideUiElement(continueButton))
                .Join(HideUiElement(mainMenuButton))
                .Join(HideUiElement(restartButton))
                .Join(HideUiElement(volumeSlider))
                .Append(HideUiElement(menuPanel));
        }

        private Tween HideUiElement(GameObject uiElement)
        {
            if (!uiElement.activeSelf) return DOTween.Sequence();
            return uiElement.transform
                .DOScale(Vector3.zero, ResultPanelAnimDuration - 0.07f)
                .OnComplete(() => uiElement.SetActive(false));
        }

        private Tween ShowUiElement(GameObject uiElement)
        {
            return DOTween.Sequence()
                .Join(uiElement.transform
                    .DOScale(Vector3.one, ResultPanelAnimDuration + 0.1f)
                    .From(Vector3.zero))
                .Join(uiElement.transform
                    .DOLocalMoveY(uiElement.transform.localPosition.y, ResultPanelAnimDuration + 0.1f)
                    .From(uiElement.transform.localPosition.y - 10)
                    .OnStart(() => uiElement.SetActive(true)));
        }

        private Tween ShowSlider(GameObject slider)
        {
            return DOTween.Sequence()
                .Append(slider.transform
                    .DOScale(Vector3.one * 2.8f, ResultPanelAnimDuration + 0.1f)
                    .From(Vector3.zero))
                .Join(slider.transform
                    .DOLocalMoveY(slider.transform.localPosition.y, ResultPanelAnimDuration + 0.1f)
                    .From(slider.transform.localPosition.y - 10)
                .OnStart(() => slider.SetActive(true)));
        }

        private Tween ShowPanel(GameObject panel)
        {
            return panel.transform
                .DOScale(Vector3.one, ResultPanelAnimDuration)
                .From(Vector3.one * 0.5f)
                .SetEase(Ease.OutBack)
                .OnStart(() => panel.SetActive(true));
        }
    }
}