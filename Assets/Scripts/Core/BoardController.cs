//using static UnityEngine.UIElements.UxmlAttributeDescription;
using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardController : MonoBehaviour, IMovable
    {
        public GameObject prefabForDropDetail;
        public GameObject[] dropDetails = new GameObject[4];

        public bool isCreateDetail;
        public event Action CreateDetailEvent;
        [SerializeField] private GameObject activeDetail;
        private Vector2 targetPosition;
        public Vector2 TargetPosition
        {
            get { return targetPosition; }
            set
            {
                if(activeDetailMoveCoroutine != null)
                {
                    StopCoroutine(activeDetailMoveCoroutine);
                }
                targetPosition = value;
                activeDetailMoveCoroutine = StartCoroutine(SmoothMove(smoothMoveDuration));
            }
        }

        bool[] lastActiveDetailRotation;

        bool isHardDropping = false;

        private float smoothMoveDuration; // Duration of the smooth movement in seconds

        private const int boardWidth = 10;
        private const int boardHeight = 20;
        private const int boardSize = boardWidth * boardHeight; //200
        public GameObject[] boardPositions = new GameObject[boardSize];

        float timeUntillNextStep;
        [SerializeField] private float stepPeriod;
        private readonly MatchProgress matchProgress = new MatchProgress();

        [SerializeField] private CanvasController canvasController;

        private Coroutine activeDetailMoveCoroutine;

        private bool _isPause = false;
        public bool IsPause 
        {
            get { return _isPause; }
            set 
            { 
                if (_isPause != value)
                {
                    _isPause = value;
                    if (value)
                        PauseGame();
                    else
                        UnPauseGame();
                }
            }
        }

        public void PauseManager()
        {
            IsPause = !IsPause;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            int dropDetailsLength = dropDetails.Length;
            for (int i = 0; i < dropDetailsLength; i++) // rewrite: move to start() and dont Destroy in RestartGame
            {
                dropDetails[i] = Instantiate(prefabForDropDetail, new Vector3(100, 100, 0.05f), transform.rotation, this.transform);
            }

            SetupGame();

            var touchInputController = GetComponent<TouchInputController>();
            touchInputController.initialization(this, Camera.main, new Plane(Vector3.forward, Vector3.zero));

            var keyboardInputController = GetComponent<KeyboardInputController>();
            keyboardInputController.initialization(this);
        }

        private void SetupGame()
        {
            canvasController.SetStartGameCanvas();
            activeDetail = null;
            targetPosition = Vector2.zero;
            matchProgress.Reset();

            var levelSettings = matchProgress.CurrentLevel;
            stepPeriod = levelSettings.stepPeriod;
            smoothMoveDuration = stepPeriod / 5;
            timeUntillNextStep = stepPeriod;
            canvasController.SetStatisticParams(matchProgress.ToStatisticParams());

            IsPause = false;
            isCreateDetail = true;
        }

        public void RestartGame()
        {
            Destroy(activeDetail);

            for (int i = 0; i < boardPositions.Length; i++)
            {
                if(boardPositions[i] != null)
                {
                    Destroy(boardPositions[i]);
                    boardPositions[i] = null;
                }
            }

            SetupGame();
        }

        private void FinishMatch()
        {
            StopAllCoroutines();
            IsPause = true;

            canvasController.SetResoultGameCanvas(matchProgress.ToStatisticParams());
        }

        private void PauseGame()
        {
            StopAllCoroutines();
            canvasController.SetPauseGameCanvas();
        }

        private void UnPauseGame()
        {
            StartCoroutine(SmoothMove(smoothMoveDuration));
            canvasController.SetStartGameCanvas();
        }

        // Update is called once per frame
        void Update()
        {
            if (isCreateDetail)
            {
                CreateNewDetail();
            }

            if (!IsPause)
            {
                MoveDetailController();
            }
        }

        public void Move(MoveDirection direction)
        {
            if (IsPause)
                return;

            switch (direction)
            {
                case MoveDirection.Left:
                    MoveDetailLeft();
                    break;
                case MoveDirection.Right:
                    MoveDetailRight();
                    break;
                case MoveDirection.Down:
                    HardDropDetail();
                    break;
                case MoveDirection.TurnRight:
                    TurnDetailRight();
                    break;
                case MoveDirection.TurnLeft:
                    TurnDetailLeft();
                    break;
            }

            GhostPieceDetail();
        }

        private void MoveDetailController()
        {
            if (activeDetail == null)
            {  return; }
            
            if (timeUntillNextStep <= 0)
            {
                Vector2 newDetailPosition = targetPosition + Vector2.down;
                if (СheckNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
                {
                    TargetPosition = newDetailPosition;
                    timeUntillNextStep = stepPeriod;
                }
                else
                {
                    var currentDetailPosition = targetPosition;
                    var detailChildCount = activeDetail.transform.childCount;
                    for (int i = 0; i < detailChildCount; i++)
                    {
                        var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
                        int row = ((int)currentDetailPosition.y + (int)childLocalPossition.y);
                        if (row > 0)
                        {
                            FinishMatch();
                            Debug.Log("Game over");
                            //break;
                            return; // when drop detail, it dont chow, need to use break;  but dont call СhangeDetailParent for correct cleaning 
                        }
                    }
                    if(activeDetailMoveCoroutine != null)
                    {
                        StopCoroutine(activeDetailMoveCoroutine);
                        activeDetailMoveCoroutine = null;
                    }

                    UpdateBoardPositionsForNewDetail();
                    СhangeDetailParent();
                    СheckLines();
                    isCreateDetail = true;

                    return;
                }
            }

            timeUntillNextStep -= Time.deltaTime;
            GhostPieceDetail();
        }

        void MoveDetailRight()
        {
            if (isHardDropping) return;

            Vector2 newDetailPosition = targetPosition + Vector2.right;
            if (СheckNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
            {
                TargetPosition = newDetailPosition;
            }
        }

        void MoveDetailLeft()
        {
            if (isHardDropping) return;

            Vector2 newDetailPosition = targetPosition + Vector2.left;
            if (СheckNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
            {
                TargetPosition = newDetailPosition;
            }
        }

        void TurnDetailRight()
        {
            if (isHardDropping) return;

            var newDetailForm = activeDetail.GetComponent<MoveDetail>().getPossitionForTurnRight();
            TurnDetail(newDetailForm);
        }

        void TurnDetailLeft()
        {
            if (isHardDropping) return;

            var newDetailForm = activeDetail.GetComponent<MoveDetail>().getPossitionForTurnLeft();
            TurnDetail(newDetailForm);
        }

        void HardDropDetail()
        {
            if (isHardDropping) return;

            int countEmptyRows = GetCountEmptyRowsUnderDetail();
            var newDetailPosition = targetPosition;
            newDetailPosition.y = newDetailPosition.y - countEmptyRows;
            timeUntillNextStep = smoothMoveDuration;

            if (СheckNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
            {
                TargetPosition = newDetailPosition;
                isHardDropping = true;
            }
        }

        Vector2[] offsets = {
            Vector2.zero,       // First, check the current position
            Vector2.right,      // Shift to the right
            Vector2.left,       // Shift to the left
            Vector2.right * 2,  // Double shift to the right
            Vector2.left * 2    // Double shift to the left
        };

        private void TurnDetail(bool[] newDetailForm)
        {
            foreach (var offset in offsets)
            {
                Vector2 potentialPosition = targetPosition + offset;
                if (СheckNewDetailPosition(potentialPosition, newDetailForm))
                {
                    // If we actually shifted, update the position
                    if (offset != Vector2.zero)
                    {
                        ChangeDetailPosition(potentialPosition);
                        Debug.Log($"Detail kicked to the {(offset.x > 0 ? "right" : "left")} for spin");
                    }

                    // If the check passes, apply the changes
                    ChangeDetailRotation(newDetailForm);
                    return; // Rotation successful, exit the function
                }
            }
            // If we reached this point, no valid position was found — rotation is impossible
        }

        private void ChangeDetailRotation(bool[] newDetailForm)
        {
            lastActiveDetailRotation = newDetailForm;
            activeDetail.GetComponent<MoveDetail>().turnDetail(newDetailForm);
        }
        private void ChangeDetailPosition(Vector2 newDetailPosition)
        {
            TargetPosition = newDetailPosition;
        }

        int GetCountEmptyRowsUnderDetail()
        {
            var currentNewDetailPosition = TargetPosition;
            var detailChildCount = activeDetail.transform.childCount;
            int countEmptyRows = boardHeight;
            int newCountEmptyRows = 0;

            for (int i = 0; i < detailChildCount; i++)
            {
                newCountEmptyRows = 0;
                var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
                int col = (int)currentNewDetailPosition.x + (int)childLocalPossition.x;
                int row = ((int)currentNewDetailPosition.y + (int)childLocalPossition.y) * -1;

                for (int j = row +1; j <= boardHeight + 1; j++)
                {
                    int currentPosition = col + j * boardWidth;
                    if (currentPosition >= boardSize || (currentPosition >= 0 && boardPositions[currentPosition] != null))
                    {
                        if (newCountEmptyRows < countEmptyRows)
                            countEmptyRows = newCountEmptyRows;
                        break;
                    }
                    newCountEmptyRows++;
                }
            }
            return countEmptyRows;
        }

        void GhostPieceDetail()
        {
            var currentNewDetailPosition = targetPosition;
            var detailChildCount = activeDetail.transform.childCount;
            int countEmptyRows = GetCountEmptyRowsUnderDetail();

            for (int i = 0; i < detailChildCount; i++)
            {
                var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
                int col = (int)currentNewDetailPosition.x + (int)childLocalPossition.x;
                int row = (int)currentNewDetailPosition.y + (int)childLocalPossition.y - countEmptyRows;
                dropDetails[i].transform.localPosition = new Vector3(col, row, 0.05f);
            }
        }

        void СhangeDetailParent()
        {
            activeDetail.transform.localPosition = targetPosition;
            var newDetailComponent = activeDetail.GetComponent<Detail>();
            var childObjects = newDetailComponent.ChildObjects;
            var childCount = childObjects.Length;

            for (int i = 0; i < childCount; i++)
            {
                childObjects[i].transform.SetParent(transform);
            }
            Destroy(activeDetail);
        }

        void СheckLines()
        {
            int numberCleanLines = 0;
            int firstFullLine = 0;
            for (int i = boardHeight - 1; i >= 0; i--)
            {
                bool isFullLine = true;

                for (int j = 0; j < boardWidth; j++)
                {
                    if (boardPositions[i * boardWidth + j] == null)
                    {
                        isFullLine = false;
                        break;
                    }
                }

                if (isFullLine)
                {
                    if (numberCleanLines == 0)
                        firstFullLine = i;
                    numberCleanLines++;
                }
            }
            if (numberCleanLines > 0)
            {
                StartCoroutine(СleanLine(firstFullLine, numberCleanLines));

                var levelSettings = matchProgress.RegisterClearedLines(numberCleanLines);
                if(levelSettings.stepPeriod != stepPeriod)
                {
                    stepPeriod = levelSettings.stepPeriod;
                    smoothMoveDuration = stepPeriod / 4;
                }

                canvasController.SetStatisticParams(matchProgress.ToStatisticParams());
            }
        }

        IEnumerator СleanLine(int firstFullLine, int numberCleanLines)
        {
            YieldInstruction[] destroyBlockAnumations = new YieldInstruction[numberCleanLines * boardWidth];
            for (int j = 0; j < boardWidth; j++)
            {
                for (int i = 0; i < numberCleanLines; i++)
                {
                    destroyBlockAnumations[i * boardWidth + j] = boardPositions[(firstFullLine - i) * boardWidth + j].GetComponent<Block>().DestroyBlock();
                    boardPositions[(firstFullLine - i) * boardWidth + j] = null;
                }
                yield return new WaitForSeconds(0.05f); // Add a small delay before cleaning the lines
            }
            foreach(YieldInstruction instruction in destroyBlockAnumations)
                yield return instruction;

            MoveLinesAfterСlean(firstFullLine, numberCleanLines);
        }

        private void MoveLinesAfterСlean(int firstFullLine, int numberCleanLines)
        {
            for (int k = firstFullLine - numberCleanLines; k >= 0; k--)
            {
                bool isAnyDetailInLine = false;

                for (int j = 0; j < boardWidth; j++)
                {
                    var oldArrayPosition = k * boardWidth + j;
                    if (boardPositions[oldArrayPosition] != null)
                    {
                        isAnyDetailInLine = true;
                        var newArrayPossition = (k + numberCleanLines) * boardWidth + j;
                        boardPositions[newArrayPossition] = boardPositions[oldArrayPosition];
                        boardPositions[oldArrayPosition] = null;

                        var localDetailPosition = boardPositions[newArrayPossition].transform.localPosition;
                        localDetailPosition.y = localDetailPosition.y - numberCleanLines;
                        StartCoroutine(SmoothBlockMove(boardPositions[newArrayPossition].transform, localDetailPosition, smoothMoveDuration));
                    }
                }

                if (!isAnyDetailInLine)
                {
                    break;
                }
            }
        }

        void CreateNewDetail()
        {
            var createDetailManager = GetComponent<CreateDetailManager>();
            activeDetail = createDetailManager.returnNextDetailAndCreateNew();
            activeDetail.transform.SetParent(transform);

            int lowestDetailPoint = 0;
            var detailChildCount = activeDetail.transform.childCount;
            for (int i = 0; i < detailChildCount; i++)
            {
                var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
                if ((int)childLocalPossition.y < lowestDetailPoint)
                {
                    lowestDetailPoint = (int)childLocalPossition.y;
                }
            }

            activeDetail.transform.localPosition = new Vector2(boardWidth/2, (lowestDetailPoint * -1) + 1);
            timeUntillNextStep = stepPeriod;

            var newDetailComponent = activeDetail.GetComponent<Detail>();
            bool[] currentNewGameObjectPositions = newDetailComponent.GameObjectPositions;
            var currentNewDetailPosition = activeDetail.transform.localPosition;

            targetPosition = currentNewDetailPosition;
            lastActiveDetailRotation = currentNewGameObjectPositions;

            isHardDropping = false;
            isCreateDetail = false;
            GhostPieceDetail();
            CreateDetailEvent?.Invoke();
        }

        bool СheckNewDetailPosition(Vector2 detailPossition, bool[] detail)
        {
            var localBoardPositions = boardPositions;

            for (int i = 0; i < 16; i++)
            {
                if (detail[i])
                {
                    int localRow = i / 4;
                    int localCol = i % 4;

                    int col = (int)detailPossition.x + localCol;
                    int row = ((int)detailPossition.y * -1 + localRow);

                    if (col < 0 || col >= boardWidth)
                    {
                        Debug.Log("The new position is outside the board or the position is already occupied. col: " + col);
                        return false;
                    }

                    if (row < 0) // block is under the board, we don't need to check it because we will check it when it will be on the board
                        continue;

                    int newArrayPosition = col + (row * boardWidth);
                    // the new position is outside the board or the position is already occupied
                    if (newArrayPosition >= boardSize || localBoardPositions[newArrayPosition] != null)
                    {
                        Debug.Log("The new position is outside the board or the position is already occupied. Possition: " + newArrayPosition);
                        return false;
                    }
                }
            }

            return true;
        }

        void UpdateBoardPositionsForNewDetail()
        {
            var currentDetailPosition = targetPosition;
            var detailChildCount = activeDetail.transform.childCount;
            for (int i = 0; i < detailChildCount; i++)
            {
                var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
                int col = (int)currentDetailPosition.x + (int)childLocalPossition.x;
                int row = ((int)currentDetailPosition.y + (int)childLocalPossition.y) * -1;
                if (row < 0) // block is under the board, we don't need to add it to the board positions because we will add it when it will be on the board
                    continue;

                boardPositions[col + row * boardWidth] = activeDetail.transform.GetChild(i).gameObject; // check (col + row * boardWidth) before set
            }
        }

        IEnumerator SmoothMove(float duration)
        {
            if (!activeDetail) yield break;

            float elapsedTime = 0f;
            Vector2 startPosition = activeDetail.transform.position;

            while (elapsedTime < duration)
            {
                if (activeDetail)
                {
                    activeDetail.transform.localPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                }
                yield return null;
            }

            if (activeDetail)
            {
                activeDetail.transform.localPosition = targetPosition;
                activeDetailMoveCoroutine = null;
            }
        }

        IEnumerator SmoothBlockMove(Transform blockTransform, Vector2 targetPosition, float duration)
        {
            float elapsedTime = 0f;
            Vector2 startPosition = blockTransform.position;

            while (elapsedTime < duration)
            {
                blockTransform.localPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            blockTransform.localPosition = targetPosition;
        }
    }
}