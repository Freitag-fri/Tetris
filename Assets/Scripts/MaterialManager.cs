using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
	public class MaterialManager: MonoBehaviour
	{
        [SerializeField] private GameObject[] _boardPrefab;
        [SerializeField] private GameObject[] _detailPrefab;
        [SerializeField] private Skins[] _boardSkinsPrefabs;
        [SerializeField] private Skins[] _detailSkinsPrefabs;

        private int _currentBoardSkinIndex = 0;
        private int _currentDetailSkinIndex = 0;

        private int _curentBoardIndex = 0;
        private int _curentDetailIndex = 0;

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


        // Use this for initialization
        void Start()
		{
            if(_boardSkinsPrefabs != null && _boardSkinsPrefabs.Length > 0)
            {
                _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[0].Material;
                _currentBoardSkinIndex = 0;
            }
            if (_detailSkinsPrefabs != null && _detailSkinsPrefabs.Length > 0)
            {
                _detailPrefab[_curentDetailIndex].GetComponent<Renderer>().material = _detailSkinsPrefabs[0].Material;
                _currentDetailSkinIndex = 0;
            }
        }

        public void NextBoardSkin() => RotateSkin(_boardPrefab, _boardSkinsPrefabs, ref _curentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Right);
        public void PreviousBoardSkin() => RotateSkin(_boardPrefab, _boardSkinsPrefabs, ref _curentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Left);
        public void NextBlockSkin() => RotateSkin(_detailPrefab, _detailSkinsPrefabs, ref _curentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Right);
        public void PreviousBlockSkin() => RotateSkin(_detailPrefab, _detailSkinsPrefabs, ref _curentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Left);

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

        public Skins GetCurrentBoardSkin()
        {
            return _boardSkinsPrefabs[_currentBoardSkinIndex];
        }

        public Skins GetCurrentBlockSkin()
        {
            return _detailSkinsPrefabs[_currentDetailSkinIndex];
        }
    }
}  