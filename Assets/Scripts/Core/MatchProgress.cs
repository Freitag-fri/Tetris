using System.Collections.Generic;

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

    public class MatchProgress
    {
        private const int pointsForLine = 200;

        private readonly IReadOnlyDictionary<int, int> pointsForLinesConfiguration = new Dictionary<int, int>
        {
            {1, pointsForLine * 1},
            {2, pointsForLine * 2},
            {3, pointsForLine * 4},
            {4, pointsForLine * 6}
        };

        public int Score { get; private set; }
        public int TotalClearLines { get; private set; }
        public LevelsSettings CurrentLevel { get; private set; }

        public void Reset()
        {
            Score = 0;
            TotalClearLines = 0;
            CurrentLevel = LevelProgression.GetLevelSettingsByClearLines(TotalClearLines);
        }

        public LevelsSettings RegisterClearedLines(int numberCleanLines)
        {
            TotalClearLines += numberCleanLines;
            Score += pointsForLinesConfiguration[numberCleanLines];
            CurrentLevel = LevelProgression.GetLevelSettingsByClearLines(TotalClearLines);
            return CurrentLevel;
        }

        public StatisticParams ToStatisticParams()
        {
            return new StatisticParams(Score, TotalClearLines, CurrentLevel.level);
        }
    }
}
