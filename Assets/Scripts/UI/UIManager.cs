using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    /* private Controls controls; */
    private void Start()
    {
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
}