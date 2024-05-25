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
      Debug.Log(cards.Count);
      foreach (CardInfo card in cards)
      {
        Debug.Log(card.name);
      }
    }

    public List<string> getPack()
    {
      List<string> pack = new List<string>();
      // Add 1 rare or mythic rare
      List<CardInfo> rares = getAllRares();

      pack.Add(rares[UnityEngine.Random.Range(0, rares.Count)].id);
      // Add 3 uncommons
      List<CardInfo> uncommons = getAllUncommons();
      for (int i = 0; i < 3; i++)
      {
        pack.Add(uncommons[UnityEngine.Random.Range(0, uncommons.Count)].id);
      }
      // Add 10 random commons
      List<CardInfo> commons = getAllCommons();
      for (int i = 0; i < 10; i++)
      {
        pack.Add(commons[UnityEngine.Random.Range(0, commons.Count)].id);
      }
      return pack;
    }

    public List<CardInfo> getAllRares()
    {
      List<CardInfo> allRares = new List<CardInfo>();
      foreach (CardInfo card in cards)
      {
        if (card.rarity == "rare" || card.rarity == "mythic")
        {
          if (!card.isToken && !card.isBack && card.finishes.Contains("nonfoil"))
          {
            // Remove alchemy cards
            if (card.name.Length > 1 && card.name.Substring(0, 2) != "A-")
            {
              allRares.Add(card);
            }
          }
        }
      }
      return allRares;
    }

    public List<CardInfo> getAllUncommons()
    {
      List<CardInfo> allUncommons = new List<CardInfo>();
      foreach (CardInfo card in cards)
      {
        if (card.rarity == "uncommon")
        {
          if (!card.isToken && !card.isBack && card.finishes.Contains("nonfoil"))
          {
            // Remove alchemy cards
            if (card.name.Length > 1 && card.name.Substring(0, 2) != "A-")
            {
              allUncommons.Add(card);
            }
          }
        }
      }
      return allUncommons;
    }

    public List<CardInfo> getAllCommons()
    {
      List<CardInfo> allCommons = new List<CardInfo>();
      foreach (CardInfo card in cards)
      {
        if (card.rarity == "common")
        {
          if (!card.isBasicLand() && !card.isToken && !card.isBack && card.finishes.Contains("nonfoil"))
          {
            // Remove alchemy cards
            if (card.name.Length > 1 && card.name.Substring(0, 2) != "A-")
            {
              allCommons.Add(card);
            }
          }
        }
      }
      return allCommons;
    }
}
