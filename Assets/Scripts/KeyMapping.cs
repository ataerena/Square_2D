using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KeyMapping : MonoBehaviour
{
    private string keyboardFilePath;
    private string controllerFilePath;

    private Dictionary<string, string> keyboardDictionary;
    private Dictionary<string, string> controllerDictionary;

    private void Start()
    {
        keyboardFilePath = Path.Combine(Application.persistentDataPath, "KeyboardMappings.json");
        controllerFilePath = Path.Combine(Application.persistentDataPath, "/Utils/ControllerMappings.json");

        LoadKeyBindings();
    }

    private void LoadKeyBindings()
    {
        Debug.Log("Path: " + keyboardFilePath);
        if (File.Exists(keyboardFilePath))
        {
            string json = File.ReadAllText(keyboardFilePath);
            Debug.Log("Json: " + json);
        }
        else
        {
            string newKeyboardFilePath = keyboardFilePath.Replace("\\", "/");
            Debug.Log("Path: " + newKeyboardFilePath);
            if (File.Exists(newKeyboardFilePath))
            {
                string json = File.ReadAllText(keyboardFilePath);
                Debug.Log("Json: " + json);
            }
        }
    }
}
