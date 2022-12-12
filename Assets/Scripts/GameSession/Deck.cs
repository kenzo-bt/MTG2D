using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    private List<CardInfo> deck = new List<CardInfo>();
    public bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
      generateDeck();
      shuffleDeck();
      isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Read decklist and generate deck
    private void generateDeck()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;

      for (int i = 0; i < selectedDeck.cards.Count; i++)
      {
        for (int n = 0; n < selectedDeck.cardFrequencies[i]; n++)
        {
          deck.Add(selectedDeck.cards[i]);
        }
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      List<CardInfo> shuffledDeck = new List<CardInfo>();
      int numCards = deck.Count;
      for (int i = 0; i < numCards; i++)
      {
        int randIndex = UnityEngine.Random.Range(0, deck.Count);
        shuffledDeck.Add(deck[randIndex]);
        deck.RemoveAt(randIndex);
      }
      deck = shuffledDeck;
    }

    // Draws a card from deck. Remove from deck and return card name.
    public CardInfo drawCard()
    {
      if (deck.Count > 0)
      {
        CardInfo card = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        return card;
      }
      else
      {
        Debug.Log("CANT DRAW! Deck size is currently " + deck.Count);
        return null;
      }
    }

    // Reset deck
    public void resetDeck()
    {
      deck = new List<CardInfo>();
      generateDeck();
      shuffleDeck();
    }

    // Debug deck info
    private void debugPrintDeck()
    {
      foreach (CardInfo card in deck)
      {
        Debug.Log(card.name);
      }

      Debug.Log("Card count: " + deck.Count);
    }
}
