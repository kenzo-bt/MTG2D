using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleObject;
    public GameObject loginObject;
    private Image title;
    private Image login;
    // Start is called before the first frame update
    void Start()
    {
        title = titleObject.GetComponent<Image>();
        login = loginObject.GetComponent<Image>();
        StartCoroutine(FadeTitleToFullAlpha(1.5f, title));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeTitleToFullAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeTitleToZeroAlpha(1.5f, i));
    }
 
    public IEnumerator FadeTitleToZeroAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        titleObject.SetActive(false);
        loginObject.SetActive(true);
        StartCoroutine(FadeToFullAlpha(1.5f, login));
    }

    public IEnumerator FadeToFullAlpha(float t, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }
}
