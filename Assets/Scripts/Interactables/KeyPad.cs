using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KeyPad : MonoBehaviour
{
    [SerializeField] TMP_Text outputText;
    [SerializeField] private int consoleID;
    [SerializeField] UnityEvent onCompletion;
    char[] digitValues = {'-','-','-','-'};
    [SerializeField] List<char> correctChars;
    private int currentDigitIndex;

    private bool lockState = false; //once answered correctly, enter a lock state

    void Awake()
    {
        KeyCap.onKeycapPress += handleKeycapPress;
        resetInput();
    }

    private void OnDestroy()
    {
        KeyCap.onKeycapPress -= handleKeycapPress;
    }

    private void handleKeycapPress(int keypadID, int keyValue)
    {
        if(lockState == true) return; //ignore if in lock state

        if(keypadID != this.consoleID)
        {
            return;
        }

        if (currentDigitIndex >= digitValues.Length)
        {
            resetInput();
        }

        digitValues[currentDigitIndex] = convertKeyValueToChar(keyValue);
        currentDigitIndex++;
        updateKeypadText();

        if (currentDigitIndex < digitValues.Length)
        {
            return;
        }

        if (codeInputCheck())
        {
            Debug.Log("Keypad inputted correctly");
            onCompletion?.Invoke();
            lockState = true;

            return;
        }

        Debug.Log("Keypad inputted incorrectly");
        resetInput();
    }

    private char convertKeyValueToChar(int keyValue)
    {
        if (keyValue >= 0 && keyValue <= 9)
        {
            return (char)('0' + keyValue);
        }

        return keyValue.ToString()[0];
    }

    public void updateKeypadText()
    {
        if(outputText == null)
        {
            return;
        }

        outputText.text = new string(digitValues);
    }

    public bool codeInputCheck()
    {
        if(correctChars == null || correctChars.Count != digitValues.Length)
        {
            Debug.LogWarning($"KeyPad {consoleID} has invalid correctChars setup. Expected {digitValues.Length} chars.");
            return false;
        }

        for(int i = 0; i < digitValues.Length; i++)
        {
            if(correctChars[i] != outputText.text.ToCharArray()[i])
            {
                Debug.Log("Keypad inputted incorrectly");
                return false;
            }
        }

        Debug.Log("Keypad inputted correctly");
        return true;
    }

    private void resetInput()
    {
        if(lockState) return;
        for(int i = 0; i < digitValues.Length; i++)
        {
            digitValues[i] = '-';
        }

        currentDigitIndex = 0;
        updateKeypadText();
    }
}