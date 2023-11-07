using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
public class GameoverScreen_Blitz : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //BlitzHandler.stat = new BlitzStat();
        thankyou = GameObject.Find("thankyou");
        thankyou.SetActive(false);
        //MusicManager.instance.Play("gameover");
        GameObject.Find("finalscore").GetComponent<TextMeshProUGUI>().text = Handler.score.ToString("N0");
        GameObject.Find("hex_num").GetComponent<TextMeshProUGUI>().text = BlitzHandler.stat.num_of_hex.ToString("N0");
        GameObject.Find("bomb_num").GetComponent<TextMeshProUGUI>().text = BlitzHandler.stat.num_of_bomb.ToString("N0");
        GameObject.Find("laser_num").GetComponent<TextMeshProUGUI>().text = BlitzHandler.stat.num_of_laser.ToString("N0");
        GameObject.Find("void_num").GetComponent<TextMeshProUGUI>().text = BlitzHandler.stat.num_of_void.ToString("N0");
        GameObject.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = ((char) 215) + BlitzHandler.stat.multiplier.ToString();
        show_chart();
        StartCoroutine(Post(BlitzHandler.stat.score,BlitzHandler.stat.multiplier,BlitzHandler.stat.num_of_hex,BlitzHandler.stat.num_of_bomb,BlitzHandler.stat.num_of_laser,BlitzHandler.stat.num_of_void,"yes"));
    }
    void show_chart(){
        for (int i = 5;i>=0;i--){
            GameObject.Find("col"+i.ToString()).GetComponent<Slider>().maxValue = 1;
            GameObject.Find("col"+i.ToString()).GetComponent<Slider>().minValue = 0;
            Slider xx = GameObject.Find("col"+i.ToString()).GetComponent<Slider>();
            DOTween.To(() => xx.value, x => xx.value = x,BlitzHandler.stat.score_timeline[i]*1.0f/Mathf.Max(BlitzHandler.stat.score_timeline.ToArray())*1.0f, 3f).From(0).SetEase(Ease.InOutQuad);
            //GameObject.Find("text"+i.ToString()).GetComponent<RectTransform>().DOMoveY(-144 + (108.5f + 144)*).From(-144).SetEase(Ease.InOutQuad); 
            
        }
    }
    GameObject thankyou;
    public void mainmenu(){
        StartCoroutine(SceneLoader.instance.transition("Scenes/Menu",Vector2.zero));
    }

    IEnumerator submit_form(){
        GameObject.Find("info").SetActive(false);
        GameObject.Find("yes_button").SetActive(false);
        GameObject.Find("no_button").SetActive(false);
        thankyou.SetActive(true);
        yield return new WaitForSeconds(4f);
        mainmenu();
    }
    public void replay(){
        ClassicHandler.score = 0;
        ClassicHandler.level = 1;
        StartCoroutine(SceneLoader.instance.transition("Scenes/Classic",Vector2.zero));
    }

    public void yes(){
        //StartCoroutine(Post(BlitzHandler.stat.score,BlitzHandler.stat.multiplier,BlitzHandler.stat.num_of_hex,BlitzHandler.stat.num_of_bomb,BlitzHandler.stat.num_of_laser,BlitzHandler.stat.num_of_void,"yes"));
        StartCoroutine(submit_form());
    }
    public void no(){
        //StartCoroutine(Post(BlitzHandler.stat.score,BlitzHandler.stat.multiplier,BlitzHandler.stat.num_of_hex,BlitzHandler.stat.num_of_bomb,BlitzHandler.stat.num_of_laser,BlitzHandler.stat.num_of_void,"no"));
        StartCoroutine(submit_form());
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 5;i>=0;i--){
            GameObject.Find("text"+i.ToString()).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(Mathf.Max(BlitzHandler.stat.score_timeline.ToArray())*GameObject.Find("col"+i.ToString()).GetComponent<Slider>().value).ToString("N0");

            GameObject.Find("text"+i.ToString()).GetComponent<RectTransform>().localPosition = new Vector3(GameObject.Find("text"+i.ToString()).GetComponent<RectTransform>().localPosition.x,(-144 + (108.5f + 144)*GameObject.Find("col"+i.ToString()).GetComponent<Slider>().value),GameObject.Find("text"+i.ToString()).GetComponent<RectTransform>().localPosition.z);
            
        }
    }
    [SerializeField] InputField feedback1;

    string URL = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScxTDF1qMyfsS5ahc0Xr3-WkyGMegMxoQRoN_PLDpQ1kyW0nA/formResponse";

    

    IEnumerator Post(int scores, int multi, int numhex, int numbomb, int numlaser, int numvoid, string like)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1978545043", scores);
        form.AddField("entry.2059788760", multi);
        form.AddField("entry.1355504144",numhex);
        form.AddField("entry.270720254", numbomb);
        form.AddField("entry.1656265624",numlaser);
        form.AddField("entry.1476613168",numvoid);
        form.AddField("entry.670560911",like);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        
        yield return www.SendWebRequest();
        Debug.Log("success");
    }


}