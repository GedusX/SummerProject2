using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class BlitzHandler : Handler
{
    // Start is called before the first frame update
    //public List<GameObject> holding;
    
    public float display_score;
    
    //public int difficulty;
    private GameObject background;
    private GameObject background_old;
    
    
    private board bd;

    public static BlitzStat stat;

    [SerializeField] private List<GameObject> bgavail;

    [SerializeField] private GameObject hex_mask;
    [SerializeField] private int time;
    
    public int milestone(int lvl){
        int base_score = 30;
        float increase_factor = 1.4f;
        return Mathf.RoundToInt(base_score * Mathf.Pow(increase_factor,(lvl-1)));
    }
    private void Awake() {
        //instance = new ClassicHandler();
        if (stat!= null){
            Destroy(stat);
            stat = null;
        }
        stat = new BlitzStat();
    }
    public GameObject text_module;
    Sequence score_seq;
    public override void update_score(int sc){
        //Debug.Log(this.multiplier);
        score += Mathf.RoundToInt(sc*multiplier);
        stat.score_timeline[Mathf.FloorToInt(GameObject.Find("timer").GetComponent<Slider>().value)/30] += Mathf.RoundToInt(sc*multiplier);
        //DOTween.To(() => valFloat, x => valFloat = x, 0f, 1f);
        //DOVirtual.Int()
        //display_score = score;
        //Debug.Log(display_score);

        DOTween.To(() => display_score, x => display_score = x, score, 0.3f).SetEase(Ease.InOutQuad);
        
        DOTween.To(() => GameObject.Find("progress_bar").GetComponent<Slider>().value, x => GameObject.Find("progress_bar").GetComponent<Slider>().value = x, score, 0.3f).SetEase(Ease.InOutQuad);

    }

    public IEnumerator no_more_moves(){
        stat.score = score;
        stat.multiplier = level;

        StartCoroutine(MusicManager.instance.fading("classic_music2",0.6f));
        yield return new WaitForSeconds(0.6f);
        MusicManager.instance.Play("gameover");
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting2("TIME UP"));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SceneLoader.instance.transition("Scenes/Gameover_Blitz",Vector2.zero));
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
        for (int i = 0; i<time/30;i++){
            stat.score_timeline.Add(0);
        }
        
        background = Instantiate(bgavail[Random.Range(0,bgavail.Count)],Vector3.zero,Quaternion.identity);

        level_standby();
        

    }

    void level_standby(){
        
        //Debug.Log(multiplier);
        
        
        GameObject.Find("timer").GetComponent<Slider>().maxValue = time;
        GameObject.Find("timer").GetComponent<Slider>().minValue = 0;
        GameObject.Find("timer").GetComponent<Slider>().value = 0;
        GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().maxValue = milestone(level);
        GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().minValue = stat.num_of_hex;
        //difficulty = Mathf.RoundToInt(100 * (1 - Mathf.Pow(0.55f,level)));
        //difficulty = Mathf.CeilToInt(100 * (1 - Mathf.Exp(-0.5f * level)));
        
        bd.idle = true;
        StartCoroutine(level_process());
    }

        IEnumerator ui_transition(){
        
        Vector3 bdt = bd.transform.position;
        Vector3 original_sp_scale = GameObject.Find("scorepod").transform.localScale;
        Vector3 original_pb_pos = GameObject.Find("progress_bar").GetComponent<RectTransform>().position;
        Vector3 original_hex_scale = bd.hexagons[0,0].transform.localScale;
        Vector3 original_panel_scale = GameObject.Find("panel").GetComponent<RectTransform>().localScale;
        Vector3 original_slot_scale = bd.hexagon_slots[0,0].transform.localScale;
        
        GameObject.Find("scorepod").transform.DOScale(Vector3.zero,0.4f).SetEase(Ease.InQuad);
        GameObject.Find("progress_bar").GetComponent<RectTransform>().DOMove(original_pb_pos+20f*Vector3.down,0.7f).SetEase(Ease.InQuad);
        GameObject.Find("panel").GetComponent<RectTransform>().DOScale(Vector3.zero,0.5f);


        bd.transform.DOMoveX((bd.transform.position - bd.hexagon_slots[bd.boardWidth/2,bd.boardHeight/2].transform.localPosition).x,0.7f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MusicManager.instance.fading("classic_music",0.6f));
        AudioManager.instance.Play("lock_start");
        StartCoroutine(hex_void(new Vector2Int(bd.boardWidth/2,bd.boardWidth/2),3.0f));
        GameObject.Find("Main Camera").GetComponent<Camera>().DOOrthoSize(6.5f,0.7f).SetEase(Ease.OutQuad);
        GameObject.Find("Main Camera").GetComponent<Camera>().DOOrthoSize(2f,0.3f).SetDelay(0.8f);
        GameObject.Find("Main Camera").GetComponent<Camera>().DOOrthoSize(5f,0.1f).SetDelay(1.05f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(holing(Vector2Int.zero));
        AudioManager.instance.Play("explode");
        AudioManager.instance.Play("trans_ig");
        AudioManager.instance.Play("trans_loop");
        
        //GameObject.Find("scorepod").transform.DOScale(Vector3.zero,0.4f).SetEase(Ease.InQuad);
        yield return new WaitUntil(()=> bd.num_of_hex()<1);
        StartCoroutine(bd.fill_up());
        
        for (int i = 0;i<bd.boardWidth;i++){
            for (int j = 0; j<bd.boardHeight;j++){
                bd.hexagons[i,j].transform.localScale = Vector3.zero;
                //bd.hexagon_slots[i,j].transform.localScale = Vector3.one*10f;
                
            }
        }

        yield return new WaitForSeconds(1.75f);
        for (int i = 0;i<bd.boardWidth;i++){
            for (int j = 0; j<bd.boardHeight;j++){
                bd.hexagon_slots[i,j].transform.DOScale(original_slot_scale,1f).SetEase(Ease.InOutQuad).From(Vector3.one*10f);
                
            }
        }
        AudioManager.instance.Play("trans_arrival");
        


        yield return new WaitForSeconds(1f);
        AudioManager.instance.Play("trans_preig");
        AudioManager.instance.Stop("trans_loop");

        StartCoroutine(hex_up(new Vector2Int(bd.boardWidth/2,bd.boardWidth/2),original_hex_scale));
        yield return new WaitUntil(()=> (h_pos.Count == 0));


        GameObject.Find("scorepod").transform.DOScale(original_sp_scale,0.4f).SetEase(Ease.OutQuad);
        GameObject.Find("progress_bar").GetComponent<RectTransform>().DOMove(original_pb_pos,0.7f).SetEase(Ease.OutQuad);
        GameObject.Find("panel").GetComponent<RectTransform>().DOScale(original_panel_scale,0.5f);
        bd.transform.DOMoveX(bdt.x,0.7f).SetEase(Ease.InOutQuad);
        level_standby();
        StartCoroutine(MusicManager.instance.fadin("classic_music",0.6f));
        
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("LEVEL " + level.ToString()));

    }
    List<Vector2Int> hole_pos = new List<Vector2Int>();
    bool is_next_to(Vector2Int pos1, Vector2Int pos2){
        //Debug.Log((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude);
        return (((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude)<=1*1.1f);

    }
    Vector2 grid_to_pix(int i, int j, float scale = 1f){
        float xOffset = 0.8743f * scale;
        float yOffset = 0.4722f * scale;
        float x = i * xOffset;
        float y = j * 0.9425f * scale + (i % 2) * yOffset;
        return transform.position + new Vector3(x,y);
    }
    List<Vector2Int> h_pos = new List<Vector2Int>();
    IEnumerator hex_void(Vector2Int pos, float power){
        if ((!h_pos.Contains(pos))&&(pos.magnitude<=15)&&bd.is_legit(pos)){
            //Debug.Log("cc");
            
            h_pos.Add(pos);
            StartCoroutine(bd.hexagons[pos.x,pos.y].GetComponent<Hexa>().jump_in_void(power));
            
            bd.hexagon_slots[pos.x,pos.y].transform.DOMove(Vector3.zero,0.4f).SetEase(Ease.InOutQuad).SetDelay(0.65f);
            yield return new WaitForSeconds(0.2f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(hex_void(pos + new Vector2Int(i,j),power-0.05f));
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            bd.hexagon_slots[pos.x,pos.y].transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InOutQuad).SetDelay(0.1f);
            if (pos == Vector2Int.zero){
                StartCoroutine(holing(Vector2Int.zero));
                //GameObject.Find("warp1").GetComponent<ParticleSystem>().Play();
                //GameObject.Find("warp2").GetComponent<ParticleSystem>().Play();
                //GameObject.Find("warp3").GetComponent<ParticleSystem>().Play();
            }

            h_pos.Remove(pos);
        }
    }

    IEnumerator hex_up(Vector2Int pos, Vector3 scaling){
        if ((!h_pos.Contains(pos))&&(pos.magnitude<=15)&&bd.is_legit(pos)){
            //Debug.Log("cc");
            h_pos.Add(pos);
            //bd.hexagons[pos.x,pos.y].transform.DOScale(scaling,0.5f).SetEase(Ease.InOutCubic).SetDelay(0.5f);
            bd.hexagon_slots[pos.x,pos.y].transform.DOLocalMove(bd.hexagons[pos.x,pos.y].transform.localPosition,0.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.33f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(hex_up(pos + new Vector2Int(i,j),scaling));
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            h_pos.Remove(pos);
        }
    }


    IEnumerator hexa_transition(){

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator holing(Vector2Int pos){
        GameObject effect;
        if ((!hole_pos.Contains(pos))&&(pos.magnitude<=10)){
            effect = Instantiate(hex_mask,grid_to_pix(pos.x,pos.y,1.25f),Quaternion.identity);
            hole_pos.Add(pos);
            effect.transform.localScale = Vector3.zero;
            effect.transform.DOScale(Vector3.one*0.5f*1.25f,0.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.11f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(holing(pos + new Vector2Int(i,j)));
                    }
                }
            }
            //yield return new WaitForSeconds(2.5f);
            foreach (var k in effect.GetComponentsInChildren<SpriteRenderer>()){
                k.DOFade(0.0f,0.3f).SetEase(Ease.InOutQuad).SetEase(Ease.InOutQuad).SetDelay(2f);
            }
            //effect.transform.DOScale(Vector3.zero,0.3f).SetDelay(2.5f);
            //effect.transform.DOMove(Vector3.zero,0.3f).SetDelay(2.5f);
            
            
            yield return new WaitForSeconds(4.5f);
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
    public override void update_hex(int nor, int bomb, int laser, int void_hex){
        stat.num_of_bomb+=bomb;
        stat.num_of_laser+=laser;
        stat.num_of_void += void_hex;
        stat.num_of_hex+=nor + bomb + laser + void_hex;
        GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().value = stat.num_of_hex;
        if ( GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().value>= GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().maxValue){
            AudioManager.instance.Play("multiup");
            level++;
            GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().maxValue = stat.num_of_hex+milestone(level);
            GameObject.Find("Canvas").transform.Find("scorepod").gameObject.GetComponentInChildren<Slider>().minValue = stat.num_of_hex;
            multiplier = level;
            difficulty = Mathf.RoundToInt(100 * (1 - Mathf.Pow(0.4f,level)));
            
        }
    }
    //Sequence TimeSeq = DOTween.Sequence();
    IEnumerator level_process(){
        yield return new WaitForSeconds(0.6f);
        DOTween.To(() => GameObject.Find("timer").GetComponent<Slider>().value, x => GameObject.Find("timer").GetComponent<Slider>().value = x, time, 1.8f).SetEase(Ease.InOutQuad);
        AudioManager.instance.Play("ticktock");
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(bd.fill_up());
        yield return new WaitForSeconds(1.3f);
        
        DOTween.To(() => GameObject.Find("timer").GetComponent<Slider>().value, x => GameObject.Find("timer").GetComponent<Slider>().value = x, 0, time).SetEase(Ease.Linear);
     
        GameObject t = Instantiate(text_module,Vector3.zero, Quaternion.identity);
        t.transform.SetParent(GameObject.Find("Canvas").transform);
        t.GetComponent<RectTransform>().localScale = Vector3.one;
        StartCoroutine(t.GetComponent<bigtext>().texting("GO!"));
        MusicManager.instance.Play("classic_music2");
        while (true){
            yield return new WaitUntil(() => (bd.idle));

            if (GameObject.Find("timer").GetComponent<Slider>().value <=0){
                bd.idle = false;
                yield return new WaitForSeconds(0.6f);
                StartCoroutine(no_more_moves());
                break;
            }
        }
        //yield return new WaitUntil(() => (bd.idle&&score>=GameObject.Find("progress_bar").GetComponent<Slider>().maxValue));
        
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
    public void mainmenu(){
        if (bd.idle == true){
            SceneManager.LoadScene("Scenes/PauseMenu",LoadSceneMode.Additive);
            StartCoroutine(pausing());
        }
        
    }
    
    // Update is called once per frame
    public IEnumerator pausing(){
        Debug.Log("op1");
        GameObject canvas = GameObject.Find("panel");
        PauseMenu.previous_scene = "Scenes/Blitz";
        //TimeSeq.TogglePause();
        Time.timeScale = 0;
        GameObject bdg = bd.gameObject;
        canvas.SetActive(false);
        bdg.SetActive(false);
        MusicManager.instance.Pause("classic_music2");
        MusicManager.instance.Play("gameover");
        yield return new WaitUntil(()=>(!SceneManager.GetSceneByName("PauseMenu").IsValid()));
        Debug.Log("op2");
        //TimeSeq;
        Time.timeScale = 1;
        MusicManager.instance.Stop("gameover");
        MusicManager.instance.Continue("classic_music2");
        canvas.SetActive(true);
        bdg.SetActive(true);

    }
    // Update is called once per frame
    
    void Update()
    {
        multiplier = Handler.level;
        //Debug.Log(stat.score_timeline[0].ToString()+","+stat.score_timeline[1].ToString()+","+stat.score_timeline[2].ToString()+","+stat.score_timeline[3].ToString()+","+ stat.score_timeline[4].ToString()+","+stat.score_timeline[5].ToString());
        if (Input.GetKeyDown(KeyCode.Escape)){
            mainmenu();
        }
        if (display_score < score){
            
        }
        
        else
            display_score = score;
        
        var l = new Vector3(66.5f,0,0);
        var k = new Vector3(-66.4199982f,0,0);
        GameObject.Find("timer").transform.Find("slider").transform.localPosition = k + (GameObject.Find("timer").GetComponent<Slider>().value/GameObject.Find("timer").GetComponent<Slider>().maxValue)*(l-k);
        int minutes = Mathf.FloorToInt(GameObject.Find("timer").GetComponent<Slider>().value / 60F);
        int seconds = Mathf.FloorToInt(GameObject.Find("timer").GetComponent<Slider>().value - minutes * 60);
        GameObject.Find("timer").transform.Find("slider").gameObject.GetComponentInChildren<TextMeshPro>().text = string.Format("{0:0}:{1:00}", minutes, seconds);
        GameObject.Find("scorepod").transform.Find("score_text").gameObject.GetComponent<TextMeshPro>().text = Mathf.FloorToInt(display_score).ToString("N0");
        GameObject.Find("scorepod").transform.Find("level_text").gameObject.GetComponent<TextMeshPro>().text = ((char) 215) + level.ToString();
        if (GameObject.Find("hint")!=null)
            GameObject.Find("hint").GetComponent<Slider>().value+=Time.deltaTime*0.1f;
        
    }
}
