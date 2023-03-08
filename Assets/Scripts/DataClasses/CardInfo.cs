using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo
{
    public string id;
    public string name;
    public string text;
    public List<string> colours;
    public List<string> colourIdentity;
    public int manaValue;
    public string manaCost;
    public List<string> types;
    public string rarity;
    public string set;
    public string backId;
    public bool isBack;
    public string layout;
    public bool isToken;
    public List<string> finishes;

    public bool isBasicLand()
    {
      if (name == "Plains" || name == "Swamp" || name == "Forest" || name == "Mountain" || name == "Island")
      {
        return true;
      }
      return false;
    }

    public bool hasBackSide()
    {
      if (layout != "adventure" && layout != "split" && backId != "" && backId != null)
      {
        return true;
      }
      return false;
    }
}
