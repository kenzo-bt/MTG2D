using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decklist {
  public string name;
  public List<CardInfo> cards;
  public List<int> cardFrequencies;

  public string getDecklistString()
  {
    string decklistString = name + "\n%\n";
    for (int i = 0; i < cards.Count; i++)
    {
      decklistString += cards[i].id + " " + cardFrequencies[i] + "\n";
    }
    return decklistString.Trim();
  }

  public void debugPrintDeck()
  {
    Debug.Log("------");
    Debug.Log("Deck name: " + name);
    for (int i = 0; i < cards.Count; i++)
    {
      Debug.Log(cards[i].name + " x" + cardFrequencies[i]);
    }
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
  public void addCard(CardInfo card)
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
  public void removeCard(CardInfo card)
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

  // Default constructor
  public Decklist() {
    this.name = "";
    this.cards = new List<CardInfo>();
    this.cardFrequencies = new List<int>();
  }

  // Copy constructor
  public Decklist (Decklist deckToCopy) {
    this.name = deckToCopy.name;
    this.cards = new List<CardInfo>(deckToCopy.cards);
    this.cardFrequencies = new List<int>(deckToCopy.cardFrequencies);
  }

  // Get selected card
  public CardInfo getCoverCard()
  {
    if (cards.Count > 0)
    {
      return cards[0];
    }
    return PlayerManager.Instance.cardCollection[0].cards[0];
  }
}
