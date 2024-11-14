using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private PlayerController controller;
    private Player player;

    [Header("Menu Items")]
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject settingsMenu;
    [SerializeField] Button mainMenuDefaultBtn; 
    [SerializeField] Button settingsMenuDefaultBtn;
    private InputActionMap uiMap;
    private Button activeButton = null;
    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        controller = go.GetComponent<PlayerController>();
        player = go.GetComponent<Player>();

        uiMap = EventSystem.current.GetComponent<InputSystemUIInputModule>().actionsAsset.actionMaps[0];
    }

    private void Update()
    {
        InitializeActiveButton();
    }

    public void OnHoverButton(GameObject button)
    {
        if (button != null)
        {
            RawImage border = button.transform.Find("Border").GetComponent<RawImage>();
            border.color = Color.white;
        }
    }

    public void OnHoverExitButton(GameObject button)
    {
        if (button != null)
        {
            RawImage border = button.transform.Find("Border").GetComponent<RawImage>();
            border.color = Color.black;
        }
    }

    public void HandleResume()
    {
        ResetActiveButton();
        player.playerState = controller.lastPlayerState;
    }

    public void HandleSettings()
    {
        ResetActiveButton();
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void HandleBack()
    {
        ResetActiveButton();
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void HandleQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    #region UI Controller
    private void InitializeActiveButton()
    {
        if (activeButton == null )
        {
            if (uiMap["Navigate"].ReadValue<Vector2>() != Vector2.zero)
            {
                SetDefaultButtonActive();
            }
        }
    }
    private void SetDefaultButtonActive()
    {
        bool isMainMenu = mainMenu.activeSelf;
        Button btn = isMainMenu ? mainMenuDefaultBtn : settingsMenuDefaultBtn;
        SetActiveButton(btn);
    }
    private void SetActiveButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        activeButton = button;
    }

    public void ResetActiveButton()
    {
        activeButton = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion
}