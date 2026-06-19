//using static UnityEngine.UIElements.UxmlAttributeDescription;
using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI.Table;

public struct LevelsSettings
{
    public int clearLines;
    public float stepPeriod;
    public int level;
}

public static class LevelManager
{
    static LevelsSettings[] pointsForLinesConfigurationn  = new LevelsSettings[18] {
        new LevelsSettings { clearLines = 5, stepPeriod = 1.2f, level = 1 },
        new LevelsSettings { clearLines = 10, stepPeriod = 0.528f, level = 2 },
        new LevelsSettings { clearLines = 15, stepPeriod = 0.528f, level = 3 },
        new LevelsSettings { clearLines = 20, stepPeriod = 0.528f, level = 4 },
        new LevelsSettings { clearLines = 25, stepPeriod = 0.528f, level = 5 },
        new LevelsSettings { clearLines = 30, stepPeriod = 0.264f, level = 6 },
        new LevelsSettings { clearLines = 35, stepPeriod = 0.264f, level = 7 },
        new LevelsSettings { clearLines = 40, stepPeriod = 0.264f, level = 8 },
        new LevelsSettings { clearLines = 45, stepPeriod = 0.264f, level = 9 },
        new LevelsSettings { clearLines = 50, stepPeriod = 0.132f, level = 10 },
        new LevelsSettings { clearLines = 55, stepPeriod = 0.066f, level = 11 },
        new LevelsSettings { clearLines = 60, stepPeriod = 0.066f, level = 12 },
        new LevelsSettings { clearLines = 65, stepPeriod = 0.066f, level = 13 },
        new LevelsSettings { clearLines = 70, stepPeriod = 0.033f, level = 14 },
        new LevelsSettings { clearLines = 75, stepPeriod = 0.033f, level = 15 },
        new LevelsSettings { clearLines = 80, stepPeriod = 0.016f, level = 16 },
        new LevelsSettings { clearLines = 85, stepPeriod = 0.016f, level = 17 },
        new LevelsSettings { clearLines = 90, stepPeriod = 0.016f, level = 18 }
    };

    public static LevelsSettings GetLevelSettingsByClearLines(int totalNumberClearLines)
    {
        return pointsForLinesConfigurationn.FirstOrDefault(v => v.clearLines > totalNumberClearLines);
    }
}


public class BoardController : MonoBehaviour
{
    //public Material materialEmptyPossition; // for debuging empty positions
    //public Material materialFullPossition; // for debuging full positions
    //public GameObject prefabForDebug; // detal for debuging
    public GameObject prefabForDropDetail;
    public GameObject[] dropDetails = new GameObject[4];
    //public GameObject[] boardDebugingDetails = new GameObject[boardWidth * boardHeight]; // for debuging 200

    public bool isCreateDetail;
    public event Action CreateDetailEvent;
    [SerializeField] private GameObject activeDetail;
    Vector2 lastActiveDetailPosition;
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
    [SerializeField] private int score;
    private const int pointsForLine = 200;
    [SerializeField] private int totalNumberCleanLines = 0;

    [SerializeField] private CanvasController canvasController;

    private Coroutine activeDetailMoveCoroutine;

    [SerializeField] private Vector2 touchPossition;
    private Camera mainCamera;
    private Plane tablePlane;
    private bool isTouchMove = false;

    private readonly IReadOnlyDictionary<int, int> pointsForLinesConfiguration = new Dictionary<int, int>
    {
        {1, pointsForLine * 1},
        {2, pointsForLine * 2},
        {3, pointsForLine * 4},
        {4, pointsForLine * 6}
    };

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


        mainCamera = Camera.main;
        tablePlane = new Plane(Vector3.forward, Vector3.zero);
    }

    private void SetupGame()
    {
        canvasController.SetStartGameCanvas();
        activeDetail = null;
        lastActiveDetailPosition = Vector2.zero;
        targetPosition = Vector2.zero;
        score = 0;
        totalNumberCleanLines = 0;

        var levelSettings = LevelManager.GetLevelSettingsByClearLines(totalNumberCleanLines);
        stepPeriod = levelSettings.stepPeriod;
        smoothMoveDuration = stepPeriod / 5;
        timeUntillNextStep = stepPeriod;

        canvasController.SetStatisticParams(new StatisticParams(score, totalNumberCleanLines, levelSettings.level));

        //StartCoroutine(SmoothMove(smoothMoveDuration));
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
        //reDrawDebugingObjects(); // for debuging
        IsPause = true;

        var levelSettings = LevelManager.GetLevelSettingsByClearLines(totalNumberCleanLines);
        canvasController.SetResoultGameCanvas((new StatisticParams(score, totalNumberCleanLines, levelSettings.level)));
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
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                //touchPossition = touch.position.ReadValue();
                //Vector2 startPosition = touch.position.ReadValue();
                //Debug.Log($"[НАЖАТИЕ] Палец коснулся экрана в точке: {startPosition}");



                Vector2 screenPos = touch.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(screenPos);

                float enter; // Сюда запишется дистанция до плоскости

                // Проверяем, пересекает ли луч нашу математическую плоскость стола
                if (tablePlane.Raycast(ray, out enter))
                {
                    // Получаем точную Vector3 точку пересечения в мире Unity
                    touchPossition = ray.GetPoint(enter);

                    // Двигаем ваш объект в эту точку
                    //detTest.transform.position = worldPos;
                    isTouchMove = false;
                }
            }


            // Проверяем, зажата ли точка касания в данный момент
            if (touch.press.isPressed)
            {
                Vector2 screenPos = touch.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(screenPos);

                float enter; // Сюда запишется дистанция до плоскости

                // Проверяем, пересекает ли луч нашу математическую плоскость стола
                if (tablePlane.Raycast(ray, out enter))
                {
                    /*
                    // Получаем точную Vector3 точку пересечения в мире Unity
                    Vector3 worldPos = ray.GetPoint(enter);

                    // Двигаем ваш объект в эту точку
                    detTest.transform.position = worldPos;
                    */

                    var currentPosition = ray.GetPoint(enter);
                    var deltaX = currentPosition.x - touchPossition.x;
                    if (deltaX > 1)
                    {
                        PressKeyD();
                        GhostPieceDetail();
                        touchPossition.x = currentPosition.x;
                        isTouchMove = true;
                    }
                    else if (deltaX < -1)
                    {
                        PressKeyA();
                        GhostPieceDetail();
                        touchPossition.x = currentPosition.x;
                        isTouchMove = true;
                    }
                    //else if (currentPosition.y - touchPossition.y > -3)
                    //{
                    //    PressKeySpace();
                    //    GhostPieceDetail();
                    //    isTouchMove = true;
                    //}

                }
            }

            if (touch.press.wasReleasedThisFrame)
            {
                Debug.Log($"wasReleasedThisFrame");

                Vector2 screenPos = touch.position.ReadValue();
                Ray ray = mainCamera.ScreenPointToRay(screenPos);

                float enter; // Сюда запишется дистанция до плоскости

                // Проверяем, пересекает ли луч нашу математическую плоскость стола
                if (tablePlane.Raycast(ray, out enter))
                {
                    var currentPosition = ray.GetPoint(enter);
                    if (currentPosition.y - touchPossition.y < -3)
                    {
                        PressKeySpace();
                        GhostPieceDetail();
                    } 
                    else if (!isTouchMove)
                    {
                        PressKeyW();
                        GhostPieceDetail();
                    }
                }

                touchPossition = Vector2.zero;
            }
        }
        else
        {
            //Debug.Log($"тачпад не активен");
        }



        if (isCreateDetail)
        {
            CreateNewDetail();
        }

        if (!IsPause)
        {
            moveDetailController();
        }
    }

    private void moveDetailController()
    {
        if (activeDetail == null)
        {  return; }
        
        if (timeUntillNextStep <= 0)
        {
            Vector2 newDetailPosition = targetPosition + Vector2.down;
            if (checkNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
            {
                TargetPosition = newDetailPosition;
                timeUntillNextStep = stepPeriod;
            }
            else
            {
                // here we need to check if we need to delete some lines and then create new detail


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
                        return; // when drop detail, it dont chow, need to use break;  but dont call changeDetailParent for correct cleaning 
                    }
                }
                if(activeDetailMoveCoroutine != null)
                {
                    StopCoroutine(activeDetailMoveCoroutine);
                    activeDetailMoveCoroutine = null;
                }

                updateBoardPositionsForNewDetail();
                changeDetailParent();
                checkLines();
                isCreateDetail = true;

                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PressKeyD();
            //GhostPieceDetail();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            PressKeyA();
            //GhostPieceDetail();
        }

        if (Input.GetKeyDown(KeyCode.W)) // соединить W и S в один метод для поворота, а не дублировать код
        {
            PressKeyW();
            //GhostPieceDetail();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            PressKeyS();
            //GhostPieceDetail();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PressKeySpace();
            //GhostPieceDetail();
        }

        timeUntillNextStep -= Time.deltaTime;
        GhostPieceDetail();
    }

    // move detail to the right
    void PressKeyD()
    {
        if (isHardDropping) return;

        Vector2 newDetailPosition = targetPosition + Vector2.right;
        if (checkNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
        {
            TargetPosition = newDetailPosition;
        }
    }

    // move detail to the left
    void PressKeyA()
    {
        if (isHardDropping) return;

        Vector2 newDetailPosition = targetPosition + Vector2.left;
        if (checkNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
        {
            TargetPosition = newDetailPosition;
        }
    }

    // turn detail to the right
    void PressKeyW()
    {
        if (isHardDropping) return;

        var newDetailForm = activeDetail.GetComponent<moveDetail>().getPossitionForTurnRight();
        TurnDetail(newDetailForm);
    }

    // turn detail to the right
    void PressKeyS()
    {
        if (isHardDropping) return;

        var newDetailForm = activeDetail.GetComponent<moveDetail>().getPossitionForTurnLeft();
        TurnDetail(newDetailForm);
    }

    // Hard drop detail
    void PressKeySpace()
    {
        if (isHardDropping) return;

        int countEmptyRows = getCountEmptyRowsUnderDetail();
        var newDetailPosition = targetPosition;
        newDetailPosition.y = newDetailPosition.y - countEmptyRows;
        timeUntillNextStep = smoothMoveDuration;

        if (checkNewDetailPosition(newDetailPosition, lastActiveDetailRotation))
        {
            TargetPosition = newDetailPosition;
            isHardDropping = true;
            //ChangeDetailPosition(newDetailPosition);
        }
    }

    //private void TurnDetail(TurnResult[] arr)
    //{
    //    var f = arr.FirstOrDefault(v => v.NewPosition > 5);
    //    var f1 = arr.Select(v => new Turn(v.IsSuccess, v.NewPosition)).ToList();
    //    if (f != null)
    //    {
    //    }
    //    //var f2 = arr.Contains(v => v.id == 7);
    //    var f5 = new int[5];
    //    var f6 = f5.Contains(8);

    //    var groupedUsers = arr.GroupBy(u => u.NewPosition);
    //    groupedUsers[3].ToList().ForEach(v => new Turn(v.IsSuccess, v.NewPosition));
    //}

    //private class TurnResult
    //{
    //    public int id;
    //    public bool IsSuccess;
    //    public int NewPosition;
    //}

    //private class Turn
    //{
    //    public Turn (bool IsSuccess, int NewPosition)
    //    {
    //            this.IsSuccess = IsSuccess;
    //            this.NewPosition = NewPosition;
    //    }


    //    public bool IsSuccess;
    //    public int NewPosition;
    //}

    //private void TurnDetail(bool[] newDetailForm)
    //{
    //    //Position after wall kick applied during rotation
    //    Vector2 kickedPosition;

    //    var isCheck = checkNewDetailPosition(lastActiveDetailPosition, newDetailForm);
    //    if (isCheck)
    //    {
    //        ChangeDetailPosition(newDetailForm);
    //    }
    //    else if (checkNewDetailPosition(kickedPosition = lastActiveDetailPosition + new Vector2(1, 0), newDetailForm))
    //    {
    //        ChangeDetailPosition(newDetailForm);
    //        ChangeDetailPosition(kickedPosition);
    //        Debug.Log("move possition to the rigth for spin");
    //    }
    //    else if (checkNewDetailPosition(kickedPosition = lastActiveDetailPosition + new Vector2(-1, 0), newDetailForm))
    //    {
    //        ChangeDetailPosition(newDetailForm);
    //        ChangeDetailPosition(kickedPosition);
    //        Debug.Log("move possition to the left for spin");
    //    }
    //}

    Vector2[] offsets = {
        Vector2.zero,           // First, check the current position
        Vector2.right,   // Shift to the right
        Vector2.left,  // Shift to the left
        Vector2.right * 2,   // Double shift to the right
        Vector2.left * 2   // Double shift to the left
    };

    private void TurnDetail(bool[] newDetailForm)
    {
        foreach (var offset in offsets)
        {
            Vector2 potentialPosition = targetPosition + offset;
            if (checkNewDetailPosition(potentialPosition, newDetailForm))
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
        activeDetail.GetComponent<moveDetail>().turnDetail(newDetailForm);
    }
    private void ChangeDetailPosition(Vector2 newDetailPosition)
    {
        TargetPosition = newDetailPosition;
        //activeDetail.transform.localPosition = newDetailPosition;
    }

    int getCountEmptyRowsUnderDetail()
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
        int countEmptyRows = getCountEmptyRowsUnderDetail();

        for (int i = 0; i < detailChildCount; i++)
        {
            var childLocalPossition = activeDetail.transform.GetChild(i).gameObject.transform.localPosition;
            int col = (int)currentNewDetailPosition.x + (int)childLocalPossition.x;
            int row = (int)currentNewDetailPosition.y + (int)childLocalPossition.y - countEmptyRows;
            dropDetails[i].transform.localPosition = new Vector3(col, row, 0.05f);
        }
    }


    void changeDetailParent()
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

    void checkLines()
    {
        int numberCleanLines = 0;
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
                cleanLine(i);
                i++; // After cleaning the line, we lower details in the board, so we need to check the same line
                numberCleanLines++;
            }
        }

        if(numberCleanLines > 0)
        {
            totalNumberCleanLines += numberCleanLines;
            score += pointsForLinesConfiguration[numberCleanLines];

            var levelSettings = LevelManager.GetLevelSettingsByClearLines(totalNumberCleanLines);
            if(levelSettings.stepPeriod != stepPeriod)
            {
                stepPeriod = levelSettings.stepPeriod;
                smoothMoveDuration = stepPeriod / 4;
            }

            canvasController.SetStatisticParams(new StatisticParams(score, totalNumberCleanLines, levelSettings.level));
        }
    }


    void cleanLine(int cleaningLine)
    {
        for (int j = 0; j < boardWidth; j++)
        {
            Destroy(boardPositions[cleaningLine * boardWidth + j]);
            boardPositions[cleaningLine * boardWidth + j] = null; // correct?
        }

        for (int k = cleaningLine - 1; k >= 0; k--)
        {
            bool isAnyDetailInLine = false;

            for (int j = 0; j < boardWidth; j++)
            {
                if (boardPositions[k * boardWidth + j] != null)
                {
                    isAnyDetailInLine = true;
                    boardPositions[(k + 1) * boardWidth + j] = boardPositions[k * boardWidth + j];
                    boardPositions[k * boardWidth + j] = null;

                    var localDetailPosition = boardPositions[(k + 1) * boardWidth + j].transform.localPosition;
                    localDetailPosition.y = localDetailPosition.y - 1;
                    boardPositions[(k + 1) * boardWidth + j].transform.localPosition = localDetailPosition;
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

        lastActiveDetailPosition = currentNewDetailPosition;
        targetPosition = currentNewDetailPosition;
        lastActiveDetailRotation = currentNewGameObjectPositions;

        isHardDropping = false;
        isCreateDetail = false;
        GhostPieceDetail();
        CreateDetailEvent?.Invoke();
    }

    bool checkNewDetailPosition(Vector2 detailPossition, bool[] detail)
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

    void updateBoardPositionsForNewDetail()
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
            //reDrawDebugingObjects(); // for debuging
        }
    }

    //void reDrawDebugingObjects()
    //{
    //    int boardPositionsLength = boardDebugingDetails.Length;
    //    for (int i = 0; i < boardPositionsLength; i++)
    //    {
    //        if (boardPositions[i] != null)
    //        {
    //            boardDebugingDetails[i].GetComponent<Renderer>().material = materialFullPossition;
    //        }
    //        else
    //        {
    //            boardDebugingDetails[i].GetComponent<Renderer>().material = materialEmptyPossition;
    //        }
    //    }
    //}


    IEnumerator SmoothMove(float duration)
    {
        if (!activeDetail) yield break;

        float elapsedTime = 0f;
        // Запоминаем НАЧАЛЬНУЮ точку один раз перед стартом
        Vector2 startPosition = activeDetail.transform.position;

        // Цикл выполняется, пока не выйдет время
        while (elapsedTime < duration)
        {
            if (activeDetail)
            {
                // Двигаемся от СТАРТОВОЙ точки к ЦЕЛЕВОЙ
                activeDetail.transform.localPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
            }
            yield return null; // Ждем следующий кадр
        }

        // В конце жестко приравниваем к цели, чтобы убрать погрешность
        if (activeDetail)
        {
            activeDetail.transform.localPosition = targetPosition;
            activeDetailMoveCoroutine = null;
        }
    }
}