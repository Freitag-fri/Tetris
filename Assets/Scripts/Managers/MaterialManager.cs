using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    public struct SkinInfo
    {
        public SkinInfo(Skins skin, int currentSkinIndex, int totalSkinsCount)
        {
            this.Skin = skin;
            this.CurrentSkinIndex = currentSkinIndex;
            this.TotalSkinsCount = totalSkinsCount;
        }
        public Skins Skin { get; }
        public int CurrentSkinIndex { get; }
        public int TotalSkinsCount { get; }


    }
	public class MaterialManager: MonoBehaviour
	{
        [SerializeField] private GameObject[] _boardPrefab;
        [SerializeField] private GameObject[] _detailPrefab;
        [SerializeField] private Skins[] _boardSkinsPrefabs;
        [SerializeField] private Skins[] _detailSkinsPrefabs;

        private int _currentBoardSkinIndex = 0;
        private int _currentDetailSkinIndex = 0;

        private int _currentBoardIndex = 0;
        private int _currentDetailIndex = 0;

        public enum MoveDirection
        {
            Left = -1,
            Right = 1
        }

        public struct AnimationConfig
        {
            public float Offset;
            public float Duration;

            public AnimationConfig(float offset, float duration)
            {
                Offset = offset;
                Duration = duration;
            }
        }

        private readonly AnimationConfig _boardConfig = new AnimationConfig(40f, 1f);
        private readonly AnimationConfig _blockConfig = new AnimationConfig(8f, 0.9f);

        void Start()
		{
            _currentBoardSkinIndex = SaveData.BoardSkinIndex;
            if (_currentBoardSkinIndex > _boardSkinsPrefabs.Length - 1)
                _currentBoardSkinIndex = _boardSkinsPrefabs.Length - 1;
            _boardPrefab[_currentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
            
            _currentDetailSkinIndex = SaveData.BlockSkinIndex;
            if (_currentDetailSkinIndex > _detailSkinsPrefabs.Length - 1)
                _currentDetailSkinIndex = _detailSkinsPrefabs.Length - 1;
            _detailPrefab[_currentDetailIndex].GetComponent<Renderer>().material = _detailSkinsPrefabs[_currentDetailSkinIndex].Material;
        }

        public void NextBoardSkin() => RotateSkin(_boardPrefab, _boardSkinsPrefabs, ref _currentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Right);
        public void PreviousBoardSkin() => RotateSkin(_boardPrefab, _boardSkinsPrefabs, ref _currentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Left);
        public void NextBlockSkin() => RotateSkin(_detailPrefab, _detailSkinsPrefabs, ref _currentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Right);
        public void PreviousBlockSkin() => RotateSkin(_detailPrefab, _detailSkinsPrefabs, ref _currentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Left);

        public void RollbackBoardSkin()
        {
            _currentBoardSkinIndex = (SaveData.BoardSkinIndex - 1 + _boardSkinsPrefabs.Length) % _boardSkinsPrefabs.Length;
            NextBoardSkin();
        }

        public void RollbackBlockSkin()
        {
            _currentDetailSkinIndex = (SaveData.BlockSkinIndex - 1 + _detailSkinsPrefabs.Length) % _detailSkinsPrefabs.Length;
            NextBlockSkin();
        }

        private void RotateSkin(GameObject[] prefabsArray, Skins[] skinsPrefabsArray, ref int currentPrefabIndex, ref int currentSkinIndex, AnimationConfig animationConfig, MoveDirection moveDirection)
        {
            int directionSign = (int)moveDirection;
            int previousPrefabIndex = currentPrefabIndex;
            prefabsArray[currentPrefabIndex].transform
                .DOLocalMoveX(-directionSign * animationConfig.Offset, animationConfig.Duration)
                .SetEase(Ease.InSine)
                .OnComplete(() => prefabsArray[previousPrefabIndex].SetActive(false));

            currentSkinIndex = (currentSkinIndex + directionSign + skinsPrefabsArray.Length) % skinsPrefabsArray.Length;
            currentPrefabIndex = (currentPrefabIndex + directionSign + prefabsArray.Length) % prefabsArray.Length;

            GameObject newActivePrefab = prefabsArray[currentPrefabIndex];
            newActivePrefab.GetComponent<Renderer>().material = skinsPrefabsArray[currentSkinIndex].Material;
            newActivePrefab.SetActive(true);
            newActivePrefab.transform
                .DOLocalMoveX(0, animationConfig.Duration)
                .SetEase(Ease.OutSine).From(directionSign * animationConfig.Offset)
                .OnStart(() => newActivePrefab.SetActive(true));
        }

        public void SelectCurrentBoardSkin()
        {
            SaveData.BoardSkinIndex = _currentBoardSkinIndex;
        }

        public void SelectCurrentBlockSkin()
        {
            SaveData.BlockSkinIndex = _currentDetailSkinIndex;
        }

        public bool IsCurrentBoardSkinSelected()
        {
            return SaveData.BoardSkinIndex == _currentBoardSkinIndex;
        }

        public bool IsCurrentBlockSkinSelected()
        {
            return SaveData.BlockSkinIndex == _currentDetailSkinIndex;
        }

        public SkinInfo GetCurrentBoardSkinInfo()
        {
            return new SkinInfo
            (
                _boardSkinsPrefabs[_currentBoardSkinIndex],
                _currentBoardSkinIndex,
                _boardSkinsPrefabs.Length
            );
        }

        public SkinInfo GetCurrentBlockSkinInfo()
        {
            return new SkinInfo
            (
                _detailSkinsPrefabs[_currentDetailSkinIndex],
                _currentDetailSkinIndex,
                _detailSkinsPrefabs.Length
            );
        }

        public Material GetSelectedBoardMaterial() => _boardSkinsPrefabs[SaveData.BoardSkinIndex].Material;
        public Material GetSelectedBlockMaterial() => _detailSkinsPrefabs[SaveData.BlockSkinIndex].Material;
    }
}  