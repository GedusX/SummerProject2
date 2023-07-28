using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum Hexa_Type{NORMAL, BOMB, LINE, WILD, NOVA, NONE};
public class Hexa : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> color_avail;
    public Sprite color;

    public Vector2Int pos;

    public Hexa_Type type = Hexa_Type.NORMAL;

    public bool isChoosing;

    public GameObject choosing_spr;
    void Start()
    {
        
        color_random();
    }
    public void color_random(){
        color = color_avail[Mathf.RoundToInt(Random.Range(0,color_avail.Count))];
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = color;
    }
    public void create_hex(){
        gameObject.GetComponent<Animator>().SetTrigger("create");
    }
    public void change_to(Hexa_Type type){
        this.type = type;
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
        if (GetComponentInParent<board>().legal_move){
            transform.Find("hexa_choosing").gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        }
        else{
            transform.Find("hexa_choosing").gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }

    }
    bool hexa_diming = false;
    private void OnMouseOver() {
        if (Input.GetMouseButton(0)&&!hexa_diming&&GetComponentInParent<board>().idle){
            transform.parent.gameObject.GetComponent<board>().checking(gameObject);
            //Debug.Log(transform.position);
        }
            
    }
    public void dimming(){
        hexa_diming = true;
        gameObject.GetComponent<Animator>().SetTrigger("dim");
        GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(deleting());
    }

    IEnumerator deleting(){
        yield return new WaitForSeconds(0.0f);
        GetComponentInParent<board>().hexagons[pos.x,pos.y] = null;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    IEnumerator go_path(){
        int ind = GetComponentInParent<board>().drag_list.IndexOf(gameObject);
        if (ind > 0){
            dimming();
            yield return new WaitForSeconds(0.025f);   
        }
    }
    public void combining(){
        StartCoroutine(go_path());   
    }
    IEnumerator wait_for_delete(){
        yield return new WaitForSeconds(0.01f);
        GetComponentInParent<board>().hexagons[pos.x,pos.y] = null;
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);

    }
    public void explode(){
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

}
