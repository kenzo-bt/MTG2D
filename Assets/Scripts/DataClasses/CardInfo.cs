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
    public int convertedManaCost;
    public List<string> types;
    public string rarity;
    public string set;
}
