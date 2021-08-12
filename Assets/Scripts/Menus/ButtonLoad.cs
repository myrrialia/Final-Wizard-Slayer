using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ButtonLoad : MonoBehaviour
{
    private AsyncOperation async;   //Data that's been loaded

    public void BtnLoadScene(int i)
    {
        if (async != null) return;  //If there's already something loading, don't continue
        Time.timeScale = 1.0f;      //Reset to default time scale
        Input.ResetInputAxes();     //Avoid accidental selection on button release
        System.GC.Collect();        //Clear memory of unused items
        async = SceneManager.LoadSceneAsync(i);   //Load specified scene
        async.allowSceneActivation = false; //Wait to switch to next scene
    }

    // Update is called once per frame
    void Update()
    {
        //if (async.progress >= 0.9f && SplashScreen.isFinished)  //If next scene is 90% loaded
        if (async != null && async.progress >= 0.9f)
        {
            async.allowSceneActivation = true;
        }

    }
}
