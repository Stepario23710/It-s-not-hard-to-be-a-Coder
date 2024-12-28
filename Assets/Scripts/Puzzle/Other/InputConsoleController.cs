using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputConsoleController : OutputConsoleController
{
    private Text input;
    private string inputText;
    public static event Action InputDone;
    void Start()
    {
        gameObject.SetActive(false);
        Invoke(nameof(AppearWithInput), 0.1f);
    }
    void Update()
    {
        inputText = input.text;
        if (inputText.Length > 0 && inputText.Substring(0, 1) == "-"){
            gameObject.transform.GetChild(0).GetComponent<InputField>().characterLimit = 2;
        } else {
            gameObject.transform.GetChild(0).GetComponent<InputField>().characterLimit = 1;
        }
    }
    public int ReturnInput(){
        return Int32.TryParse(inputText, out int i) ? i : 0;
    }
    public void InputEnd(){
        InputDone?.Invoke();
        Disappear();
    }
    private void AppearWithInput(){
        gameObject.SetActive(true);
        input = gameObject.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        gameObject.transform.GetChild(0).GetComponent<InputField>().ActivateInputField();
    }
}
