using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class bigtext : MonoBehaviour
{
    // Start is called before the first frame update
    Sequence seq;
    private void Awake() {
        seq = DOTween.Sequence();
    }
    void Start()
    {
        
        
        
    }
    public IEnumerator texting(string message = ""){

        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        Color orginal_color = transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color;
        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("text").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("border").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

        //transform.Find("text").gameObject.GetComponent<TextMeshPro>()
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(1,1,1),1f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOColor(orginal_color,1f));
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(1,1,1),1f));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1,1f));
        seq.Play();
        yield return new WaitForSeconds(1.1f);
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(10,10,1),1f).SetEase(Ease.InQuad));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,1f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,1f));
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(3,3,1),0.5f).SetEase(Ease.InQuad));
        seq.Play();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        //transform.Find("text").gameObject.GetComponent<TextMeshPro>().DOFade(0,1f);
        
    }

    public IEnumerator texting2(string message = ""){

        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        Color orginal_color = transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color;
        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("text").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("border").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

        //transform.Find("text").gameObject.GetComponent<TextMeshPro>()
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(1,1,1),1f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOColor(orginal_color,1f));
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(1,1,1),1f));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1,1f));
        seq.Play();
        yield return new WaitForSeconds(1f);
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(10,10,1),1f));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,1f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,1f));
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(3,3,1),0.5f));
        seq.Play();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        //transform.Find("text").gameObject.GetComponent<TextMeshPro>().DOFade(0,1f);
        
    }
    public IEnumerator texting3(string message = ""){

        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().text = message;
        Color orginal_color = transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color;
        transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("text").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
        transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().color *= new Color(1,1,1,0);
        transform.Find("border").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

        //transform.Find("text").gameObject.GetComponent<TextMeshPro>()
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(1.5f,1.5f,1),1f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOColor(orginal_color,1f));
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(1.5f,1.5f,1),1f));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(1,1f));
        seq.Play();
        yield return new WaitForSeconds(2.7f);
        seq.Insert(0.0f,transform.Find("border").DOScale(new Vector3(10,10,1),0.9f));
        seq.Insert(0.0f,transform.Find("border").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,0.9f));
        seq.Insert(0.0f,transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().DOFade(0,0.9f));
        seq.Insert(0.0f,transform.Find("text").DOScale(new Vector3(3,3,1),0.6f));
        seq.Play();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        //transform.Find("text").gameObject.GetComponent<TextMeshPro>().DOFade(0,1f);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
