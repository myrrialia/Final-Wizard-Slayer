using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningSequence : MonoBehaviour
{
    private bool introInProgress = true;
    private int currentDialogue = 0;
    [SerializeField] string[] dialogue;
    [SerializeField] private Text txt_message;

    public void PrintNextMessage()
    {

        if (currentDialogue < dialogue.Length)
        {
            txt_message.text = dialogue[currentDialogue];   //If there's still dialogue to print
            currentDialogue++;
            introInProgress = true;
        }
        else
        {
            Time.timeScale = 1.0f;      //Reset to default time scale
            Input.ResetInputAxes();     //Avoid accidental selection on button release
            System.GC.Collect();        //Clear memory of unused items
            Scene currentScene = SceneManager.GetActiveScene(); //Get current scene
            AsyncOperation async = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);   //Load next scene
            async.allowSceneActivation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (introInProgress && Input.anyKeyDown)        //Waiting for key press to continue opening sequence
        {
            introInProgress = false;
            PrintNextMessage();
        }
    }
}
