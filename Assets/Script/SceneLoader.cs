using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public static SceneLoader instance;
    [SerializeField] private GameObject hex_hole;
    List<Vector2Int> hole_pos = new List<Vector2Int>();

    Vector2 pos_wanting;
    void Awake(){
        if (instance != null)
		{
			Destroy(gameObject);
			return;
		} else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
    }
    void Start()
    {

    }
    Vector2 grid_to_pix(int i, int j){
        float xOffset = 0.8743f;
        float yOffset = 0.4722f;
        float x = i * xOffset;
        float y = j * 0.9425f + (i % 2) * yOffset;
        return new Vector2(x,y);
    }
    bool is_next_to(Vector2Int pos1, Vector2Int pos2){
        //Debug.Log((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude);
        return (((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude)<=1*1.1f);

    }
    IEnumerator holing(Vector2Int pos){
        GameObject effect;
        if ((!hole_pos.Contains(pos))&&(pos.magnitude<=15)){
            effect = Instantiate(hex_hole,grid_to_pix(pos.x,pos.y),Quaternion.identity);
            effect.transform.localPosition = grid_to_pix(pos.x,pos.y);
            effect.transform.SetParent(gameObject.transform);
            hole_pos.Add(pos);
            effect.transform.localScale = Vector3.zero;
            effect.transform.DOScale(Vector3.one*0.5f,0.2f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.07f);
            for (int i = -1; i<=1;i++){
                for (int j = -1; j<=1;j++){
                    if (is_next_to(pos, pos + new Vector2Int(i,j))){
                        StartCoroutine(holing(pos + new Vector2Int(i,j)));
                    }
                }
            }
            yield return new WaitForSeconds(0.75f);
            hole_pos.Remove(pos);
            yield return new WaitUntil(() => asyncLoadLevel.isDone);
            foreach (var k in effect.GetComponentsInChildren<SpriteRenderer>())
                k.DOFade(0.0f,0.3f);
            //effect.transform.DOScale(Vector3.zero*0.5f,0.15f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.3f);
            
            Destroy(effect);

        }
            
        

    }
    private AsyncOperation asyncLoadLevel;
    
    public IEnumerator transition(string scene, Vector2 pos){
        pos_wanting = pos;
        
        StartCoroutine(holing(Vector2Int.zero));
        yield return new WaitForSeconds(0.7f);
        asyncLoadLevel = SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
