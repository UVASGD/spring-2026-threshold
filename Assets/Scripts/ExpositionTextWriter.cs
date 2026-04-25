using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpositionTextWriter : MonoBehaviour
{
    [SerializeField] List<string> introExposition;
    [SerializeField] TMP_Text textOutput;
    [SerializeField] float characterDelay = 0.1f;
    [SerializeField] float lineDelay = 1.5f;

    [Header("Intro Scene")]
    [SerializeField] string introSceneName;
    public IEnumerator typeText(string text)
    {
        if (textOutput == null)
        {
            yield break;
        }

        textOutput.text = string.Empty;
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        if (characterDelay <= 0f)
        {
            textOutput.text = text;
            yield break;
        }

        foreach (char letter in text)
        {
            textOutput.text += letter;
            yield return new WaitForSeconds(characterDelay);
        }

        yield return null;
    }

    void Awake()
    {
        StartCoroutine(IntroCutscene());   
    }

    public IEnumerator ExpositionCutscene(List<string> lines){
        foreach(var text in lines){
            yield return typeText(text);
            yield return new WaitForSeconds(lineDelay);
        }
        yield return null;
    }

    public IEnumerator IntroCutscene(){
        yield return ExpositionCutscene(introExposition);
        
        //load the intro scene
        SceneManager.LoadScene(introSceneName);
    }
}