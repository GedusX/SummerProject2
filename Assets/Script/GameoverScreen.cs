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
        GameObject.Find("finalscore").GetComponent<TextMeshProUGUI>().text = "your final score: " + ClassicHandler.score.ToString();
    }
    public void mainmenu(){
        SceneManager.LoadScene("Scenes/Menu",LoadSceneMode.Single);
    }
    public void replay(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        SceneManager.LoadScene("Scenes/SampleScene",LoadSceneMode.Single);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
