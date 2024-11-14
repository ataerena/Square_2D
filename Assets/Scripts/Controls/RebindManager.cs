using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindManager : MonoBehaviour
{
    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
    }

    public void StartRebindingKeyboard(GameObject button)
    { 
        string objectName = button.name;
        Button buttonToUpdate = button.GetComponent<Button>();

        InputAction targetAction = GetTargetAction(objectName);

        targetAction.Disable();

        if (targetAction == controls.Player.Move)
        {
           int bindingIndex = GetMovementBindingIndex(objectName);
           Debug.Log($"Index: {bindingIndex}");

           targetAction.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("<Gamepad>")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => 
                {
                    string existingText = buttonToUpdate.GetComponentInChildren<Text>().text;
                    string buttonName = existingText.Substring(0, existingText.IndexOf(":") + 1);
                    string keys = targetAction.GetBindingDisplayString();
                    string newKey;

                    string[] bindings = keys.Split('|');
                    string keyPart = bindings[1].Trim();
                    if (keyPart.Contains("/"))
                    {
                        newKey = keyPart.Split('/')[bindingIndex];
                    }
                    else
                    {
                        throw new System.Exception("No good.");
                    }

                    buttonToUpdate.GetComponentInChildren<Text>().text = $"{buttonName} ${newKey}";
                    operation.Dispose();
                    targetAction.Enable();
                    Debug.Log($"Action {objectName} has been rebound to {newKey}.");
                })
                .Start();
        }
        else
        {
            targetAction.PerformInteractiveRebinding()
                .WithControlsExcluding("<Gamepad>")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => 
                {
                    string existingText = buttonToUpdate.GetComponentInChildren<Text>().text;
                    string buttonName = existingText.Substring(0, existingText.IndexOf(":") + 1);
                    string newKey = targetAction.GetBindingDisplayString();

                    buttonToUpdate.GetComponentInChildren<Text>().text = $"{buttonName} ${newKey}";
                    operation.Dispose();
                    targetAction.Enable();
                })
                .Start();
        }
    }

    private InputAction GetTargetAction(string name)
    {
        if (name.ToLower().Contains("move"))
        {
            return controls.Player.Move;
        }
        else if (name.ToLower().Contains("jump"))
        {
            return controls.Player.Jump;
        } 
        else if (name.ToLower().Contains("dash"))
        {
            return controls.Player.Dash;
        }
        else
        {
            throw new System.Exception($"No good: {name}");
        }
    }

    private int GetMovementBindingIndex(string name)
    {
        if (name.ToLower().Contains("move"))
        {
            int startingIndex = name.IndexOf(" ");
            int endingIndex = name.Length;
            string direction = name.Substring(startingIndex, endingIndex - startingIndex);
            Debug.Log($"Direction: {direction}");
            switch (direction.Trim())
            {
                case "Up": return 0;
                case "Down": return 1;
                case "Left": return 2;
                case "Right": return 3;
                default: throw new System.Exception($"No good: {direction}");            
            }
        }
        else
        {
            throw new System.Exception($"No good: {name}");
        }
    }
}
