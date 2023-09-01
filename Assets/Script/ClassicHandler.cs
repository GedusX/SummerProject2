using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class ClassicHandler : Handler
{
    // Start is called before the first frame update
    List<GameObject> holding;
    public int display_score;
    public float multiplier; 
    //public int difficulty;

    
    public int milestone(int lvl){
        return (lvl*lvl)*1500;
    }
    public GameObject text_module;
    public override void update_score(int sc){
        //Debug.Log(11);
        score += Mathf.RoundToInt(sc*level);
    }
    void Awake(){
        
    }
    IEnumerator no_more_moves(){
        StartCoroutine(MusicManager.instance.fading("classic_music",0.6f));
        yield return new WaitForSeconds(0.6f);
        MusicManager.instance.Play("gameover");
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("NO MORE MOVES"));
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Scenes/Gameover",LoadSceneMode.Single);
    }
    public override void gameover(){
        StartCoroutine(no_more_moves());
    }
    void Start()
    {
        level = 1;
        MusicManager.instance.Play("classic_music");
        level_standby();
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("GO!"));
    }

    void level_standby(){
        GameObject.Find("progress_bar").GetComponent<Slider>().maxValue = milestone(level);
        GameObject.Find("progress_bar").GetComponent<Slider>().minValue = score;
        difficulty = Mathf.CeilToInt(100 * (1 - Mathf.Exp(-0.5f * level)));
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
    public IEnumerator hinting(){
        if (GameObject.Find("board").GetComponent<board>().idle){
            GameObject hint_hex = GameObject.Find("board").GetComponent<board>().get_hint();
            GameObject.Find("hint").transform.position = hint_hex.transform.position;
            GameObject.Find("hint").GetComponent<SpriteRenderer>().DOFade(1.0f,0.3f);
            
            yield return new WaitForSeconds(3.0f);
            GameObject.Find("hint").GetComponent<SpriteRenderer>().DOFade(0.0f,0.3f);
        }
        
    }
    void mainmenu(){
        SceneManager.LoadScene("Scenes/Menu",LoadSceneMode.Single);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            mainmenu();
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(hinting());
        }
        if (display_score < score){
            display_score += Mathf.RoundToInt(10.0f);
        }
        else
            display_score = score;
        GameObject.Find("scorepod").transform.Find("score_text").gameObject.GetComponent<TextMeshPro>().text = display_score.ToString("N0");
        GameObject.Find("scorepod").transform.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = level.ToString();
        GameObject.Find("progress_bar").GetComponent<Slider>().value = display_score;
    }
}
