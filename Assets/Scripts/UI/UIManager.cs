using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private PlayerController controller;
    private Player player;
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject settingsMenu;
    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        controller = go.GetComponent<PlayerController>();
        player = go.GetComponent<Player>();
    }

    private void Update()
    {
        
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
        player.playerState = controller.lastPlayerState;
    }

    public void HandleSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void HandleBack()
    {
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
}