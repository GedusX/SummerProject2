using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameoverScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //MusicManager.instance.Play("gameover");
        GameObject.Find("finalscore").GetComponent<TextMeshProUGUI>().text = "your final score:\n" + ClassicHandler.score.ToString();
    }
    public void mainmenu(){
        StartCoroutine(SceneLoader.instance.transition("Scenes/Menu",Vector2.zero));
    }
    public void replay(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        StartCoroutine(SceneLoader.instance.transition("Scenes/Classic",Vector2.zero));

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
