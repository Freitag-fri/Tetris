using Assets.Scripts;
using Cinemachine;
using System.Collections;
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
        SceneManager.LoadScene("GameScene");
    }

    // rename to OpenSettings
    public void Settings()  // need rewrite
    {
        mainCamera.Priority = 0;
        //settingsCamera.Priority = 1;

        menuPanel.SetActive(false);
        StartCoroutine(OpenSettingsAfterTime());
    }

    public void EnterToMainMenu()
    {
        mainCamera.Priority = 1;
        boardCamera.Priority = 0;
        blockCamera.Priority = 0;

        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        skinsMenu.SetActive(false);
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
        boardCamera.Priority = 3;
        blockCamera.Priority = 0;
        currentMenuStatus = MenuStatus.BoardSkins;
    }

    public void EnterToBlockSkins()
    {
        blockCamera.Priority = 3;
        boardCamera.Priority = 0;
        currentMenuStatus = MenuStatus.BlockSkins;
    }

    //IEnumerable
    IEnumerator OpenSettingsAfterTime()
    {
        yield return new WaitForSeconds(2.0f);
        settingsPanel.SetActive(true);
    }

    IEnumerator CloseSettingAfterTime()
    {
        yield return new WaitForSeconds(2.0f);
        menuPanel.SetActive(true);
    }

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
    }
}
