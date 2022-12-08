using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    private List<string> deck = new List<string>();
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
      TextAsset listFile = Resources.Load("Decklist") as TextAsset;
      string[] decklist = listFile.text.Split('\n');

      foreach (var line in decklist)
      {
        string cardName = line.Split(' ')[0];
        int cardFrequency = Int32.Parse(line.Split(' ')[1]);

        for (int i = 0; i < cardFrequency; i++)
        {
          deck.Add(cardName);
        }
      }
    }

    // Shuffle deck
    public void shuffleDeck()
    {
      List<string> shuffledDeck = new List<string>();
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
    public string drawCard()
    {
      string card = deck[deck.Count - 1];
      deck.RemoveAt(deck.Count - 1);
      return card;
    }

    // Reset deck
    public void resetDeck()
    {
      deck = new List<string>();
      generateDeck();
      shuffleDeck();
    }

    // Debug deck info
    private void debugPrintDeck()
    {
      foreach (var card in deck)
      {
        Debug.Log(card);
      }

      Debug.Log("Card count: " + deck.Count);
    }
}
