using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
public class mainmenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.StopAll();
        MusicManager.instance.Play("menu");
    }

    public void playgame(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        StartCoroutine(SceneLoader.instance.transition("Scenes/Blitz",Vector2.zero));
        AudioManager.instance.Play("warp1");
        StartCoroutine(MusicManager.instance.fading("menu",0.6f));

    }
    public void classic(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        GameObject.Find("classic").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        StartCoroutine(SceneLoader.instance.transition("Scenes/Classic",Vector2.zero));
        AudioManager.instance.Play("warp1");
        StartCoroutine(MusicManager.instance.fading("menu",0.6f));
    }
    public void blitz(){
        GameObject.Find("blitz").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        StartCoroutine(SceneLoader.instance.transition("Scenes/Blitz",Vector2.zero));
        AudioManager.instance.Play("warp1");
        StartCoroutine(MusicManager.instance.fading("menu",0.6f));

    }

    public void classic_hover(){
        GameObject.Find("classic").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("classic_glow").GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "No more moves, you lose, how many levels can you go?";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);
    }
    public void classic_return(){
        GameObject.Find("classic").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("classic_glow").GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "-HOW TO PLAY-\nDrag to link hexagons in the same line";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void blitz_hover(){
        GameObject.Find("blitz").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("blitz_glow").GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "How many score can you get in 3 minutes? Let's find out!";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void blitz_return(){
        GameObject.Find("blitz").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("blitz_glow").GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "-HOW TO PLAY-\nDrag to link hexagons in the same line";


        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void options_hover(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("options").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "Change settings";
        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void options_return(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("options").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void options(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        SceneManager.LoadSceneAsync("Scenes/Options",LoadSceneMode.Additive);
    }
    public void quit(){
        Application.Quit();
    }
    public IEnumerator pausing(){
        Debug.Log("op1");
        GameObject canvas = GameObject.Find("_Buttons");
        canvas.SetActive(false);
        yield return new WaitUntil(()=>(!SceneManager.GetSceneByName("Options").IsValid()));
        Debug.Log("op2");
        canvas.SetActive(true);

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            
            SceneManager.LoadSceneAsync("Scenes/Options", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            
            StartCoroutine(pausing());
            //SceneManager.SetActiveScene;
            //Time.timeScale = 0;
            
        }
    }
}
