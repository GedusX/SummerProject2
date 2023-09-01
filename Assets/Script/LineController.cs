using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lrender;
    [SerializeField] Texture[] textures;
    [SerializeField] Vector2[] points;
    private int ani_step;

    [SerializeField] private float fps = 60f;

    private float fps_counter;

    void Start()
    {
        
    }
    private void Awake() {
        lrender = gameObject.GetComponent<LineRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i<points.Length;i++){
            lrender.SetPosition(i,new Vector3(points[i].x,points[i].y,0));
        }
        fps_counter += Time.deltaTime;
        if (fps_counter>=1f/fps){
            ani_step++;
            if (ani_step >= textures.Length){
                ani_step = 0;
            }
            lrender.material.SetTexture("_MainTex",textures[ani_step]);
            fps_counter = 0;
        }
    }
}
