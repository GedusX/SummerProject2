using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    // Start is called before the first frame update
    public float time_to_end;
    void Start()
    {
        StartCoroutine(delay_to_destroy(time_to_end));
    }
    IEnumerator delay_to_destroy(float clock){
        yield return new WaitForSeconds(clock);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
