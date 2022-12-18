using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaSymbol : MonoBehaviour
{
    public GameObject foreground;
    public GameObject textObject;
    private TMP_Text symbolText;
    private Image symbolImage;

    // Start is called before the first frame update
    void Awake()
    {
      symbolText = textObject.GetComponent<TMP_Text>();
      symbolImage = foreground.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the symbol's data
    public void paintSymbol(string symbol)
    {
      string targetSymbol = "?";
      string text = "";

      if (symbol == "W") { targetSymbol = "W"; }
      else if (symbol == "U") { targetSymbol = "U"; }
      else if (symbol == "B") { targetSymbol = "B"; }
      else if (symbol == "R") { targetSymbol = "R"; }
      else if (symbol == "G") { targetSymbol = "G"; }
      else if (symbol == "C") { targetSymbol = "C"; }
      else {
        text = symbol;
        targetSymbol = "N";
      }
      Texture2D symbolTexture = Resources.Load("Images/Symbols/" + targetSymbol) as Texture2D;
      symbolImage.sprite = Sprite.Create(symbolTexture, new Rect(0, 0, symbolTexture.width, symbolTexture.height), new Vector2(0.5f, 0.5f));
      // Add text
      if (targetSymbol == "N")
      {
        symbolText.text = text;
      }
    }
}
