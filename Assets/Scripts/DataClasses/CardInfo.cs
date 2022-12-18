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
}
