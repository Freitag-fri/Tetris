using Assets.Scripts;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IMovable
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera boardCamera;
    [SerializeField] private CinemachineVirtualCamera blockCamera;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject skinsMenu;

    [SerializeField] private GameObject skinLabel;
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private TMP_Text skinCounterText;

    [SerializeField] private Button skinBoardButton;
    [SerializeField] private Button skinBlockButton;
    [SerializeField] private MaterialManager materialManager;

    enum MenuStatus
    {
        MainMenu,
        Settings,
        SkinsMenu,
        BoardSkins,
        BlockSkins
    }

    private MenuStatus currentMenuStatus;
    private TouchInputController touchInputController;

    void Start()
    {
        currentMenuStatus = MenuStatus.MainMenu;
        mainCamera.Priority = 1;
        boardCamera.Priority = 0;
        blockCamera.Priority = 0;
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        skinsMenu.SetActive(false);

        touchInputController = GetComponent<TouchInputController>();
        if (touchInputController == null)
            touchInputController = gameObject.AddComponent<TouchInputController>();

        touchInputController.Initialization(this);
        touchInputController.enabled = false;
    }

    public void Play()
    {
        GameManager.Instance.BoardMaterial = materialManager.GetCurrentBoardSkinInfo().Skin.Material;
        GameManager.Instance.BlockMaterial = materialManager.GetCurrentBlockSkinInfo().Skin.Material;
        SceneManager.LoadScene("GameScene");
    }

    // rename to OpenSettings
    public void Settings()  // need rewrite
    {
        mainCamera.Priority = 0;
        //settingsCamera.Priority = 1;

        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        //StartCoroutine(OpenSettingsAfterTime());
    }

    public void EnterToMainMenu()
    {
        mainCamera.Priority = 1;
        boardCamera.Priority = 0;
        blockCamera.Priority = 0;

        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        skinsMenu.SetActive(false);
        skinLabel.SetActive(false);
        currentMenuStatus = MenuStatus.MainMenu;
        touchInputController.enabled = false;

        //StartCoroutine(CloseSettingAfterTime());
    }

    public void EnterToSkinsMenu()
    {
        skinsMenu.SetActive(true);
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        currentMenuStatus = MenuStatus.SkinsMenu;
        touchInputController.enabled = false;
    }

    //public void ExitFromSkinsMenu()
    //{
    //    skinsMenu.SetActive(false);
    //    menuPanel.SetActive(true);
    //}

    public void EnterToBoardSkins()
    {
        skinBoardButton.transform
            .DOScale(Vector3.one * 1.2f, 0.2f)
            .OnComplete(() =>{ skinBoardButton.transform
                .DOScale(Vector3.one, 0.2f);
            });

        boardCamera.Priority = 3;
        blockCamera.Priority = 0;
        currentMenuStatus = MenuStatus.BoardSkins;
        touchInputController.enabled = true;

        skinLabel.SetActive(true);
        UpdateSkinInfo();
    }

    public void EnterToBlockSkins()
    {
        skinBlockButton.transform
            .DOScale(Vector3.one * 1.2f, 0.2f)
            .OnComplete(() => {
                skinBlockButton.transform
                .DOScale(Vector3.one, 0.2f);
            });

        blockCamera.Priority = 3;
        boardCamera.Priority = 0;
        currentMenuStatus = MenuStatus.BlockSkins;
        touchInputController.enabled = true;

        skinLabel.SetActive(true);
        UpdateSkinInfo();
    }

    private void SensorMoveLeft()
    {
        if (currentMenuStatus == MenuStatus.BoardSkins)
        {
            materialManager.NextBoardSkin();
        }
        else if (currentMenuStatus == MenuStatus.BlockSkins)
        {
            materialManager.NextBlockSkin();
        }
        UpdateSkinInfo();
    }

    private void SensorMoveRight()
    {
        if (currentMenuStatus == MenuStatus.BoardSkins)
        {
            materialManager.PreviousBoardSkin();
        }
        else if (currentMenuStatus == MenuStatus.BlockSkins)
        {
            materialManager.PreviousBlockSkin();
        }
        UpdateSkinInfo();
    }

    private void UpdateSkinInfo()
    {
        SkinInfo skinInfo;
        if (currentMenuStatus == MenuStatus.BoardSkins)
            skinInfo = materialManager.GetCurrentBoardSkinInfo();
        else
            skinInfo = materialManager.GetCurrentBlockSkinInfo();

        skinNameText.SetText(skinInfo.Skin.Id);
        skinCounterText.SetText("{0}/{1}", skinInfo.CurrentSkinIndex + 1, skinInfo.TotalSkinsCount);
    }

    public void Move(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.TurnLeft:
                SensorMoveLeft();
                break;
            case MoveDirection.TurnRight:
                SensorMoveRight();
                break;
            default:
                Debug.Log($"Move direction {direction} is not supported in MenuManager");
                break;
        }
    }
}