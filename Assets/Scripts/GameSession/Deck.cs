using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Deck : MonoBehaviour, IPointerClickHandler
{
    private CardStack stack;
    public GameObject sideboardObject;
    public GameObject contextMenu;

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
      loadSideboard();
    }

    private void generateDeck()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;

      for (int i = 0; i < selectedDeck.cards.Count; i++)
      {
        for (int n = 0; n < selectedDeck.cardFrequencies[i]; n++)
        {
          stack.addCard(selectedDeck.cards[i], false);
        }
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      // New addition: shuffle 5 times to improve randomization
      for (int n = 0; n < 5; n++)
      {
        List<string> shuffledDeck = new List<string>();
        stack.cardsVisibility = new List<bool>();
        int numCards = stack.cards.Count;
        for (int i = 0; i < numCards; i++)
        {
          int randIndex = UnityEngine.Random.Range(0, stack.cards.Count);
          shuffledDeck.Add(stack.cards[randIndex]);
          stack.cards.RemoveAt(randIndex);
        }
        stack.cards = new List<string>(shuffledDeck);
        for (int i = 0; i < stack.cards.Count; i++)
        {
          stack.cardsVisibility.Add(false);
        }
      }
    }

    // Draws a card from deck, returns the CardInfo
    public CardInfo drawCard(string mode = "")
    {
      CardInfo card = null;
      if (stack.cards.Count > 0)
      {
        switch (mode)
        {
          case "NL":
            for (int i = stack.cards.Count - 1; i >= 0; i--)
            {
              card = PlayerManager.Instance.getCardFromLookup(stack.cards[i]);
              if (!card.isLand())
              {
                stack.removeCard(i);
                return card;
              }
            }
            card = null;
            break;
          case "L":
            for (int i = stack.cards.Count - 1; i >= 0; i--)
            {
              card = PlayerManager.Instance.getCardFromLookup(stack.cards[i]);
              if (card.isLand())
              {
                stack.removeCard(i);
                return card;
              }
            }
            card = null;
            break;
          default:
            string cardId = stack.cards[stack.cards.Count - 1];
            card = PlayerManager.Instance.getCardFromLookup(cardId);
            stack.removeCard(stack.cards.Count - 1);
            break;
        } 
        return card;
      }
      else
      {
        return null;
      }
    }

    // Get ids of my stack (! Add card visibility here)
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

    public void showContextMenu()
    {
      contextMenu.SetActive(true);
    }

    public void hideContextMenu()
    {
      contextMenu.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Right)
      {
        if (contextMenu.activeSelf)
        {
          hideContextMenu();
        }
        else
        {
          showContextMenu();
        }
      }
    }

    private void loadSideboard()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;
      CardStack sideboard = sideboardObject.GetComponent<CardStack>();
      sideboard.initializeStack();

      for (int i = 0; i < selectedDeck.sideboard.Count; i++)
      {
        for (int n = 0; n < selectedDeck.sideboardFrequencies[i]; n++)
        {
          sideboard.addCard(selectedDeck.sideboard[i], true);
        }
      }
    }
}
