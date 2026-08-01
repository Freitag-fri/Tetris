using System.Linq;

namespace Assets.Scripts
{
    public struct LevelsSettings
    {
        public int clearLines;
        public float stepPeriod;
        public int level;
    }

    public static class LevelProgression
    {
        static LevelsSettings[] pointsForLinesConfiguration  = new LevelsSettings[11] {
            new LevelsSettings { clearLines = 0, stepPeriod = 1f, level = 1 },
            new LevelsSettings { clearLines = 5, stepPeriod = 0.793f, level = 2 },
            new LevelsSettings { clearLines = 10, stepPeriod = 0.618f, level = 3 },
            new LevelsSettings { clearLines = 15, stepPeriod = 0.473f, level = 4 },
            new LevelsSettings { clearLines = 20, stepPeriod = 0.355f, level = 5 },
            new LevelsSettings { clearLines = 25, stepPeriod = 0.262f, level = 6 },
            new LevelsSettings { clearLines = 30, stepPeriod = 0.19f, level = 7 },
            new LevelsSettings { clearLines = 35, stepPeriod = 0.135f, level = 8 },
            new LevelsSettings { clearLines = 40, stepPeriod = 0.094f, level = 9 },
            new LevelsSettings { clearLines = 45, stepPeriod = 0.064f, level = 10 },
            new LevelsSettings { clearLines = 50, stepPeriod = 0.043f, level = 11 },
        };

        public static LevelsSettings GetLevelSettingsByClearLines(int totalNumberClearLines)
        {
            return pointsForLinesConfiguration.LastOrDefault(v => v.clearLines <= totalNumberClearLines);
        }
    }
}
