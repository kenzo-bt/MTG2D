using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Decklist selectedDeck;
    public List<Decklist> allDecks;

    private void Awake()
    {
      if (Instance != null)
      {
          Destroy(gameObject);
          return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);

      loadPlayerDecks();
    }

    // Read in player decks from disk
    private void loadPlayerDecks()
    {
      allDecks = new List<Decklist>();
      string filePath = Application.persistentDataPath + "/AllDecks.txt";
      string fileContents = "";

      if (!System.IO.File.Exists(filePath))
      {
        using (File.Create(filePath));
      }

      fileContents = File.ReadAllText(Application.persistentDataPath + "/AllDecks.txt");
      foreach (string deck in fileContents.Split("---"))
      {
        Decklist individualDeck = new Decklist();
        List<string> cards = new List<string>();
        List<int> cardFrequencies = new List<int>();

        foreach (string card in deck.Split("%")[1].Split('\n'))
        {
          if (!string.IsNullOrWhiteSpace(card))
          {
            cards.Add(card.Split(" ")[0]);
            cardFrequencies.Add(Int32.Parse(card.Split(" ")[1]));
          }
        }

        individualDeck.name = deck.Split("%")[0].Trim();
        individualDeck.cards = cards;
        individualDeck.cardFrequencies = cardFrequencies;

        allDecks.Add(individualDeck);
      }

      // TODO Remove this when deck selector is done
      if (allDecks.Count > 0)
      {
        selectedDeck = allDecks[allDecks.Count - 1];
      }
    }
}
