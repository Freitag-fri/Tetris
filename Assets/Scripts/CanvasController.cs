using System.Collections;
using TMPro;
using UnityEngine;

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

        [SerializeField] private TMP_Text textScore;
        [SerializeField] private TMP_Text textTotalNumberCleanLines;
        [SerializeField] private TMP_Text textGameLevel;
        [SerializeField] private TMP_Text finaleScore;

        public void SetStartGameCanvas()
        {
            gameCanvas.SetActive(true);
            pauseGameCanvas.SetActive(false);
            resoultGameCanvas.SetActive(false);
        }
        public void SetPauseGameCanvas()
        {
            gameCanvas.SetActive(false);
            pauseGameCanvas.SetActive(true);
            resoultGameCanvas.SetActive(false);
        }
        public void SetResoultGameCanvas(StatisticParams statisticParams)
        {
            gameCanvas.SetActive(false);
            pauseGameCanvas.SetActive(false);
            resoultGameCanvas.SetActive(true);
            finaleScore.text = "Score: " + statisticParams.score.ToString();
        }

        public void SetStatisticParams(StatisticParams statisticParams)
        {
            textScore.text = statisticParams.score.ToString();
            textTotalNumberCleanLines.text = statisticParams.totalNumberCleanLines.ToString();
            textGameLevel.text = statisticParams.gameLevel.ToString();
        }
    }
}