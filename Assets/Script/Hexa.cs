using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum Hexa_Type{NORMAL, BOMB, LINE, WILD, NOVA, NONE};
public enum STATE{FREE, LOCKED};
public class Hexa : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> color_avail;
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
        hexa_diming = false;
    }
    void Start()
    {
        
        color_random();
    }
    public void color_random(){
        color = color_avail[Mathf.RoundToInt(Random.Range(0,color_avail.Count))];
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
        transform.Find("bomb").gameObject.GetComponent<SpriteRenderer>().sprite = bomb_avail[color_avail.IndexOf(color)];
    }
    public void create_hex(){
        gameObject.GetComponent<Animator>().SetTrigger("create");
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
    void Update()
    {
        if (type==Hexa_Type.WILD){
            color = null;
        }
        //get_all_surrounded();
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
        transform.Find("bomb").gameObject.SetActive(type==Hexa_Type.BOMB);
        transform.Find("laser").gameObject.SetActive(type==Hexa_Type.LINE);
        transform.Find("color_bomb").gameObject.SetActive(type==Hexa_Type.WILD);
        transform.Find("color_bomb").Find("rainbow").Rotate(new Vector3(0,0,10f*Time.deltaTime));
        transform.Find("hexa_choosing").gameObject.SetActive((type != Hexa_Type.NONE)&&(!hexa_diming)&&(transform.parent.GetComponent<board>().drag_list.Contains(gameObject)));
        transform.Find("texture").gameObject.SetActive((type != Hexa_Type.NONE)&&(type!= Hexa_Type.WILD));
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
        if (GetComponentInParent<board>().idle){
            //Debug.Log(get_all_surrounded().Count);
            transform.parent.gameObject.GetComponent<board>().checking(gameObject);
            //Debug.Log(transform.position);
        }
            
    }
    public void dimming(){
        hexa_diming = true;
        if (lock_ins){
            Destroy(lock_ins);
            lock_ins = null;
        }
        gameObject.GetComponent<Animator>().SetTrigger("dim");

        GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(deleting());
    }
    public GameObject lock_ins = null;
    public void lock_create(int index = 3){
        
        lock_ins = Instantiate(lock_prefab,transform.position,Quaternion.identity);
        lock_ins.transform.SetParent(transform);
        lock_ins.GetComponent<HexLock>().lock_start(index + ((Handler.level<4)?(4-Handler.level):0));
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
        for (int i = 0;i<transform.childCount-1;i++){
            transform.GetChild(i).gameObject.SetActive(false);
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
}
