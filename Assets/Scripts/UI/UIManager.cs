using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    /* private Controls controls; */
    private PlayerController controller;
    private Player player;
    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        controller = go.GetComponent<PlayerController>();
        player = go.GetComponent<Player>();
        /* controls = new Controls(); */
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

    public void HandleQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}