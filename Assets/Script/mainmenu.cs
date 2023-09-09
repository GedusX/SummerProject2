using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.StopAll();
    }

    public void playgame(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        StartCoroutine(SceneLoader.instance.transition("Scenes/Classic",Vector2.zero));

    }
    public void quit(){
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
