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

        public void NextBoardSkin() => Next(_boardPrefab, _boardSkinsPrefabs, ref _curentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Right);
        public void PreviousBoardSkin() => Next(_boardPrefab, _boardSkinsPrefabs, ref _curentBoardIndex, ref _currentBoardSkinIndex, _boardConfig, MoveDirection.Left);
        public void NextBlockSkin() => Next(_detailPrefab, _detailSkinsPrefabs, ref _curentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Right);
        public void PreviousBlockSkin() => Next(_detailPrefab, _detailSkinsPrefabs, ref _curentDetailIndex, ref _currentDetailSkinIndex, _blockConfig, MoveDirection.Left);


        private void Next(GameObject[] prefabsArray, Skins[] skinsPrefabsArray, ref int curentPrefabIndex, ref int curentSkinIndex, AnimationConfig animationConfig, MoveDirection moveDirection)
        {
            prefabsArray[curentPrefabIndex].transform.DOLocalMoveX(-(int)moveDirection * animationConfig.Offset, animationConfig.Duration).SetEase(Ease.InSine);  // SetActive(false);

            curentSkinIndex = (curentSkinIndex + (int)moveDirection + skinsPrefabsArray.Length) % skinsPrefabsArray.Length;
            curentPrefabIndex = (curentPrefabIndex + (int)moveDirection + prefabsArray.Length) % prefabsArray.Length;
            prefabsArray[curentPrefabIndex].GetComponent<Renderer>().material = skinsPrefabsArray[curentSkinIndex].Material;
            prefabsArray[curentPrefabIndex].SetActive(true);
            prefabsArray[curentPrefabIndex].transform.DOLocalMoveX(0, animationConfig.Duration).SetEase(Ease.OutSine).From((int)moveDirection * animationConfig.Offset);
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

    