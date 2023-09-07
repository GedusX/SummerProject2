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
    private GameObject background;
    private GameObject background_old;

    [SerializeField] private List<GameObject> bgavail;

    [SerializeField] private GameObject hex_mask;
    
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
        background = Instantiate(bgavail[level % bgavail.Count],Vector3.zero,Quaternion.identity);

        level_standby();
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("GO!"));
    }

    void level_standby(){
        GameObject.Find("progress_bar").GetComponent<Slider>().maxValue = milestone(level);
        GameObject.Find("progress_bar").GetComponent<Slider>().minValue = score;
        difficulty = level*level*level;
        //difficulty = Mathf.CeilToInt(100 * (1 - Mathf.Exp(-0.5f * level)));
        StartCoroutine(level_process());
    }
    IEnumerator level_up(){
        maskobjout(background);
        //background.transform.SetAsFirstSibling();
        background_old = background;
        background = Instantiate(bgavail[level % bgavail.Count],Vector3.zero,Quaternion.identity);
        maskobjin(background);
        StartCoroutine(holing(Vector2Int.zero));
        yield return new WaitForSeconds(5.0f);
        hole_pos.Clear();
        Destroy(background_old);
        maskobjreset(background);
        level_standby();

    }
    List<Vector2Int> hole_pos = new List<Vector2Int>();
    bool is_next_to(Vector2Int pos1, Vector2Int pos2){
        //Debug.Log((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude);
        return (((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude)<=1*1.1f);

    }
    Vector2 grid_to_pix(int i, int j){
        float xOffset = 0.8743f;
        float yOffset = 0.4722f;
        float x = i * xOffset;
        float y = j * 0.9425f + (i % 2) * yOffset;
        return transform.position + new Vector3(x,y);
    }
    IEnumerator holing(Vector2Int pos){
        GameObject effect;
        if ((!hole_pos.Contains(pos))&&(pos.magnitude<=15)){
            effect = Instantiate(hex_mask,grid_to_pix(pos.x,pos.y),Quaternion.identity);
            hole_pos.Add(pos);
            effect.transform.localScale = Vector3.zero;
            effect.transform.DOScale(Vector3.one*0.5f,0.7f);
            yield return new WaitForSeconds(0.35f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(holing(pos + new Vector2Int(i,j)));
                    }
                }
            }
            yield return new WaitForSeconds(5.0f);
            effect.GetComponentInChildren<SpriteRenderer>().DOFade(0.0f,0.3f);
            yield return new WaitForSeconds(1.0f);
            Destroy(effect);

        }
            
        

    }
    IEnumerator level_transition(){

        yield return new WaitForSeconds(1.0f);
    }




    void maskobjout(GameObject msk){
        foreach (var i in msk.GetComponentsInChildren<SpriteRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        foreach (var i in msk.GetComponentsInChildren<ParticleSystemRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
    void maskobjin(GameObject msk){
        foreach (var i in msk.GetComponentsInChildren<SpriteRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        foreach (var i in msk.GetComponentsInChildren<ParticleSystemRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }
    void maskobjreset(GameObject msk){
        foreach (var i in msk.GetComponentsInChildren<SpriteRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.None;
        }
        foreach (var i in msk.GetComponentsInChildren<ParticleSystemRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.None;
        }
    }
    IEnumerator level_process(){
        yield return new WaitUntil(() => (GameObject.Find("progress_bar").GetComponent<Slider>().value>=GameObject.Find("progress_bar").GetComponent<Slider>().maxValue));
        level+=1;
        AudioManager.instance.Play("multiup");
        StartCoroutine(level_up());
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
