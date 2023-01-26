using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<CardSet> cardCollection;
    public Dictionary<string, CardInfo> cardLookup;
    public Dictionary<string, int> collectedCards;
    public Decklist selectedDeck;
    public List<Decklist> allDecks;
    public List<Decklist> starterDecks;
    private string decksFilePath;
    private string collectionFilePath;
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

      collectedCards = new Dictionary<string, int>();
      collectionFilePath = Application.persistentDataPath + "/userCards.txt";
      loadCollectedCards();

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

    // Read from textfile with cards collected by the user
    private void loadCollectedCards()
    {
      // If file doesnt exist, create a file in persistent data storage
      //// TODO: Here we can flag to give the user starter packs
      if (!System.IO.File.Exists(collectionFilePath))
      {
        using (File.Create(collectionFilePath)) {}
      }
      else
      {
        // Load the collected cards into the dictionary
        string fileContents = "";
        fileContents = File.ReadAllText(collectionFilePath);
        if (fileContents != "")
        {
          foreach (string line in fileContents.Split('\n'))
          {
            // Format: ID Freq
            string id = line.Split(" ")[0];
            int freq = Int32.Parse(line.Split(" ")[1]);
            if (id.Length > 2)
            {
              collectedCards.Add(line.Split(" ")[0], freq);
            }
          }

          foreach (KeyValuePair<string, int> item in collectedCards)
          {
            Debug.Log("ID: " + item.Key + " / Freq: " + item.Value);
          }
        }
      }
    }

    private void saveCollectedCards()
    {
      string allCardsString = "";
      foreach (KeyValuePair<string, int> item in collectedCards)
      {
        allCardsString += (item.Key + " " + item.Value + "\n");
      }
      File.WriteAllText(collectionFilePath, allCardsString);
    }

    public void readStarterDecks()
    {
      StartCoroutine(getStarterDecksFromServer());
    }

    private IEnumerator getStarterDecksFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "globals/starters";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          AllDecklists starters = new AllDecklists();
          starters = JsonUtility.FromJson<AllDecklists>(serverJson);
          starterDecks = new List<Decklist>(starters.decks);
          foreach (Decklist deck in starterDecks)
          {
            Debug.Log(deck.name);
          }
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
      readStarterDecks();

      // Read user decks
      string fileContents = "";
      fileContents = File.ReadAllText(decksFilePath);
      if (fileContents != "")
      {
        foreach (string deck in fileContents.Split("---"))
        {
          Decklist individualDeck = new Decklist();
          List<string> cards = new List<string>();
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
              cards.Add(card.Split(" ")[0]);
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
      Debug.Log("Card not found in collection! (" + id + ")");
      return targetCard;
    }
}
