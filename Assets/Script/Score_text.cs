using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Score_text : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public int points_got = 0;
    IEnumerator text(){
        GetComponent<Animator>().SetTrigger("appear");
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetTrigger("disappear");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    public void update_points(int point){
        points_got+=point;
        StopAllCoroutines();
        StartCoroutine(text());
    }
    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<TextMeshPro>().text = points_got.ToString();
        if (transform.position.y<=4.25){
            transform.Translate(new Vector3(0,Time.deltaTime*0.1f,0));
        }
    }
}
