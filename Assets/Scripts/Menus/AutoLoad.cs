using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoad : MonoBehaviour
{
    private AsyncOperation async;   
    [SerializeField] private string loadSceneByName = "";
    [SerializeField] private int sceneToLoad = -1;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;      //Reset to default time scale
        Input.ResetInputAxes();     //Avoid accidental selection on button release
        System.GC.Collect();        //Clear memory of unused items
        Scene currentScene = SceneManager.GetActiveScene(); //Get current scene

        if (loadSceneByName != "")  //If a scene was specified
        {
            async = SceneManager.LoadSceneAsync(loadSceneByName);   //Load specified scene
        }
        else if (sceneToLoad < 0)   //If scene number is negative
        {
            async = SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);   //Load next scene
        }
        else                        //If scene number is specified
        {
            async = SceneManager.LoadSceneAsync(sceneToLoad);   //Load specified scene
        }
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
