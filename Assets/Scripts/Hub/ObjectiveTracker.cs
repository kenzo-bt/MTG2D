using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveTracker : MonoBehaviour
{
    public GameObject typeObject;
    public GameObject colourObject;
    public GameObject progressObject;
    public GameObject progressTotalObject;
    private TMP_Text progressText;
    private TMP_Text progressTotalText;
    private Image typeImage;
    private Image colourImage;

    void Awake()
    {
      progressText = progressObject.GetComponent<TMP_Text>();
      progressTotalText = progressTotalObject.GetComponent<TMP_Text>();
      typeImage = typeObject.GetComponent<Image>();
      colourImage = colourObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setData(string objectiveType, string objectiveColour, int progress, int total)
    {
      // Set tracker progress text
      progressText.text = "" + progress;
      progressTotalText.text = "of " + total;
      // Get symbol for corresponding objective types
      string typeSymbol = "";
      if (objectiveType == "Lands") { typeSymbol = "L"; }
      else if (objectiveType == "Creatures") { typeSymbol = "C"; }
      else { typeSymbol = "S"; }
      // Get symbol for corresponding objective colour
      string colourSymbol = "";
      if (objectiveColour == "Green") { colourSymbol = "G"; }
      else if (objectiveColour == "Red") { colourSymbol = "R"; }
      else if (objectiveColour == "Blue") { colourSymbol = "B"; }
      else if (objectiveColour == "White") { colourSymbol = "W"; }
      else { colourSymbol = "K"; }
      // Set textures for colour/type images
      Texture2D objectiveColourTexture = Resources.Load("Images/Symbols/" + colourSymbol + "-50") as Texture2D;
      colourImage.sprite = Sprite.Create(objectiveColourTexture, new Rect(0, 0, objectiveColourTexture.width, objectiveColourTexture.height), new Vector2(0.5f, 0.5f));
      Texture2D objectiveTypeTexture = Resources.Load("Images/Symbols/" + typeSymbol + "-50") as Texture2D;
      typeImage.sprite = Sprite.Create(objectiveTypeTexture, new Rect(0, 0, objectiveTypeTexture.width, objectiveTypeTexture.height), new Vector2(0.5f, 0.5f));
    }
}
