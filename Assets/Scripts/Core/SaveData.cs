using UnityEngine;

namespace Assets.Scripts
{
    // Single access point for persistent data. Key strings live here and nowhere else.
    public static class SaveData
    {
        private const string highScoreKey = "HighScore";
        private const string boardSkinKey = "BoardSkin";
        private const string blockSkinKey = "BlockSkin";

        public static int HighScore
        {
            get => PlayerPrefs.GetInt(highScoreKey, 0);
            set
            {
                PlayerPrefs.SetInt(highScoreKey, value);
                PlayerPrefs.Save();
            } 
        }

        public static int BoardSkinIndex
        {
            get => PlayerPrefs.GetInt(boardSkinKey, 0);
            set
            {
                PlayerPrefs.SetInt(boardSkinKey, value);
                PlayerPrefs.Save();
            }
        }

        public static int BlockSkinIndex
        {
            get => PlayerPrefs.GetInt(blockSkinKey, 0);
            set
            {
                PlayerPrefs.SetInt(blockSkinKey, value);
                PlayerPrefs.Save();
            }
        }
    }
}