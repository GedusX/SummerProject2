using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;



public class Score_text : MonoBehaviour
{
    // Start is called before the first frame update
    public static Color str_to_color(string s){
    if (s == "orange"){
        return new Color(1f, 0.647f,0f);
    }
    else if (s == "blue"){
        return Color.blue;
    }
    else if (s == "red"){
        return Color.red;
    }
    else if (s == "yellow"){
        return Color.yellow;
    }
    else if (s == "purple"){
        return new Color(0.5f,0f,0.5f);
    }
    else if (s == "green"){
        return Color.green;
    }
    else if (s == "white"){
        return Color.white;
    }
    else if (s== "transparent"){
        return new Color(1f,1f,1f,0f);
    }
    return Color.black;
}
    void Start()
    {
        dis = DOTween.Sequence();
        
        
    }
    public int points_got = 0;
    Sequence dis;
    IEnumerator text(){
        transform.localScale = Vector3.one*1.7f;
        transform.DOScale(Vector3.one,0.2f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(1f);
        //GetComponent<Animator>().SetTrigger("disappear");
        
        dis.Insert(0.0f,transform.DOScale(Vector3.zero,0.5f).SetEase(Ease.OutQuad));
        dis.Insert(0.0f,transform.Find("score_text_fill").gameObject.GetComponent<TextMeshPro>().DOFade(0f,0.3f).SetEase(Ease.InQuad));
        dis.Insert(0.0f,transform.Find("score_text_border").gameObject.GetComponent<TextMeshPro>().DOFade(0f,0.3f).SetEase(Ease.InQuad));
        dis.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    public void update_points(int point){
        points_got+=point;
        StopAllCoroutines();
        StartCoroutine(text());
    }
    public void set_to_color(string col){
        transform.Find("score_text_fill").gameObject.GetComponent<TextMeshPro>().color = str_to_color(col);
        if (col != "transparent")
            transform.Find("score_text_fill").gameObject.GetComponent<TextMeshPro>().alpha = 0.5f;
    }   
    // Update is called once per frame
    void Update()
    {
        if (points_got<=100*Handler.multiplier){
            transform.Find("score_text_fill").gameObject.GetComponent<RectTransform>().localScale = 0.125f*Vector3.one;
            transform.Find("score_text_border").gameObject.GetComponent<RectTransform>().localScale = 0.125f*Vector3.one;
        }
        else{
            transform.Find("score_text_fill").gameObject.GetComponent<RectTransform>().localScale = (0.01f*((points_got-100)/(50f*Handler.multiplier))+0.125f)*Vector3.one;
            transform.Find("score_text_border").gameObject.GetComponent<RectTransform>().localScale = (0.01f*((points_got-100)/(50f*Handler.multiplier))+0.125f)*Vector3.one;
        }
        
        foreach (var k in gameObject.GetComponentsInChildren<TextMeshPro>()){
            k.text = points_got.ToString();
        }
        if (transform.position.y<=4.25){
            transform.Translate(new Vector3(0,Time.deltaTime*0.4f,0));
        }
    }
}
