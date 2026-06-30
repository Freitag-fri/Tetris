using Assets.Scripts;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera boardCamera;
    [SerializeField] private CinemachineVirtualCamera blockCamera;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject skinsMenu;

    [SerializeField] private GameObject skinTextObj;

    [SerializeField] private Button skinBoardButton;
    [SerializeField] private Button skinBlockButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    void Start()
    {
        currentMenuStatus = MenuStatus.MainMenu;
        mainCamera.Priority = 1;
        boardCamera.Priority = 0;
        blockCamera.Priority = 0;
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        skinsMenu.SetActive(false);
    }

    void Update()
    {
        SensorNavigationSkinsMenu();
    }


    public void Play()
    {
        GameManager.Instance.BoardMaterial = materialManager.GetCurrentBoardSkin().Material;
        GameManager.Instance.BlockMaterial = materialManager.GetCurrentBlockSkin().Material;
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
        skinTextObj.SetActive(false);
        currentMenuStatus = MenuStatus.MainMenu;

        //StartCoroutine(CloseSettingAfterTime());
    }

    public void EnterToSkinsMenu()
    {
        skinsMenu.SetActive(true);
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        currentMenuStatus = MenuStatus.SkinsMenu;
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

        skinTextObj.SetActive(true);
        skinTextObj.GetComponent<TMP_Text>().SetText(GetCurrentSkinName());
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

        skinTextObj.SetActive(true);
        skinTextObj.GetComponent<TMP_Text>().SetText(GetCurrentSkinName());
    }

    //IEnumerable
    //IEnumerator OpenSettingsAfterTime()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    settingsPanel.SetActive(true);
    //}

    //IEnumerator CloseSettingAfterTime()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    menuPanel.SetActive(true);
    //}

    private Vector2 touchPossition;

    private void SensorNavigationSkinsMenu()
    {
        if(currentMenuStatus != MenuStatus.BoardSkins && currentMenuStatus != MenuStatus.BlockSkins)
        {
            if (touchPossition != Vector2.zero) //rewrite
                touchPossition = Vector2.zero;
            return;
        }

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                touchPossition = touch.position.ReadValue();
            }
            else if (touch.press.wasReleasedThisFrame && touchPossition != Vector2.zero)
            {
                Vector2 releasePosition = touch.position.ReadValue();

                if (releasePosition.x > touchPossition.x + 80)
                {
                    Debug.Log($"Проведите пальцем влево");
                    SensorMoveRight();
                }
                else if (releasePosition.x < touchPossition.x - 80)
                {
                    Debug.Log($"Проведите пальцем вправо");
                    SensorMoveLeft();
                    touchPossition = Vector2.zero;
                }
            }
        }
        else
        {
            //Debug.Log($"тачпад не активен");
        }
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
        skinTextObj.GetComponent<TMP_Text>().SetText(GetCurrentSkinName());
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
        skinTextObj.GetComponent<TMP_Text>().SetText(GetCurrentSkinName());
    }

    private string GetCurrentSkinName()
    {
        if(currentMenuStatus == MenuStatus.BoardSkins)
        {
            return materialManager.GetCurrentBoardSkin().Id;
        }
        else if (currentMenuStatus == MenuStatus.BlockSkins)
        {
            return materialManager.GetCurrentBlockSkin().Id;
        }
        return "no name";
    }
}
