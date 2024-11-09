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
      // Secret lair special 3 card pack
      if (setCode == "SLD")
      {
        // Add 3 random cards
        List<int> selectedIndexes = new List<int>();
        while (pack.Count < 3)
        {
          int randomIndex = UnityEngine.Random.Range(0, cards.Count);
          if (!selectedIndexes.Contains(randomIndex))
          {
            selectedIndexes.Add(randomIndex);
            if (!cards[randomIndex].isBasicLand())
            {
              pack.Add(cards[randomIndex].id);
            }
          }
        }
        return pack;
      }
      // Add 1 rare or mythic rare
      List<CardInfo> rares = getAllRares();
      if (rares.Count != 0)
      {
        pack.Add(rares[UnityEngine.Random.Range(0, rares.Count)].id);
      }
      // Add 3 uncommons
      List<CardInfo> uncommons = getAllUncommons();
      for (int i = 0; i < 3; i++)
      {
        if (uncommons.Count != 0)
        {
          pack.Add(uncommons[UnityEngine.Random.Range(0, uncommons.Count)].id);
        }
      }
      // Add 10 random commons
      List<CardInfo> commons = getAllCommons();
      for (int i = 0; i < 10; i++)
      {
        if (commons.Count != 0)
        {
          pack.Add(commons[UnityEngine.Random.Range(0, commons.Count)].id);
        }
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

    public List<CardInfo> getAllRaresNoArtifacts()
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
              // Remove colourless cards
              if (card.colourIdentity.Count > 0)
              {
                allRares.Add(card);
              }
            }
          }
        }
      }
      return allRares;
    }

    public List<CardInfo> getAllOfRarityAndColour(string rarity, string colour)
    {
      if (rarity == "rare")
      {
        List<CardInfo> rarityList = getAllRares();
        List<CardInfo> colourList = new List<CardInfo>();
        foreach (CardInfo card in rarityList)
        {
          if (card.colours.Contains(colour))
          {
            colourList.Add(card);
          }
        }
        return colourList;
      }
      else if (rarity == "uncommon")
      {
        List<CardInfo> rarityList = getAllUncommons();
        List<CardInfo> colourList = new List<CardInfo>();
        foreach (CardInfo card in rarityList)
        {
          if (card.colours.Contains(colour))
          {
            colourList.Add(card);
          }
        }
        return colourList;
      }
      else
      {
        List<CardInfo> rarityList = getAllCommons();
        List<CardInfo> colourList = new List<CardInfo>();
        foreach (CardInfo card in rarityList)
        {
          if (card.colours.Contains(colour))
          {
            colourList.Add(card);
          }
        }
        return colourList;
      }
    }

    public List<CardInfo> getAllLegendaries()
    {
      List<CardInfo> allLegendaries = new List<CardInfo>();
      foreach (CardInfo card in cards)
      {
        if (card.supertypes.Contains("Legendary"))
        {
          allLegendaries.Add(card);
        }
      }
      return allLegendaries;
    }

    public List<CardInfo> getAllAlternateArts()
    {
      List<CardInfo> allAlternateArts = new List<CardInfo>();
      List<string> addedCardIds = new List<string>();
      foreach (CardInfo card in cards)
      {
        if (!card.isBasicLand() && !addedCardIds.Contains(card.id) && card.variations.Count != 0)
        {
          foreach(string variation in card.variations)
          {
            addedCardIds.Add(variation);
            allAlternateArts.Add(PlayerManager.Instance.getCardFromLookup(variation));
          }
        }
      }
      return allAlternateArts;
    }
}
