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
        // clearLines is the inclusive lower bound of a level.
        // Entries must stay ascending and the first one must stay at 0, so the lookup never falls through to default.
        static LevelsSettings[] pointsForLinesConfiguration  = new LevelsSettings[18] {
            new LevelsSettings { clearLines = 0, stepPeriod = 1.2f, level = 1 },
            new LevelsSettings { clearLines = 5, stepPeriod = 0.528f, level = 2 },
            new LevelsSettings { clearLines = 10, stepPeriod = 0.528f, level = 3 },
            new LevelsSettings { clearLines = 15, stepPeriod = 0.528f, level = 4 },
            new LevelsSettings { clearLines = 20, stepPeriod = 0.528f, level = 5 },
            new LevelsSettings { clearLines = 25, stepPeriod = 0.264f, level = 6 },
            new LevelsSettings { clearLines = 30, stepPeriod = 0.264f, level = 7 },
            new LevelsSettings { clearLines = 35, stepPeriod = 0.264f, level = 8 },
            new LevelsSettings { clearLines = 40, stepPeriod = 0.264f, level = 9 },
            new LevelsSettings { clearLines = 45, stepPeriod = 0.132f, level = 10 },
            new LevelsSettings { clearLines = 50, stepPeriod = 0.066f, level = 11 },
            new LevelsSettings { clearLines = 55, stepPeriod = 0.066f, level = 12 },
            new LevelsSettings { clearLines = 60, stepPeriod = 0.066f, level = 13 },
            new LevelsSettings { clearLines = 65, stepPeriod = 0.033f, level = 14 },
            new LevelsSettings { clearLines = 70, stepPeriod = 0.033f, level = 15 },
            new LevelsSettings { clearLines = 75, stepPeriod = 0.016f, level = 16 },
            new LevelsSettings { clearLines = 80, stepPeriod = 0.016f, level = 17 },
            new LevelsSettings { clearLines = 85, stepPeriod = 0.016f, level = 18 }
        };

        public static LevelsSettings GetLevelSettingsByClearLines(int totalNumberClearLines)
        {
            return pointsForLinesConfiguration.LastOrDefault(v => v.clearLines <= totalNumberClearLines);
        }
    }
}
