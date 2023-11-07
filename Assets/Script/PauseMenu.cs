using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public static string previous_scene;
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
    public void resume(){
        GameObject.Find("resume").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        SceneManager.UnloadSceneAsync(gameObject.scene);

    }
    public void restart(){
        GameObject.Find("restart").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        StartCoroutine(SceneLoader.instance.transition(previous_scene,Vector2.zero));
        Time.timeScale = 1;
    }
    public void options(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        SceneManager.LoadSceneAsync("Scenes/Options",LoadSceneMode.Additive);
    }
    public void mainmenu(){
        GameObject.Find("main_menu").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        StartCoroutine(SceneLoader.instance.transition("Scenes/Menu",Vector2.zero));
        Time.timeScale =1;
    }
    public void resume_hover(){
        GameObject.Find("resume").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("resume").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "Continue the game";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);
    }
    public void resume_return(){
        GameObject.Find("resume").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("resume").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void restart_hover(){
        GameObject.Find("restart").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("restart").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "Abandon the current game and start a new one";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void restart_return(){
        GameObject.Find("restart").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("restart").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void options_hover(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("options").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "Change settings";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void options_return(){
        GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("options").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }

    public void mm_hover(){
        GameObject.Find("main_menu").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("main_menu").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "Go back to the main menu (your process will not be saved)";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

    }
    public void mm_return(){
        GameObject.Find("main_menu").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("main_menu").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad).SetUpdate(true);

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
        
    }
}
