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
    [SerializeField] private Button selectSkinButton;

    [SerializeField] private TMP_Text highScoreText;

    [SerializeField] private Button skinBoardButton;
    [SerializeField] private Button skinBlockButton;
    [SerializeField] private MaterialManager materialManager;
    [SerializeField] private SwipeHint swipeHint;

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

        if (SaveData.HighScore > 0)
        {
            highScoreText.SetText("Best: {0}", SaveData.HighScore);
            highScoreText.gameObject.SetActive(true);
        }
        else
        {
            highScoreText.gameObject.SetActive(false);
        }
    }

    public void Play()
    {
        GameManager.Instance.BoardMaterial = materialManager.GetSelectedBoardMaterial();
        GameManager.Instance.BlockMaterial = materialManager.GetSelectedBlockMaterial();
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Roll the current skin preview back to its saved skin if the player left without pressing Select.
    private void RollbackPreviewIfNeeded()
    {
        if(currentMenuStatus != MenuStatus.BoardSkins && currentMenuStatus != MenuStatus.BlockSkins)
            return;

        if ( !materialManager.IsCurrentBoardSkinSelected())
            materialManager.RollbackBoardSkin();
        if (!materialManager.IsCurrentBlockSkinSelected())
            materialManager.RollbackBlockSkin();
    }

    public void EnterToMainMenu()
    {
        RollbackPreviewIfNeeded();
        swipeHint.Hide();

        mainCamera.Priority = 1;
        boardCamera.Priority = 0;
        blockCamera.Priority = 0;

        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        skinsMenu.SetActive(false);
        skinLabel.SetActive(false);
        currentMenuStatus = MenuStatus.MainMenu;
        touchInputController.enabled = false;
    }

    public void EnterToSkinsMenu()
    {
        skinsMenu.SetActive(true);
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        currentMenuStatus = MenuStatus.SkinsMenu;
        touchInputController.enabled = false;
    }

    public void EnterToBoardSkins()
    {
        EnterToSkins(skinBoardButton, boardCamera, blockCamera, MenuStatus.BoardSkins);
    }

    public void EnterToBlockSkins()
    {
        EnterToSkins(skinBlockButton, blockCamera, boardCamera, MenuStatus.BlockSkins);
    }

    private void EnterToSkins(Button button, CinemachineVirtualCamera activeCamera,
                             CinemachineVirtualCamera inactiveCamera, MenuStatus status)
    {
        button.transform
            .DOScale(Vector3.one * 1.2f, 0.2f)
            .OnComplete(() => button.transform.DOScale(Vector3.one, 0.2f));

        activeCamera.Priority = 3;
        inactiveCamera.Priority = 0;
        currentMenuStatus = status;
        touchInputController.enabled = true;

        skinLabel.SetActive(true);
        UpdateSkinInfo();

        swipeHint.Show();
    }

    private void NextSkin()
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

    private void PreviousSkin()
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
        bool isCurrentSkinSelected;
        if (currentMenuStatus == MenuStatus.BoardSkins)
        {
            skinInfo = materialManager.GetCurrentBoardSkinInfo();
            isCurrentSkinSelected = materialManager.IsCurrentBoardSkinSelected();
        }
        else
        {
            skinInfo = materialManager.GetCurrentBlockSkinInfo();
            isCurrentSkinSelected = materialManager.IsCurrentBlockSkinSelected();
        }

        skinNameText.SetText(skinInfo.Skin.Id);
        skinCounterText.SetText("{0}/{1}", skinInfo.CurrentSkinIndex + 1, skinInfo.TotalSkinsCount);
        selectSkinButton.interactable = !isCurrentSkinSelected;
    }

    public void SelectSkin()
    {
        if (currentMenuStatus == MenuStatus.BoardSkins)
        {
            materialManager.SelectCurrentBoardSkin();
        }
        else if (currentMenuStatus == MenuStatus.BlockSkins)
        {
            materialManager.SelectCurrentBlockSkin();
        }

        selectSkinButton.interactable = false;
    }

    public void Move(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.TurnLeft:
                swipeHint.HideAfterSwipe();
                NextSkin();
                break;
            case MoveDirection.TurnRight:
                swipeHint.HideAfterSwipe();
                PreviousSkin();
                break;
            default:
                Debug.Log($"Move direction {direction} is not supported in MenuManager");
                break;
        }
    }
}