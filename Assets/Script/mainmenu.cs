using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void playgame(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        SceneManager.LoadScene("Scenes/SampleScene",LoadSceneMode.Single);
    }
    public void quit(){
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
