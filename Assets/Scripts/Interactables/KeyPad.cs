using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyPad : MonoBehaviour
{
    [SerializeField] TMP_Text outputText;
    [SerializeField] private int consoleID;
    [SerializeField] Action<int> onCompletion;
    char[] digitValues = {'-','-','-','-'}; 
    [SerializeField] List<char> correctChars;
    void Awake()
    {
        KeyCap.onKeycapPress += handleKeycapPress;
    }

    private void handleKeycapPress(int keypadID, int keyValue)
    {
        if(keypadID == this.consoleID)
        {
           for(int i = 0; i < digitValues.Length; i++)
            {
                if(digitValues[i] == '-')
                {
                    //modify the value of the current '-' mark to the keyPad value
                    digitValues[i] = (char)('0' + keyValue);

                    updateKeypadText();
                    
                    if (i == digitValues.Length - 1) //the final digit has been input
                    {
                        //complete the input of the code
                        if (codeInputCheck())
                        {
                            
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                
                updateKeypadText();
            }
        }
    }
    public void updateKeypadText()
    {
        outputText.text = "";
        for(int i = 0; i < 4; i++)
        {
            if(i<digitValues.Length - 1)
            {
                outputText.text += digitValues[i];
            }
            else
            {
                outputText.text += '-';
            }
        }
    }
    public bool codeInputCheck()
    {
        //check if the four inputted characters are correct
        for(int i = 0; i < correctChars.Count; i++)
        {
            if(correctChars[i] == digitValues[i])
            {
                continue;
            }
            else
            {
                //code check failed
                Debug.Log("Keypad inputted incorrectly");
                for(int j = 0; j < 3; j++) //reset screen characters
                {
                    digitValues[j] = '-';
                }
                updateKeypadText();
                return false;
            }
        }
        Debug.Log("Keypad inputted correctly");
        return true;
    }
}