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
}
