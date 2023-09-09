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
    //public List<GameObject> holding;
    public int display_score;
    public float multiplier; 
    //public int difficulty;
    private GameObject background;
    private GameObject background_old;
    private board bd;

    [SerializeField] private List<GameObject> bgavail;

    [SerializeField] private GameObject hex_mask;
    
    public int milestone(int lvl){
        return (lvl*lvl)*1500;
    }
    public GameObject text_module;
    Sequence score_seq;
    public override void update_score(int sc){
        //Debug.Log(11);
        score += Mathf.RoundToInt(sc*level);
        score_seq.Append(DOTween.To(() => display_score, x => display_score = x, score, 0.5f).SetEase(Ease.InOutQuad));
        //score_seq.Append(DOTween.To(() => display_score, x => display_score = x, score, 0.3f).SetEase(Ease.InOutQuad));
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
        StartCoroutine(t.GetComponent<bigtext>().texting2("NO MORE MOVES"));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SceneLoader.instance.transition("Scenes/Gameover",Vector2.zero));
        //SceneManager.LoadScene("Scenes/Gameover",LoadSceneMode.Single);
    }
    public override void gameover(){
        StartCoroutine(no_more_moves());
    }
    void Start()
    {
        Sequence score_seq = DOTween.Sequence();
        holding = new List<GameObject>();
        bd = GameObject.Find("board").GetComponent<board>();
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
        difficulty = Mathf.RoundToInt(100 * (1 - Mathf.Pow(0.55f,level)));
        //difficulty = Mathf.CeilToInt(100 * (1 - Mathf.Exp(-0.5f * level)));
        bd.idle = true;
        StartCoroutine(level_process());
    }
    IEnumerator level_up(){
        
        for (int i = 0; i < bd.boardWidth;i++){
            for (int j = 0;j<bd.boardHeight;j++){
                if (bd.is_legit(new Vector2Int(i,j))&&bd.hexagons[i,j].GetComponent<Hexa>().type!=Hexa_Type.NORMAL){
                    holding.Add(bd.hexagons[i,j]);
                }
            }
        }
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting3("LEVEL COMPLETE"));


        yield return new WaitForSeconds(1.0f);
        
        StartCoroutine(ui_transition());
        background_old = background;
        yield return new WaitForSeconds(1.2f);
        level+=1;
        background = Instantiate(bgavail[level % bgavail.Count],Vector3.zero,Quaternion.identity);
        maskobjout(background_old);
        maskobjin(background);
        background.transform.SetAsLastSibling();
        StartCoroutine(holing(Vector2Int.zero));
        
        yield return new WaitForSeconds(2.5f);
        maskobjreset(background);
        Destroy(background_old);
        
        yield return new WaitForSeconds(2.0f);
        
        AudioManager.instance.Play("trans_preig");
        level_standby();
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(MusicManager.instance.fadin("classic_music",0.6f));
        
        t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("LEVEL " + level.ToString()));

    }
        IEnumerator ui_transition(){
        
        Vector3 bdt = bd.transform.position;
        Vector3 original_sp_scale = GameObject.Find("scorepod").transform.localScale;
        Vector3 original_pb_pos = GameObject.Find("progress_bar").GetComponent<RectTransform>().position;
        Vector3 original_hex_scale = bd.hexagons[0,0].transform.localScale;
        Vector3 original_panel_scale = GameObject.Find("panel").GetComponent<RectTransform>().localScale;
        GameObject.Find("scorepod").transform.DOScale(Vector3.zero,0.4f).SetEase(Ease.InQuad);
        GameObject.Find("progress_bar").GetComponent<RectTransform>().DOMove(original_pb_pos+20f*Vector3.down,0.7f).SetEase(Ease.InQuad);
        GameObject.Find("panel").GetComponent<RectTransform>().DOScale(Vector3.zero,0.5f);
        bd.transform.DOMoveX(-3.97f,0.7f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MusicManager.instance.fading("classic_music",0.6f));
        AudioManager.instance.Play("lock_start");
        StartCoroutine(hex_void(new Vector2Int(bd.boardWidth/2,bd.boardWidth/2),4.0f));

        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.Play("trans_ig");
        AudioManager.instance.Play("trans_loop");
        //GameObject.Find("scorepod").transform.DOScale(Vector3.zero,0.4f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(bd.fill_up());
        
        for (int i = 0;i<bd.boardWidth;i++){
            for (int j = 0; j<bd.boardHeight;j++){
                bd.hexagons[i,j].transform.localScale = Vector3.zero;
            }
        }
        yield return new WaitForSeconds(2.5f);
        AudioManager.instance.Play("trans_arrival");
        AudioManager.instance.Stop("trans_loop");
        StartCoroutine(hex_up(new Vector2Int(bd.boardWidth/2,bd.boardWidth/2),original_hex_scale));
        yield return new WaitForSeconds(1f);
        GameObject.Find("scorepod").transform.DOScale(original_sp_scale,0.4f).SetEase(Ease.OutQuad);
        GameObject.Find("progress_bar").GetComponent<RectTransform>().DOMove(original_pb_pos,0.7f).SetEase(Ease.OutQuad);
        GameObject.Find("panel").GetComponent<RectTransform>().DOScale(original_panel_scale,0.5f);
        bd.transform.DOMoveX(bdt.x,0.7f).SetEase(Ease.InOutQuad);

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
    List<Vector2Int> h_pos = new List<Vector2Int>();
    IEnumerator hex_void(Vector2Int pos, float power){
        if ((!h_pos.Contains(pos))&&(pos.magnitude<=15)&&bd.is_legit(pos)){
            //Debug.Log("cc");
            h_pos.Add(pos);
            StartCoroutine(bd.hexagons[pos.x,pos.y].GetComponent<Hexa>().jump_in_void(power));
            bd.hexagon_slots[pos.x,pos.y].transform.DOMove(Vector3.zero,0.5f).SetEase(Ease.InOutQuad).SetDelay(0.6f);
            yield return new WaitForSeconds(0.15f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(hex_void(pos + new Vector2Int(i,j),power-0.5f));
                    }
                }
            }
            yield return new WaitForSeconds(3.0f);
            h_pos.Remove(pos);
        }
    }

    IEnumerator hex_up(Vector2Int pos, Vector3 scaling){
        if ((!h_pos.Contains(pos))&&(pos.magnitude<=15)&&bd.is_legit(pos)){
            //Debug.Log("cc");
            h_pos.Add(pos);
            bd.hexagons[pos.x,pos.y].transform.DOScale(scaling,0.5f).SetEase(Ease.InOutCubic).SetDelay(0.3f);
            bd.hexagon_slots[pos.x,pos.y].transform.DOLocalMove(bd.hexagons[pos.x,pos.y].transform.localPosition,0.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.33f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(hex_up(pos + new Vector2Int(i,j),scaling));
                    }
                }
            }
            yield return new WaitForSeconds(3.0f);
            h_pos.Remove(pos);
        }
    }


    IEnumerator hexa_transition(){

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator holing(Vector2Int pos){
        GameObject effect;
        if ((!hole_pos.Contains(pos))&&(pos.magnitude<=15)){
            effect = Instantiate(hex_mask,grid_to_pix(pos.x,pos.y),Quaternion.identity);
            hole_pos.Add(pos);
            effect.transform.localScale = Vector3.zero;
            effect.transform.DOScale(Vector3.one*0.5f,0.5f);
            yield return new WaitForSeconds(0.2f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(holing(pos + new Vector2Int(i,j)));
                    }
                }
            }
            yield return new WaitForSeconds(2.5f);
            effect.GetComponentInChildren<SpriteRenderer>().DOFade(0.0f,0.3f);
            yield return new WaitForSeconds(1.5f);
            hole_pos.Remove(pos);
            Destroy(effect);

        }
            
        

    }
    





    void maskobjout(GameObject msk){
        foreach (var i in msk.GetComponentsInChildren<SpriteRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            Debug.Log('h');
        }
        foreach (var i in msk.GetComponentsInChildren<ParticleSystemRenderer>()){
            i.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            Debug.Log('k');
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
        yield return new WaitUntil(() => (bd.idle&&score>=GameObject.Find("progress_bar").GetComponent<Slider>().maxValue));
        bd.idle = false;
        bd.drag_list.Clear();
        yield return new WaitUntil(() => (score==display_score));

        AudioManager.instance.Play("multiup");
        StartCoroutine(level_up());
        //StartCoroutine(level_process());
    }
    public void hint()
    {
        Debug.Log(11);
        Slider sl = GameObject.Find("hint").GetComponent<Slider>();
        if (sl.value>=sl.maxValue){
            StartCoroutine(hinting());
            sl.value = 0;
        }
    }
    public IEnumerator hinting(){
        if (bd.idle){
            GameObject hint_hex = bd.get_hint();
            GameObject.Find("hint_pos").transform.position = hint_hex.transform.position;
            GameObject.Find("hint_pos").GetComponent<SpriteRenderer>().DOFade(1.0f,0.3f);
            
            yield return new WaitForSeconds(2.0f);
            GameObject.Find("hint_pos").GetComponent<SpriteRenderer>().DOFade(0.0f,0.3f);
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

        if (display_score < score){
            display_score += Mathf.RoundToInt(10.0f);
        }
        
        else
            display_score = score;
        GameObject.Find("scorepod").transform.Find("score_text").gameObject.GetComponent<TextMeshPro>().text = display_score.ToString("N0");
        GameObject.Find("scorepod").transform.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = level.ToString();
        GameObject.Find("progress_bar").GetComponent<Slider>().value = display_score;
        GameObject.Find("hint").GetComponent<Slider>().value+=Time.deltaTime*0.1f;
    }
}
