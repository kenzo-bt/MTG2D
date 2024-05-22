using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeToFullAlpha(float t)
    {
      Image i = transform.gameObject.GetComponent<Image>();
      i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
      while (i.color.a < 1.0f)
      {
          i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
          yield return null;
      }
    }

    public IEnumerator FadeToZeroAlpha(float t)
    {
      Image i = transform.gameObject.GetComponent<Image>();
      i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
      while (i.color.a > 0.0f)
      {
          i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
          yield return null;
      }
    }
}
