using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
public class Options : MonoBehaviour
{
    // Start is called before the first frame update
    public void back(){
        GameObject.Find("back").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).From(Vector3.one*0.73f).SetEase(Ease.InOutQuad);
        //Time.timeScale = 1.0f;
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scenes/mainmenu"));
        SceneManager.UnloadSceneAsync(gameObject.scene);
        
        //SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene("Scenes/mainmenu", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        
    }
    public void back_hover(){
        GameObject.Find("back").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("back").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "No more moves, you lose, how many levels can you go?";
        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);
    }
    public void back_return(){
        GameObject.Find("back").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("back").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void feedback_hover(){
        GameObject.Find("feedback").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("feedback").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "How many score can you get in 3 minutes? Let's find out!";
        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void feedback_return(){
        GameObject.Find("feedback").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        GameObject.Find("feedback").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void audio_hover(){
        //GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("audio").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "How many score can you get in 3 minutes? Let's find out!";
        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void audio_return(){
        //GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("audio").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }


    public void music_hover(){
        //GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("music").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "How many score can you get in 3 minutes? Let's find out!";
        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void music_return(){
        //GameObject.Find("options").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("music").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        //GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        //GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void setAudioVolume(){
        //GlobalConfig.audio_volume
        AudioManager.instance.VolumeChange();
        AudioManager.instance.Play("powerhex");
    }
    public void setMusicVolume(){
        //GlobalConfig.audio_volume
        MusicManager.instance.VolumeChange();
    }
    public void mm_hover(){
        GameObject.Find("main_menu").GetComponent<RectTransform>().DOScale(Vector3.one*1.25f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("main_menu").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "How many score can you get in 3 minutes? Let's find out!";
        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    public void mm_return(){
        GameObject.Find("main_menu").GetComponent<RectTransform>().DOScale(Vector3.one*1f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("main_menu").transform.Find("glow").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad);
        GameObject.Find("info").GetComponent<TextMeshPro>().text = "PAUSED";

        GameObject.Find("info").GetComponent<TextMeshPro>().DOFade(1.0f,0.3f).From(0.0f).SetEase(Ease.InOutQuad);

    }
    void Start()
    {
        //MusicManager.instance.StopAll();
        //MusicManager.instance.Play("menu");
        //SceneManager.SetActiveScene(gameObject.scene);
        GameObject.Find("audio").GetComponentInChildren<Slider>().value=GlobalConfig.audio_volume;
        GameObject.Find("music").GetComponentInChildren<Slider>().value=GlobalConfig.music_volume;
    }

    // Update is called once per frame
    void Update()
    {
        GlobalConfig.audio_volume = GameObject.Find("audio").GetComponentInChildren<Slider>().value;
        GlobalConfig.music_volume = GameObject.Find("music").GetComponentInChildren<Slider>().value;

    }
}
