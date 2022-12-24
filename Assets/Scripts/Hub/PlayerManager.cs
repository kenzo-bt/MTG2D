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
    public string apiUrl;
    public string serverImageFileExtension;
    public int myID;
    public int opponentID;
    public string myName;
    public string opponentName;
    public List<int> friendIDs;
    public List<string> friendNames;
    public bool loggedIn;
    public string role;

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
      apiUrl = "http://myxos.live/pythonAPI/";
      serverImageFileExtension = ".jpg";

      cardCollection = new List<CardSet>();
      loadCardCollection();

      cardLookup = new Dictionary<string, CardInfo>();
      createCardLookup();

      decksFilePath = Application.persistentDataPath + "/userDecks.txt";
      loadPlayerDecks();

      myID = -1;
      friendIDs = new List<int>();
      friendNames = new List<string>();
      loggedIn = false;
      role = "";
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

      // If file doesnt exist, create a file in persistent data storage and load starter decks into it
      if (!System.IO.File.Exists(decksFilePath))
      {
        using (File.Create(decksFilePath)) {}
      }

      // Read starter decks
      TextAsset starterDeckFile = Resources.Load("StarterDecks") as TextAsset;
      List<string> starterDecks = new List<string>(starterDeckFile.text.Split("---"));
      for (int i = 0; i < starterDecks.Count; i++)
      {
        starterDecks[i] = starterDecks[i].Trim();
        Decklist individualDeck = new Decklist();
        List<CardInfo> cards = new List<CardInfo>();
        List<int> cardFrequencies = new List<int>();

        // Split name / cover / cards
        string deckName = starterDecks[i].Split("%")[0].Trim();
        string[] cardStrings = starterDecks[i].Split("%")[1].Split('\n');
        string coverId = "";
        if (starterDecks[i].Split("%").Length == 3)
        {
          coverId = starterDecks[i].Split("%")[2].Trim();
        }

        foreach (string card in cardStrings)
        {
          if (!string.IsNullOrWhiteSpace(card))
          {
            cards.Add(getCardFromLookup(card.Split(" ")[0]));
            cardFrequencies.Add(Int32.Parse(card.Split(" ")[1]));
          }
        }

        individualDeck.name = deckName;
        if (!individualDeck.name.Contains("!Starter!"))
        {
          individualDeck.name = individualDeck.name + " !Starter!";
        }
        individualDeck.cards = cards;
        individualDeck.cardFrequencies = cardFrequencies;
        individualDeck.coverId = coverId;

        allDecks.Add(individualDeck);
      }

      // Read user decks
      string fileContents = "";
      fileContents = File.ReadAllText(decksFilePath);
      if (fileContents != "")
      {
        foreach (string deck in fileContents.Split("---"))
        {
          Decklist individualDeck = new Decklist();
          List<CardInfo> cards = new List<CardInfo>();
          List<int> cardFrequencies = new List<int>();

          // Split name / cover / cards
          string deckName = deck.Split("%")[0].Trim();
          string[] cardStrings = deck.Split("%")[1].Split('\n');
          string coverId = "";
          if (deck.Split("%").Length == 3)
          {
            coverId = deck.Split("%")[2].Trim();
          }

          foreach (string card in cardStrings)
          {
            if (!string.IsNullOrWhiteSpace(card))
            {
              cards.Add(getCardFromLookup(card.Split(" ")[0]));
              cardFrequencies.Add(Int32.Parse(card.Split(" ")[1]));
            }
          }

          individualDeck.name = deckName;
          individualDeck.cards = cards;
          individualDeck.cardFrequencies = cardFrequencies;
          individualDeck.coverId = coverId;

          allDecks.Add(individualDeck);
        }
      }

      // TODO Remove this when deck selector is done
      if (allDecks.Count > 0)
      {
        //selectedDeck = allDecks[allDecks.Count - 1];
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
          if (!allDecks[i].name.Contains("!Starter!"))
          {
            allDecksString += allDecks[i].getDecklistString();
            if (i < (deckCount - 1))
            {
              allDecksString += "\n---\n";
            }
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
