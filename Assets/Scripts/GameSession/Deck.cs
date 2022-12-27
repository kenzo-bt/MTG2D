using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    private CardStack stack;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Initialize the deck at game start
    public void initializeDeck()
    {
      stack = GetComponent<CardStack>();
      stack.initializeStack();
      generateDeck();
      shuffleDeck();
    }

    private void generateDeck()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;

      for (int i = 0; i < selectedDeck.cards.Count; i++)
      {
        for (int n = 0; n < selectedDeck.cardFrequencies[i]; n++)
        {
          stack.addCard(selectedDeck.cards[i].id);
        }
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      List<string> shuffledDeck = new List<string>();
      int numCards = stack.cards.Count;
      for (int i = 0; i < numCards; i++)
      {
        int randIndex = UnityEngine.Random.Range(0, stack.cards.Count);
        shuffledDeck.Add(stack.cards[randIndex]);
        stack.cards.RemoveAt(randIndex);
      }
      stack.cards = new List<string>(shuffledDeck);
    }

    // Draws a card from deck, returns the CardInfo
    public CardInfo drawCard()
    {
      if (stack.cards.Count > 0)
      {
        string cardId = stack.cards[stack.cards.Count - 1];
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        stack.cards.RemoveAt(stack.cards.Count - 1);
        return card;
      }
      else
      {
        Debug.Log("CANT DRAW! Deck size is currently " + stack.cards.Count);
        return null;
      }
    }

    // Get ids of my stack
    public List<string> getDeckIds()
    {
      return new List<string>(stack.cards);
    }

    // Debug deck info
    private void debugPrintDeck()
    {
      foreach (string cardId in stack.cards)
      {
        Debug.Log("CardID: " + cardId);
      }
      Debug.Log("Card count: " + stack.cards.Count);
    }
}
