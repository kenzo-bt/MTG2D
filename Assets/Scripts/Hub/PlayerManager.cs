using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Decklist selectedDeck;
    public List<Decklist> allDecks;
    string decksFilePath;

    private void Awake()
    {
      if (Instance != null)
      {
          Destroy(gameObject);
          return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);

      decksFilePath = Application.persistentDataPath + "/AllDecks.txt";
      loadPlayerDecks();
    }

    // Read in player decks from disk
    private void loadPlayerDecks()
    {
      allDecks = new List<Decklist>();
      string fileContents = "";

      // If file doesnt exist, create a file in persistent data storage and load starter decks into it
      if (!System.IO.File.Exists(decksFilePath))
      {
        using (File.Create(decksFilePath));
        TextAsset starterDeckFile = Resources.Load("StarterDecks") as TextAsset;
        File.WriteAllText(decksFilePath, starterDeckFile.text);
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

    // Save decks to persistent storage
    public void savePlayerDecks()
    {
      int deckCount = allDecks.Count;
      if (deckCount > 0)
      {
        string allDecksString = "";
        for (int i = 0; i < deckCount; i++)
        {
            allDecksString += allDecks[i].getDecklistString();
            if (i < (deckCount - 1))
            {
              allDecksString += "\n---\n";
            }
        }
        File.WriteAllText(decksFilePath, allDecksString);
      }
    }
}
