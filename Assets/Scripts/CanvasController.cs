using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public struct StatisticParams
    {
        public StatisticParams (int score, int totalNumberCleanLines, int gameLevel)
        {
            this.score = score;
            this.totalNumberCleanLines = totalNumberCleanLines;
            this.gameLevel = gameLevel;
        }
        public int score;
        public int totalNumberCleanLines;
        public int gameLevel;
    }

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
        [SerializeField] private TMP_Text textGameLevel; // textGameLevel.SetText("{0}", score); rewrite for all
        [SerializeField] private TMP_Text finaleScore;
        [SerializeField] private TMP_Text finaleLevel;
        [SerializeField] private TMP_Text finaleLines;


        public void SetStartGameCanvas()
        {
            gameCanvas.SetActive(true);
            pauseGameCanvas.SetActive(false);
            resoultGameCanvas.SetActive(false);
            if (shadowCanvas.activeInHierarchy)
            {
                shadowObject
                    .DOFade(0f, 0.1f)
                    .OnComplete(() => shadowCanvas.SetActive(false));
            }
        }
        public void SetPauseGameCanvas()
        {
            gameCanvas.SetActive(false);
            pauseGameCanvas.SetActive(true);
            resoultGameCanvas.SetActive(false);

            if (!shadowCanvas.activeInHierarchy)
            {
                shadowObject
                    .DOFade(0.8f, 0.15f)
                    .From(0.5f)
                    .OnStart(() => shadowCanvas.SetActive(true));
            }
        }
        public void SetResoultGameCanvas(StatisticParams statisticParams)
        {
            finaleScore.text = "Score: " + statisticParams.score.ToString();
            finaleLevel.text = "Level: " + statisticParams.gameLevel.ToString();
            finaleLines.text = "Lines: " + statisticParams.totalNumberCleanLines.ToString();

            gameCanvas.SetActive(false);
            pauseGameCanvas.SetActive(false);
            resoultGameCanvas.SetActive(true);

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(resoultGamePanel.transform.DOScale(Vector3.one, 0.2f).From(new Vector3(0.5f, 0.5f, 0.5f)).SetEase(Ease.OutBack)).Append(newGameButton.transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero).SetEase(Ease.OutBack));
        }

        public void SetStatisticParams(StatisticParams statisticParams)
        {
            textScore.text = statisticParams.score.ToString();
            textTotalNumberCleanLines.text = statisticParams.totalNumberCleanLines.ToString();
            textGameLevel.text = statisticParams.gameLevel.ToString();
        }
    }
}