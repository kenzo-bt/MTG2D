using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Decklist {
  public string name;
  public List<string> cards;
  public List<string> sideboard;
  public List<int> cardFrequencies;
  public List<int> sideboardFrequencies;
  public string coverId;
  public bool isDraft;

  public string getDecklistString()
  {
    string decklistString = name + "\n%\n";
    for (int i = 0; i < cards.Count; i++)
    {
      decklistString += cards[i] + " " + cardFrequencies[i] + "\n";
    }
    if (coverId != "")
    {
      decklistString += "%\n" + coverId;
    }
    return decklistString.Trim();
  }

  public void debugPrintDeck()
  {
    Debug.Log("------");
    Debug.Log("Deck name: " + name);
    for (int i = 0; i < cards.Count; i++)
    {
      Debug.Log(cards[i] + " x" + cardFrequencies[i]);
    }
    Debug.Log("Cover card: " + coverId);
  }

  // Get number of cards in deck
  public int getNumCards()
  {
    int total = 0;
    foreach (int freq in cardFrequencies)
    {
      total += freq;
    }
    return total;
  }

  // Add card to deck
  public void addCard(string card)
  {
    if (cards.Contains(card))
    {
      int index = cards.IndexOf(card);
      cardFrequencies[index] += 1;
    }
    else
    {
      cards.Add(card);
      cardFrequencies.Add(1);
    }
  }

  // Remove card (Remove 1 instance of the passed card)
  public void removeCard(string card)
  {
    if (cards.Contains(card))
    {
      int index = cards.IndexOf(card);
      cardFrequencies[index] -= 1;
      if (cardFrequencies[index] <= 0)
      {
        cards.RemoveAt(index);
        cardFrequencies.RemoveAt(index);
      }
    }
  }

  public void addToSideboard(string card)
  {
    if (sideboard.Contains(card))
    {
      int index = sideboard.IndexOf(card);
      sideboardFrequencies[index] += 1;
    }
    else
    {
      sideboard.Add(card);
      sideboardFrequencies.Add(1);
    }
  }

  public void removeFromSideboard(string card)
  {
    if (sideboard.Contains(card))
    {
      int index = sideboard.IndexOf(card);
      sideboardFrequencies[index] -= 1;
      if (sideboardFrequencies[index] <= 0)
      {
        sideboard.RemoveAt(index);
        sideboardFrequencies.RemoveAt(index);
      }
    }
  }

  // Default constructor
  public Decklist() {
    this.name = "";
    this.cards = new List<string>();
    this.cardFrequencies = new List<int>();
    this.sideboard = new List<string>();
    this.sideboardFrequencies = new List<int>();
    this.coverId = "";
    this.isDraft = false;
  }

  // Copy constructor
  public Decklist (Decklist deckToCopy) {
    this.name = deckToCopy.name;
    this.cards = new List<string>(deckToCopy.cards);
    this.cardFrequencies = new List<int>(deckToCopy.cardFrequencies);
    this.sideboard = new List<string>(deckToCopy.sideboard);
    this.sideboardFrequencies = new List<int>(deckToCopy.sideboardFrequencies);
    this.coverId = deckToCopy.coverId;
    this.isDraft = deckToCopy.isDraft;
  }

  // Get selected card
  public CardInfo getCoverCard()
  {
    if (coverId != "")
    {
      return PlayerManager.Instance.getCardFromLookup(coverId);
    }
    else if (cards.Count > 0)
    {
      return PlayerManager.Instance.getCardFromLookup(cards[0]);
    }
    else
    {
      return PlayerManager.Instance.cardCollection[0].cards[0];
    }
  }
}
