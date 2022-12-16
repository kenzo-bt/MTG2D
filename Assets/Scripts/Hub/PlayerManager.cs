using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<CardSet> cardCollection;
    public Dictionary<string, CardInfo> cardLookup;
    public Decklist selectedDeck;
    public List<Decklist> allDecks;
    private string decksFilePath;
    public string serverUrl;
    public string serverImageFileExtension;

    private void Awake()
    {
      if (Instance != null)
      {
          Destroy(gameObject);
          return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);

      serverUrl = "http://myxos.live/MTG/";
      serverImageFileExtension = ".jpg";

      cardCollection = new List<CardSet>();
      loadCardCollection();

      cardLookup = new Dictionary<string, CardInfo>();
      createCardLookup();

      decksFilePath = Application.persistentDataPath + "/AllDecks.txt";
      loadPlayerDecks();
    }

    // Load in card collection
    private void loadCardCollection()
    {

      TextAsset activeSetsFile = Resources.Load("ActiveSets") as TextAsset;
      string[] activeSets = activeSetsFile.text.Split('\n');
      for (int i = 0; i < activeSets.Length; i++)
      {
        TextAsset setFile = Resources.Load("Sets/" + activeSets[i].Trim()) as TextAsset;
        CardSet set = JsonUtility.FromJson<CardSet>(setFile.text);
        cardCollection.Add(set);
      }
    }

    // Create lookup dictionary for all cards
    private void createCardLookup()
    {
      foreach (CardSet set in cardCollection)
      {
        foreach (CardInfo card in set.cards)
        {
          cardLookup.Add(card.id, card);
        }
      }
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
        List<CardInfo> cards = new List<CardInfo>();
        List<int> cardFrequencies = new List<int>();

        foreach (string card in deck.Split("%")[1].Split('\n'))
        {
          if (!string.IsNullOrWhiteSpace(card))
          {
            cards.Add(getCardFromLookup(card.Split(" ")[0]));
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

    public CardInfo getCardFromLookup(string id)
    {
      CardInfo targetCard = new CardInfo();
      try
      {
        targetCard = cardLookup[id];
        return targetCard;
      }
      catch (KeyNotFoundException)
      {

      }
      Debug.Log("Card not found in collection!");
      return targetCard;
    }
}
