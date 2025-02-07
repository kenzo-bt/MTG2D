using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo
{
    public string id;
    public string imageUrl;
    public string name;
    public string text;
    public List<string> colours;
    public List<string> colourIdentity;
    public int manaValue;
    public string manaCost;
    public List<string> types;
    public List<string> supertypes;
    public List<string> subtypes;
    public string rarity;
    public string set;
    public string backId;
    public bool isBack;
    public string layout;
    public bool isToken;
    public List<string> finishes;
    public string artist;
    public string language;
    public List<string> variations;

    public bool isBasicLand()
    {
      if (types.Contains("Land") && supertypes.Contains("Basic"))
      {
        return true;
      }
      return false;
    }

    public bool isLand()
    {
        if (types.Contains("Land") && !types.Contains("Creature"))
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

    public bool hasEnglishTranslation()
    {
      if (language != "English" && variations.Count != 0)
      {
        return true;
      }
      return false;
    }
}
