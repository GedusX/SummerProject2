using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HexLock : MonoBehaviour
{
    // Start is called before the first frame update
    int count;

    void Start()
    {
        
    }
    public void lock_start(int index){
        count = index;
        AudioManager.instance.Play("lock_appear");
    }

    public void lock_counter(){
        if (count>0){
            count--;
        }
    }
    public void lock_check(){
        if (count == 0){
            StartCoroutine(locked());
        }
    }
    public IEnumerator locked(){
        GetComponentInParent<Hexa>().status = STATE.LOCKED;
        GetComponent<Animator>().SetTrigger("lock");
        AudioManager.instance.Play("lock_start");
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.Play("lock_end");


    }
    public IEnumerator unlocked(){
        GetComponentInParent<Hexa>().status = STATE.FREE;
        GetComponentInParent<Hexa>().lock_ins = null;
        GetComponent<Animator>().SetTrigger("destroyed");
        AudioManager.instance.Play("lock_destroy");
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<TextMeshPro>().text = count.ToString();
    }
}
