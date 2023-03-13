using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaSymbol : MonoBehaviour
{
    public GameObject foreground;
    public GameObject splitLeft;
    public GameObject splitRight;
    public GameObject textObject;
    private TMP_Text symbolText;
    private Image symbolImage;
    private Image symbolLeft;
    private Image symbolRight;

    // Start is called before the first frame update
    void Awake()
    {
      symbolText = textObject.GetComponent<TMP_Text>();
      symbolImage = foreground.GetComponent<Image>();
      symbolLeft = splitLeft.GetComponent<Image>();
      symbolRight = splitRight.GetComponent<Image>();
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
      symbolText.text = text;

      if (symbol == "W") { targetSymbol = "W"; }
      else if (symbol == "U") { targetSymbol = "U"; }
      else if (symbol == "B") { targetSymbol = "B"; }
      else if (symbol == "R") { targetSymbol = "R"; }
      else if (symbol == "G") { targetSymbol = "G"; }
      else if (symbol == "C") { targetSymbol = "C"; }
      else if (symbol.Contains("/"))
      {
        if (symbol.Split("/")[1] == "P") // Phyrexian mana
        {
          targetSymbol = symbol.Split("/")[0] + "P";
        }
        else { // Hybrid mana
          string left = symbol.Split("/")[0];
          string right = symbol.Split("/")[1];
          if (symbol.Length == 5 && symbol.Split("/")[2] == "P") // Hybrid Phyrexian mana
          {
            Texture2D leftTexture = Resources.Load("Images/Symbols/" + left + "P-1-small") as Texture2D;
            Texture2D rightTexture = Resources.Load("Images/Symbols/" + right + "P-2-small") as Texture2D;
            symbolLeft.sprite = Sprite.Create(leftTexture, new Rect(0, 0, leftTexture.width, leftTexture.height), new Vector2(0.5f, 0.5f));
            symbolRight.sprite = Sprite.Create(rightTexture, new Rect(0, 0, rightTexture.width, rightTexture.height), new Vector2(0.5f, 0.5f));
          }
          else
          {
            Texture2D leftTexture = Resources.Load("Images/Symbols/" + left + "-1-small") as Texture2D;
            Texture2D rightTexture = Resources.Load("Images/Symbols/" + right + "-2-small") as Texture2D;
            symbolLeft.sprite = Sprite.Create(leftTexture, new Rect(0, 0, leftTexture.width, leftTexture.height), new Vector2(0.5f, 0.5f));
            symbolRight.sprite = Sprite.Create(rightTexture, new Rect(0, 0, rightTexture.width, rightTexture.height), new Vector2(0.5f, 0.5f));
          }
          splitLeft.GetComponent<CanvasGroup>().alpha = 1;
          splitRight.GetComponent<CanvasGroup>().alpha = 1;
          return;
        }
      }
      else {
        text = symbol;
        targetSymbol = "N";
      }
      Texture2D symbolTexture = Resources.Load("Images/Symbols/" + targetSymbol + "-Small") as Texture2D;
      symbolImage.sprite = Sprite.Create(symbolTexture, new Rect(0, 0, symbolTexture.width, symbolTexture.height), new Vector2(0.5f, 0.5f));
      // Add text
      if (targetSymbol == "N")
      {
        symbolText.text = text;
      }
    }
}
