using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum Hexa_Type{NORMAL, BOMB, LINE, WILD, LINK ,NONE};
public enum STATE{FREE, LOCKED};

public class Hexa : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> color_avail;
    [SerializeField] private List<GameObject> effects;
    private GameObject main_effect;
    public List<Sprite> bomb_avail;
    public Sprite color;

    public Vector2Int pos;

    public Hexa_Type type;

    public bool isChoosing;

    public GameObject choosing_spr;

    public STATE status;
    bool hexa_diming;

    public GameObject lock_prefab;
    private void Awake() {
        status = STATE.FREE;
        type = Hexa_Type.NORMAL;
        old_type = type;
        hexa_diming = false;
        
    }
    Vector3 old_scale;
    void Start()
    {
        
        //gameObject.GetComponent<Animator>().SetTrigger("start");
        old_scale = transform.localScale;
        transform.Find("texture").localScale = Vector3.zero;
        //gameObject.transform.localScale = Vector3.zero;
        //transform.localScale = Vector3.zero;
        color_random();
        
        //change_to(Hexa_Type.LINK);
    }
    public void color_random(){
        color = color_avail[Mathf.RoundToInt(Random.Range(0,color_avail.Count))];
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
        //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
    }
    public void create_hex(){
        if (type == Hexa_Type.NORMAL){
           
            //transform.localScale = old_scale;
            SpriteRenderer spr = transform.Find("texture").gameObject.GetComponent<SpriteRenderer>();
            
            spr.DOFade(1.0f,0.5f).From(0.0f);
            transform.Find("texture").DOScale(Vector3.one,0.5f).From(Vector3.zero).SetEase(Ease.OutQuad);
        }
        else if (type == Hexa_Type.LINK)
            {
                transform.Find("texture").localScale = Vector3.one;
                transform.DOScale(old_scale,0.15f).From(Vector3.zero).SetEase(Ease.InOutQuad);
            }
        else{
            transform.Find("texture").localScale = Vector3.one;
        }
            
    }
    public void change_to(Hexa_Type type){
        this.type = type;
        if (lock_ins){
            Destroy(lock_ins);
            lock_ins = null;
        }
        transform.Find("hexshock").GetComponent<ParticleSystem>().Play();
    }
    // Update is called once per frame
    Hexa_Type old_type;

    void TypeChange(){
        if (old_type!=type){
            if (main_effect!=null){
                Destroy(main_effect);
                main_effect = null;
            }
            if (type == Hexa_Type.BOMB){
            main_effect = Instantiate(effects[0],transform.position,Quaternion.identity);
            main_effect.transform.parent = transform;
            main_effect.transform.localPosition = Vector3.zero;
            main_effect.GetComponent<SpriteRenderer>().sprite = bomb_avail[color_avail.IndexOf(color)];
            main_effect.transform.localScale = Vector3.one * 0.78f;

        }
        else if (type == Hexa_Type.LINE){
            main_effect = Instantiate(effects[1],transform.position,Quaternion.identity);
            main_effect.transform.parent = transform;
            main_effect.transform.localPosition = Vector3.zero;
            main_effect.transform.localScale = Vector3.one * 2f;
        }
        else if (type == Hexa_Type.WILD){  
            main_effect = Instantiate(effects[2],transform.position,Quaternion.identity);
            main_effect.transform.parent = transform;
            main_effect.transform.localPosition = Vector3.zero;
            main_effect.transform.localScale = Vector3.one * 1f;
        }
        else if (type == Hexa_Type.LINK){
            main_effect = Instantiate(effects[3],transform.position,Quaternion.identity);
            main_effect.transform.parent = transform;
            main_effect.transform.localPosition = Vector3.zero;
            main_effect.transform.localScale = Vector3.one * 1f;
            color = null;
        }
        }
        old_type = type;
    }
    void Update()
    {
        if (type==Hexa_Type.WILD){
            color = null;
        }
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
        //get_all_surrounded();
        

        
        transform.Find("hexa_choosing").gameObject.SetActive((type != Hexa_Type.NONE)&&(!hexa_diming)&&(transform.parent.GetComponent<board>().drag_list.Contains(gameObject)));
        transform.Find("texture").gameObject.SetActive((type != Hexa_Type.NONE)&&(type!= Hexa_Type.WILD)&& (type!=Hexa_Type.LINK));
        TypeChange();
        if (type == Hexa_Type.WILD)
            main_effect.transform.Find("rainbow").Rotate(new Vector3(0,0,10f*Time.deltaTime));
        if (GetComponentInParent<board>().drag_list.Count>0)
            if (GetComponentInParent<board>().legal_move){
                transform.Find("hexa_choosing").gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            }
            else{
                transform.Find("hexa_choosing").gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }

    }
    
    public void ClickOn() {
        if (!GetComponentInParent<board>().is_legit(pos))
            return;
        if (status==STATE.LOCKED){
            StartCoroutine(shaking());
            return;
        }
        if (type == Hexa_Type.LINK)
            return;
        if (GetComponentInParent<board>().idle){
            //Debug.Log(get_all_surrounded().Count);
            transform.parent.gameObject.GetComponent<board>().checking(gameObject);
            //Debug.Log(transform.position);
        }
            
    }
    public void dimming(){
        hexa_diming = true;
        if (lock_ins&&status == STATE.LOCKED){
            StartCoroutine(lock_ins.GetComponent<HexLock>().unlocked());
        }
        if (lock_ins){
            Destroy(lock_ins);
            lock_ins = null;
        }
        
        
        //Sequence seq = DOTween.Sequence();
         SpriteRenderer spr = transform.Find("texture").gameObject.GetComponent<SpriteRenderer>();
            
        spr.DOFade(0.0f,0.5f);
        transform.Find("texture").DOScale(Vector3.zero,0.35f).From(Vector3.one).SetEase(Ease.InBounce);
        GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(deleting());
    }
    public GameObject lock_ins = null;
    public void lock_create(int index = 3){
        
        lock_ins = Instantiate(lock_prefab,transform.position,Quaternion.identity);
        lock_ins.transform.SetParent(transform);
        lock_ins.GetComponent<HexLock>().lock_start(17-(GameObject.Find("Level_handler").GetComponent<Handler>().difficulty)/10);
        lock_ins.transform.localScale = new Vector3(1,1,1);
    }
    IEnumerator deleting(){
        GetComponentInParent<board>().hexagons[pos.x,pos.y] = null;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    public IEnumerator bursting(){
        Vector3 original_scale = transform.localScale;
        transform.DOScale(original_scale*3.0f,0.2f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.2f);
        transform.DOScale(original_scale,0.2f).SetEase(Ease.OutBounce);
    }
    IEnumerator go_path(List<Vector2> points){
        dimming();
        foreach (Vector2 i in points){
            transform.DOMove(i,0.5f/(GetComponentInParent<board>().drag_list.Count-1)).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.3f/(GetComponentInParent<board>().drag_list.Count-1));
        }
        //yield return new WaitForSeconds(0.3f/(GetComponentInParent<board>().drag_list.Count-1));   
    }
    public void combining(){
        int ind = GetComponentInParent<board>().drag_list.IndexOf(gameObject);
        if (ind > 0){
            List<Vector2> points = new List<Vector2>();
            for (int k = ind-1;k>=0;k--){
                points.Add(GetComponentInParent<board>().grid_to_pix(GetComponentInParent<board>().drag_list[k].GetComponent<Hexa>().pos.x,GetComponentInParent<board>().drag_list[k].GetComponent<Hexa>().pos.y));
            }
            StartCoroutine(go_path(points));
        }
         
    }
    IEnumerator wait_for_delete(){
        yield return new WaitForSeconds(0.01f);
        GetComponentInParent<board>().hexagons[pos.x,pos.y] = null;
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);

    }
    public void explode(){
        if (lock_ins&&status == STATE.LOCKED){
            StartCoroutine(lock_ins.GetComponent<HexLock>().unlocked());
        }
        if (lock_ins){
            Destroy(lock_ins);
            lock_ins = null;
        }
        if (main_effect!=null){
            Destroy(main_effect);
            main_effect = null;
        }
        type = Hexa_Type.NONE;
        transform.Find("explosion").gameObject.SetActive(true);
        foreach (ParticleSystem ps in transform.Find("explosion").gameObject.GetComponentsInChildren<ParticleSystem>()){
            ps.Play();
        }
        StartCoroutine(wait_for_delete());
    }

    public List<GameObject> get_all_surrounded(){
        List<GameObject> res = new List<GameObject>();
        for (int i = -1;i<=1;i++){
            for (int j = -1; j<=1;j++){
                if (GetComponentInParent<board>().is_legit(pos + new Vector2Int(i,j))){
                    if (gameObject == GetComponentInParent<board>().hexagons[pos.x+i,pos.y+j])
                        continue;
                    if (GetComponentInParent<board>().is_next_to(gameObject,GetComponentInParent<board>().hexagons[pos.x+i,pos.y+j])){
                        res.Add(GetComponentInParent<board>().hexagons[pos.x+i,pos.y+j]);
                    }
                }
            }
        }
        //Debug.Log(res.Count);
        return res;
    }
    public IEnumerator knockback(Vector2 direction, float force){
        transform.DOMove(GetComponentInParent<board>().grid_to_pix(pos.x,pos.y)+direction*force,0.15f);
        yield return new WaitForSeconds(0.15f);
        transform.DOMove(GetComponentInParent<board>().grid_to_pix(pos.x,pos.y),0.15f);
        yield return new WaitForSeconds(0.15f);
    }
    public IEnumerator shaking(float force = 0.25f){
        for (int i = 0;i<=Random.Range(0,2);i++){
            transform.DOMove(GetComponentInParent<board>().grid_to_pix(pos.x,pos.y)+new Vector2(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f))*force,0.08f);
            yield return new WaitForSeconds(0.08f);
            transform.DOMove(GetComponentInParent<board>().grid_to_pix(pos.x,pos.y),0.08f);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public IEnumerator jump_in_void(float jumpforce){
        Vector3 pivot = GetComponentInParent<board>().grid_to_pix(GetComponentInParent<board>().boardWidth/2,GetComponentInParent<board>().boardHeight/2);
        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOScale(transform.localScale*(jumpforce + Random.Range(-1.0f, 1.0f)),0.5f).SetEase(Ease.OutQuad));
        sq.Join(transform.DOMove(transform.position + (transform.position - pivot)*(3.0f - jumpforce)*Random.Range(3.0f,10.0f),0.5f).SetEase(Ease.OutQuad));

        //yield return new WaitForSeconds(0.5f);
        
        sq.Append(transform.DOScale(Vector3.zero,0.3f));
        sq.Join(transform.DOMove(Vector3.zero,0.3f).SetEase(Ease.OutQuad));
        
        sq.Play();

        
        yield return new WaitForSeconds(1.0f);
        GetComponentInParent<board>().hexagons[pos.x,pos.y] = null;
        if (!GameObject.Find("Level_handler").GetComponent<Handler>().holding.Contains(gameObject))
            Destroy(gameObject);
    }
}
