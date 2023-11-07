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
        GameObject.Find("finalscore").GetComponent<TextMeshProUGUI>().text = ClassicHandler.score.ToString("N0");
        GameObject.Find("hex_num").GetComponent<TextMeshProUGUI>().text = ClassicHandler.stat.num_of_hex.ToString("N0");
        GameObject.Find("bomb_num").GetComponent<TextMeshProUGUI>().text = ClassicHandler.stat.num_of_bomb.ToString("N0");
        GameObject.Find("laser_num").GetComponent<TextMeshProUGUI>().text = ClassicHandler.stat.num_of_laser.ToString("N0");
        GameObject.Find("void_num").GetComponent<TextMeshProUGUI>().text = ClassicHandler.stat.num_of_void.ToString("N0");
        GameObject.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = ClassicHandler.stat.level.ToString();
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
