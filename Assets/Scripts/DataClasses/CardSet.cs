using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardSet
{
    public string setCode;
    public string setName;
    public List<CardInfo> cards;

    public void debugSet()
    {
      foreach (CardInfo card in cards)
      {
        Debug.Log(card.name);
      }
    }
}
