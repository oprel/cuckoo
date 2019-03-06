using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cutsceneManager : MonoBehaviour
{
    static cutsceneManager self;
    public bool startCutscene;
    private bool old;

  

    public static void loadScene(bool cutscene){
        self.StartCoroutine(loadSceneRoutine(cutscene));
    }
    // Start is called before the first frame update
    public static IEnumerator loadSceneRoutine(bool cutscene){
        Scene main = SceneManager.GetSceneByName("main");
        Debug.Log(main);
        if (main.isLoaded) SceneManager.UnloadSceneAsync(main);
        var loadedLevel = Application.LoadLevelAdditiveAsync("main");
        yield return loadedLevel;
        Camera.main.GetComponent<Cutscene>().playCutscene=cutscene;
        Camera.main.GetComponent<Cutscene>().Start();


    }
}
