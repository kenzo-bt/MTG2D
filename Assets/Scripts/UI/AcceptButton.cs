using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AcceptButton : MonoBehaviour
{
    private Color buttonColour;
    public GameObject fgTextObject;
    public GameObject bgTextObject;
    private TMP_Text text;
    private TMP_Text shadow;

    // Start is called before the first frame update
    void Awake()
    {
      buttonColour = GetComponent<Image>().color;
      text = fgTextObject.GetComponent<TMP_Text>();
      shadow = bgTextObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void disable()
    {
      GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
      GetComponent<Button>().interactable = false;
      text.color = new Color(text.color.r, text.color.g, text.color.b, 0.3f);
      shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, 0.3f);
    }

    public void enable()
    {
      GetComponent<Image>().color = buttonColour;
      GetComponent<Button>().interactable = true;
      text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
      shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, 1f);
    }
}
