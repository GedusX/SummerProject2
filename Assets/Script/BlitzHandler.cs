using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BlitzHandler : MonoBehaviour
{
    // Start is called before the first frame update
    static public int score = 0;
    static public int level = 1;
    List<GameObject> holding;
    public int display_score;
    public float multiplier; 
    
    public int difficulty;
    public int milestone(int lvl){
        return (lvl*lvl)*1500;
    }
    public void update_score(int sc){
        //Debug.Log(11);
        score += Mathf.RoundToInt(sc*level);
    }
    public void gameover(){
        SceneManager.LoadScene("Scenes/Gameover",LoadSceneMode.Single);
    }
    void Start()
    {
        level = 1;
        AudioManager.instance.Play("music");
        level_standby();
    }
    void level_standby(){
        GameObject.Find("progress_bar").GetComponent<Slider>().maxValue = milestone(level);
        GameObject.Find("progress_bar").GetComponent<Slider>().minValue = score;
        difficulty = level*level*level*level;
        StartCoroutine(level_process());
    }
    IEnumerator level_up(){
        yield return new WaitForSeconds(1.0f);
    }
    IEnumerator level_process(){
        yield return new WaitUntil(() => (GameObject.Find("progress_bar").GetComponent<Slider>().value>=GameObject.Find("progress_bar").GetComponent<Slider>().maxValue));
        level+=1;
        AudioManager.instance.Play("multiup");
        level_standby();
        //StartCoroutine(level_process());
    }
    // Update is called once per frame
    void Update()
    {
        if (display_score < score){
            display_score += Mathf.RoundToInt(10.0f);
        }
        else
            display_score = score;
        GameObject.Find("scorepod").transform.Find("score_text").gameObject.GetComponent<TextMeshPro>().text = display_score.ToString();
        GameObject.Find("scorepod").transform.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = level.ToString();
        GameObject.Find("progress_bar").GetComponent<Slider>().value = display_score;
    }
}
