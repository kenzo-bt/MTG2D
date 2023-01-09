using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{
    public GameObject logger;
    public GameObject logTextObject;
    private TMP_Text loggerText;
    private bool visible;
    // Start is called before the first frame update
    void Start()
    {
      loggerText = logTextObject.GetComponent<TMP_Text>();
      visible = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void toggleLogger()
    {
      if (visible)
      {
        logger.GetComponent<CanvasGroup>().alpha = 0;
        logger.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
      else
      {
        logger.GetComponent<CanvasGroup>().alpha = 1;
        logger.GetComponent<CanvasGroup>().blocksRaycasts = true;
      }
      visible = !visible;
    }

    public void addToLogger(string message)
    {

      loggerText.text += ("\n" + message);
    }
}
