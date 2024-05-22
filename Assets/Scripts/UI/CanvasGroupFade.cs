using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGroupFade : MonoBehaviour
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
      CanvasGroup canvas = transform.gameObject.GetComponent<CanvasGroup>();
      canvas.alpha = 0.0f;
      while (canvas.alpha < 1.0f)
      {
          canvas.alpha = canvas.alpha + (Time.deltaTime / t);
          yield return null;
      }
      canvas.blocksRaycasts = true;
    }

    public IEnumerator FadeToZeroAlpha(float t)
    {
      CanvasGroup canvas = transform.gameObject.GetComponent<CanvasGroup>();
      canvas.alpha = 1.0f;
      while (canvas.alpha > 0.0f)
      {
          canvas.alpha = canvas.alpha - (Time.deltaTime / t);
          yield return null;
      }
      canvas.blocksRaycasts = false;
    }
}
