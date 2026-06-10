using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts
{
	public class MaterialManager: MonoBehaviour
	{
        [SerializeField] private GameObject[] _boardPrefab;
        [SerializeField] private GameObject[] _detailPrefab;
        [SerializeField] private Skins[] _boardSkinsPrefabs;
        [SerializeField] private Skins[] _detailSkinsPrefabs;

        private int _boardSkinCount = 0;
        private int _detailSkinCount = 0;

        private int _currentBoardSkinIndex = 0;
        private int _currentDetailSkinIndex = 0;

        private int _curentBoardIndex = 0;
        private int _curentDetailIndex = 0;

        public bool boardForward = false;
        public bool detailForward = false;
        public bool boardBackward = false;
        public bool detailBackward = false;

        // Use this for initialization
        void Start()
		{
            if(_boardSkinsPrefabs != null && _boardSkinsPrefabs.Length > 0)
            {
                _boardSkinCount = _boardSkinsPrefabs.Length;
                _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[0].Material;
                _currentBoardSkinIndex = 0;
            }
            if (_detailSkinsPrefabs != null && _detailSkinsPrefabs.Length > 0)
            {
                _detailSkinCount = _detailSkinsPrefabs.Length;
                _detailPrefab[_curentDetailIndex].GetComponent<Renderer>().material = _detailSkinsPrefabs[0].Material;
                _currentDetailSkinIndex = 0;
            }

        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    _detailPrefab.transform.DOMoveX(6, 0.7f).SetEase(Ease.InQuad).From(0);
            //}

            if (boardForward)
            {
                boardForward = false;

                _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(-40, 0.9f).SetEase(Ease.InQuad);

                _currentBoardSkinIndex = (_currentBoardSkinIndex + 1) % _boardSkinCount;
                _curentBoardIndex = (_curentBoardIndex + 1) % 2;
                _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
                _boardPrefab[_curentBoardIndex].SetActive(true);
                _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(0, 0.9f).SetEase(Ease.InQuad).From(40);

            }
            else if (boardBackward) {
                boardBackward = false;

                _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(40, 0.9f).SetEase(Ease.InQuad);

                _currentBoardSkinIndex = (_currentBoardSkinIndex - 1 + _boardSkinCount) % _boardSkinCount;
                _curentBoardIndex = (_curentBoardIndex - 1 + 2) % 2;
                _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
                _boardPrefab[_curentBoardIndex].SetActive(true);
                _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(0, 0.9f).SetEase(Ease.InQuad).From(-40);
            }

            //if (boardForward)
            //{
            //    boardForward = false;
            //    _currentBoardSkinIndex = (_currentBoardSkinIndex + 1) % _boardSkinCount;
            //    _boardPrefab.GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
            //}
            //else if (detailForward)
            //{
            //    detailForward = false;
            //    _currentDetailSkinIndex = (_currentDetailSkinIndex + 1) % _detailSkinCount;
            //    _detailPrefab.GetComponent<Renderer>().material = _detailSkinsPrefabs[_currentDetailSkinIndex].Material;
            //}
            //else if (boardBackward)
            //{
            //    boardBackward = false;
            //    _currentBoardSkinIndex = (_currentBoardSkinIndex - 1 + _boardSkinCount) % _boardSkinCount;
            //    _boardPrefab.GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
            //}
            //else if (detailBackward)
            //{
            //    detailBackward = false;
            //    _currentDetailSkinIndex = (_currentDetailSkinIndex - 1 + _detailSkinCount) % _detailSkinCount;
            //    _detailPrefab.GetComponent<Renderer>().material = _detailSkinsPrefabs[_currentDetailSkinIndex].Material;

            //}
        }
        public void NextBoardSkin()
        {
            _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(-40, 1f).SetEase(Ease.InSine);

            _currentBoardSkinIndex = (_currentBoardSkinIndex + 1) % _boardSkinCount;
            _curentBoardIndex = (_curentBoardIndex + 1) % 2;
            _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
            _boardPrefab[_curentBoardIndex].SetActive(true);
            _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(0, 1f).SetEase(Ease.OutSine).From(40);

            //add .SetActive(false);
        }

        public void PreviousBoardSkin()
        {
            _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(40, 1f).SetEase(Ease.InSine);

            _currentBoardSkinIndex = (_currentBoardSkinIndex - 1 + _boardSkinCount) % _boardSkinCount;
            _curentBoardIndex = (_curentBoardIndex - 1 + 2) % 2;
            _boardPrefab[_curentBoardIndex].GetComponent<Renderer>().material = _boardSkinsPrefabs[_currentBoardSkinIndex].Material;
            _boardPrefab[_curentBoardIndex].SetActive(true);
            _boardPrefab[_curentBoardIndex].transform.DOLocalMoveX(0, 1f).SetEase(Ease.OutSine).From(-40);
        }

        public void NextBlockSkin()
        {
            _detailPrefab[_curentDetailIndex].transform.DOLocalMoveX(-8, 0.9f).SetEase(Ease.InSine);
            _currentDetailSkinIndex = (_currentDetailSkinIndex + 1) % _detailSkinCount;
            _curentDetailIndex = (_curentDetailIndex + 1) % 2;
            _detailPrefab[_curentDetailIndex].GetComponent<Renderer>().material = _detailSkinsPrefabs[_currentDetailSkinIndex].Material;
            _detailPrefab[_curentDetailIndex].SetActive(true);
            _detailPrefab[_curentDetailIndex].transform.DOLocalMoveX(0, 0.9f).SetEase(Ease.OutSine).From(8);
        }
        

        public void PreviousBlockSkin()
        {
            _detailPrefab[_curentDetailIndex].transform.DOLocalMoveX(8, 0.9f).SetEase(Ease.InSine);
            _currentDetailSkinIndex = (_currentDetailSkinIndex - 1 + _detailSkinCount) % _detailSkinCount;
            _curentDetailIndex = (_curentDetailIndex - 1 + 2) % 2;
            _detailPrefab[_curentDetailIndex].GetComponent<Renderer>().material = _detailSkinsPrefabs[_currentDetailSkinIndex].Material;
            _detailPrefab[_curentDetailIndex].SetActive(true);
            _detailPrefab[_curentDetailIndex].transform.DOLocalMoveX(0, 0.9f).SetEase(Ease.OutSine).From(-8);
        }


    }
}

    