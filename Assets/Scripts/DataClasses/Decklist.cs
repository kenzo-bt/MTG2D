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
  public bool isTimeChallenge;
  public string timeChallengeCardSet;
  public List<string> timeChallengeCardColours;
  public bool isTimeChallengeEditable;
  public int objectiveId;
  public int objectiveDeckId;

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

  private int getNumberOfRares()
  {
    int numberOfRares = 0;
    for (int i = 0; i < cards.Count; i++)
      {
        if (PlayerManager.Instance.getCardFromLookup(cards[i]).rarity == "rare")
        {
          numberOfRares += cardFrequencies[i];
        }
      }
      return numberOfRares;
  }

  private int getNumberOfMythics()
  {
    int numberOfMythics = 0;
    for (int i = 0; i < cards.Count; i++)
    {
      if (PlayerManager.Instance.getCardFromLookup(cards[i]).rarity == "mythic")
      {
        numberOfMythics += cardFrequencies[i];
      }
    }
    return numberOfMythics;
  }

  private int getNumberOfNonBasicLandCards()
  {
    int nonLandCards = 0;
    for (int i = 0; i < cards.Count; i++)
    {
      if (!PlayerManager.Instance.getCardFromLookup(cards[i]).isBasicLand())
      {
        nonLandCards += cardFrequencies[i];
      }
    }
    return nonLandCards;
  }

  // Add card to deck
  public void addCard(string card)
  {
    CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(card);
    if (PlayerManager.Instance.selectedDeck.isTimeChallenge)
    {
      if (getNumberOfNonBasicLandCards() >= 40)
      {
        return;
      }
      string cardRarity = cardInfo.rarity;
      if (cardRarity == "rare" || cardRarity == "mythic")
      {
        if ((getNumberOfRares() + getNumberOfMythics()) >= 5)
        {
          return;
        }
        if (cardRarity == "mythic" && getNumberOfMythics() >= 2)
        {
          return;
        }
      }
    }

    if (PlayerManager.Instance.selectedDeck.isTimeChallenge && !PlayerManager.Instance.selectedDeck.isTimeChallengeEditable)
    {
      if (!cardInfo.isBasicLand())
      {
        return;
      }
    }

    if (cards.Contains(card))
    {
      int index = cards.IndexOf(card);
      if (!PlayerManager.Instance.getCardFromLookup(card).isBasicLand())
      {
        if (cardFrequencies[index] >= 4)
        {
          return;
        }
      }
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
    CardInfo cardInfo = PlayerManager.Instance.getCardFromLookup(card);
    if (PlayerManager.Instance.selectedDeck.isTimeChallenge)
    {
      if (card == PlayerManager.Instance.timeChallengeSelectedRare)
      {
        for (int i = 0; i < cards.Count; i++)
        {
          if (cards[i] == PlayerManager.Instance.timeChallengeSelectedRare && cardFrequencies[i] == 1)
          {
            return;
          }
        }
      }
    }

    if (PlayerManager.Instance.selectedDeck.isTimeChallenge && !PlayerManager.Instance.selectedDeck.isTimeChallengeEditable)
    {
      if (!cardInfo.isBasicLand())
      {
        return;
      }
    }

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
    this.isTimeChallenge = false;
    this.timeChallengeCardSet = "";
    this.timeChallengeCardColours = new List<string>();
    this.isTimeChallengeEditable = true;
    this.objectiveId = -1;
    this.objectiveDeckId = -1;
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
    this.isTimeChallenge = deckToCopy.isTimeChallenge;
    this.timeChallengeCardSet = deckToCopy.timeChallengeCardSet;
    this.timeChallengeCardColours = new List<string>(deckToCopy.timeChallengeCardColours);
    this.isTimeChallengeEditable = deckToCopy.isTimeChallengeEditable;
    this.objectiveId = deckToCopy.objectiveId;
    this.objectiveDeckId = deckToCopy.objectiveDeckId;
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
