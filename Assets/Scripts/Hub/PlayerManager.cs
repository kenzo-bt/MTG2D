using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    public Decklist opponentSelectedDeck;
    public List<Decklist> allDecks;
    public List<Decklist> starterDecks;
    public List<Decklist> proFeaturedDecks;
    private string decksFilePath;
    private string collectionFilePath;
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
    public int draftHostID;
    public int lobbyHostID;
    public int timeChallengeHostID;
    public List<int> lobbyOpponents;
    public List<Decklist> lobbyOpponentDecks;
    public DailyObjectives dailyObjectives;
    public GameObject objectivesPanel;
    public int lastCoinAmount;
    public int lastGemAmount;
    public List<string> timeChallengeRares;
    public string timeChallengeSelectedRare;
    public string timeChallengeSetCode;
    public int objectiveId;

    private void Awake()
    {
      if (Instance != null)
      {
          Destroy(gameObject);
          return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // apiUrl = "http://127.0.0.1:5000/";
      apiUrl = "https://mirariapi.onrender.com/";
      serverImageFileExtension = ".jpg";

      cardCollection = new List<CardSet>();
      loadCardCollection();

      cardLookup = new Dictionary<string, CardInfo>();
      createCardLookup();

      decksFilePath = Application.persistentDataPath + "/userDecks.txt";

      myID = -1;
      friendIDs = new List<int>();
      friendNames = new List<string>();
      loggedIn = false;
      role = "";
      lastGemAmount = 0;
      lastCoinAmount = 0;
      objectiveId = -1;

      // Keep connection to server alive
      StartCoroutine(keepConnectionAliveSignal());
    }

    // Load in card collection
    private void loadCardCollection()
    {
      TextAsset activeSetsFile = Resources.Load("ActiveSets") as TextAsset;
      string[] activeSets = activeSetsFile.text.Split('\n');
      for (int i = 0; i < activeSets.Length; i++)
      {
        TextAsset setFile = Resources.Load("Sets/" + activeSets[i].Trim()) as TextAsset;
        if (activeSets[i].Trim() != "") {
          CardSet set = JsonUtility.FromJson<CardSet>(setFile.text);
          cardCollection.Add(set);
        }
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

    public void saveCollectedCards()
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
        }
      }
    }

    public void readProDecks()
    {
      StartCoroutine(getProDecksFromServer());
    }

    private IEnumerator getProDecksFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "globals/proFeatured";
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
          AllDecklists proDecks = new AllDecklists();
          proDecks = JsonUtility.FromJson<AllDecklists>(serverJson);
          proFeaturedDecks = new List<Decklist>(proDecks.decks);
        }
      }
    }

    public void loadPlayerDecks()
    {
      StartCoroutine(getPlayerDecksFromServer());
    }

    private IEnumerator getPlayerDecksFromServer()
    {
      string url = apiUrl + "users/" + myID + "/decks";
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
          AllDecklists playerDecks = new AllDecklists();
          playerDecks = JsonUtility.FromJson<AllDecklists>(serverJson);
          allDecks = new List<Decklist>(playerDecks.decks);
          // If there are no decks in server, check if local decklist exists
          if (allDecks.Count == 0)
          {
            if (System.IO.File.Exists(decksFilePath))
            {
              // Load decks from local decklist
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

                // Update player decks in server
                savePlayerDecks();

                // Empty the local file
                File.WriteAllText(decksFilePath, "");
              }
            }
          }
        }
      }
    }

    public void savePlayerDecks()
    {
      /*
      if (allDecks.Count > 0)
      {
        StartCoroutine(savePlayerDecksToServer());
      }
      */
      StartCoroutine(savePlayerDecksToServer());
    }

    public IEnumerator savePlayerDecksToServer()
    {
      string url = apiUrl + "users/" + myID + "/decks";
      AllDecklists allPlayerDecks = new AllDecklists();
      allPlayerDecks.decks = new List<Decklist>(allDecks);
      string decksJson = JsonUtility.ToJson(allPlayerDecks);
      byte[] bytes = Encoding.UTF8.GetBytes(decksJson);
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      request.Dispose();
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
      return targetCard;
    }

    public void loadPlayerDraftPacks()
    {
      StartCoroutine(getPlayerDraftPacksFromServer());
    }

    private IEnumerator getPlayerDraftPacksFromServer()
    {
      string url = apiUrl + "users/" + myID + "/draftPacks";
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
          DraftPacks packs = new DraftPacks();
          packs = JsonUtility.FromJson<DraftPacks>(serverJson);
        }
      }
    }

    public void deletePlayerDrafts()
    {
      StartCoroutine(deletePlayerDraftsInServer());
    }

    private IEnumerator deletePlayerDraftsInServer()
    {
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.myID;
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbDELETE;
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      request.Dispose();
    }

    public void deletePlayerLobbies()
    {
      StartCoroutine(deletePlayerLobbiesInServer());
    }

    private IEnumerator deletePlayerLobbiesInServer()
    {
      string url = PlayerManager.Instance.apiUrl + "lobbies/" + PlayerManager.Instance.myID;
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbDELETE;
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      request.Dispose();
    }

    public IEnumerator fetchPlayerObjectivesFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/dailies";
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
          dailyObjectives = new DailyObjectives();
          dailyObjectives = JsonUtility.FromJson<DailyObjectives>(serverJson);
          objectivesPanel.GetComponent<ObjectivesPanel>().updateObjectives();
        }
      }
    }

    public IEnumerator addPlayerCurrenciesInServer(int coins, int gems)
    {
      string url = apiUrl + "users/" + myID + "/currency";
      PlayerCurrencies currency = new PlayerCurrencies();
      currency.coins = coins;
      currency.gems = gems;
      byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(currency));
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      request.Dispose();
    }

    private IEnumerator keepConnectionAliveSignal()
    {
      yield return new WaitForSeconds(5f);
      int failureCount = 0;
      int maxFailures = 10;
      while (failureCount < maxFailures)
      {
        string url = apiUrl + "ping";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
          yield return request.SendWebRequest();
          if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
          {
            Debug.Log(request.error);
            Debug.Log("Server Ping FAILED");
            failureCount++;
          }
          yield return new WaitForSeconds(30f);
        }
      }
    }

    public IEnumerator deleteChallenges()
    {
      string url = apiUrl + "users/" + myID + "/challenges";
      using (UnityWebRequest request = new UnityWebRequest(url))
      {
        request.method = UnityWebRequest.kHttpVerbDELETE;
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
        }
      }
    }

    public IEnumerator fetchPlayerCollectionFromServer()
    {
      string url = apiUrl + "player/" + myID + "/cards";
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
          Pack receivedCards = JsonUtility.FromJson<Pack>(serverJson);
          collectedCards = new Dictionary<string, int>();
          foreach (string cardId in receivedCards.cards)
          {
            collectedCards.Add(cardId, 1);
          }
        }
      }
    }

    public IEnumerator addPackToPlayerCollectionInServer(Pack pack)
    {
      string url = apiUrl + "player/" + myID + "/cards";
      byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(pack));
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Debug.Log("Successfully added packed cards to collection in server");
      }
      request.Dispose();
    }

    public IEnumerator registerWin(int winnerId)
    {
      string gameId = getActiveGameId();
      string url = apiUrl + "activegames/" + gameId + "/winner";
      ActiveGame activeGame = new ActiveGame();
      activeGame.hostId = Int32.Parse(gameId.Split('-')[0]);
      activeGame.guestId = Int32.Parse(gameId.Split('-')[1]);
      activeGame.winner = "" + winnerId;
      byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(activeGame));
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Debug.Log("Successfully registered game result");
      }
      request.Dispose();
    }

    public string getActiveGameId()
    {
      int hostId = -1;
      int guestId = -1;
      if (role == "guest")
      {
        hostId = opponentID;
        guestId = myID;
      }
      else
      {
        hostId = myID;
        guestId = opponentID;
      }
      string gameId = "" + hostId + "-" + guestId;
      return gameId;
    }

    public Pack getColourPack(string colour) // Colours: R | G | W | B (Black) | U (Blue)
    {
      Pack colourPack = new Pack();
      colourPack.cards = new List<string>();
      // Generate 14 random indexes from cardSets
      List<int> setIndexes = new List<int>();
      for (int i = 0; i < 14; i++)
      {
        int randomIndex = UnityEngine.Random.Range(0, cardCollection.Count);
        setIndexes.Add(randomIndex);
      }
      // Get a card from each set of the specified colour and rarity
      for (int i = 0; i < setIndexes.Count; i++)
      {
        CardSet set = cardCollection[setIndexes[i]];
        if (i == 0) // Rare
        {
          List<CardInfo> allColourRares = set.getAllOfRarityAndColour("rare", colour);
          while (allColourRares.Count == 0)
          {
            int newSetIndex = UnityEngine.Random.Range(0, cardCollection.Count);
            set = cardCollection[newSetIndex];
            allColourRares = set.getAllOfRarityAndColour("rare", colour);
          }
          int randomRareIndex = UnityEngine.Random.Range(0, allColourRares.Count);
          colourPack.cards.Add(allColourRares[randomRareIndex].id);
        }
        else if (i < 4) // Uncommon
        {
          List<CardInfo> allColourUncommons = set.getAllOfRarityAndColour("uncommon", colour);
          while (allColourUncommons.Count == 0)
          {
            int newSetIndex = UnityEngine.Random.Range(0, cardCollection.Count);
            set = cardCollection[newSetIndex];
            allColourUncommons = set.getAllOfRarityAndColour("uncommon", colour);
          }
          int randomUncommonIndex = UnityEngine.Random.Range(0, allColourUncommons.Count);
          colourPack.cards.Add(allColourUncommons[randomUncommonIndex].id);
        }
        else // Common
        {
          List<CardInfo> allColourCommons = set.getAllOfRarityAndColour("common", colour);
          while (allColourCommons.Count == 0)
          {
            int newSetIndex = UnityEngine.Random.Range(0, cardCollection.Count);
            set = cardCollection[newSetIndex];
            allColourCommons = set.getAllOfRarityAndColour("common", colour);
          }
          int randomCommonIndex = UnityEngine.Random.Range(0, allColourCommons.Count);
          colourPack.cards.Add(allColourCommons[randomCommonIndex].id);
        }
      }
      // Return the pack
      return colourPack;
    }

    public Pack getLegendaryPack()
    {
      Pack legendaryPack = new Pack();
      legendaryPack.cards = new List<string>();
      List<int> emptySets = new List<int>();
      while (legendaryPack.cards.Count < 3) {
        // Select a set at random
        int randomIndex = UnityEngine.Random.Range(0, cardCollection.Count);
        if (!emptySets.Contains(randomIndex)) {
          CardSet set = cardCollection[randomIndex];
          // Query all legendaries in set
          List<CardInfo> allLegendaries = set.getAllLegendaries();
          if (allLegendaries.Count > 0) {
            // Select a card at random
            int randomLegendaryIndex = UnityEngine.Random.Range(0, allLegendaries.Count);
            legendaryPack.cards.Add(allLegendaries[randomLegendaryIndex].id);
          }
          else
          {
            emptySets.Add(randomIndex);
          }
        }
      }
      return legendaryPack;
    }

    public Pack getArtPack()
    {
      Pack artPack = new Pack();
      artPack.cards = new List<string>();
      List<int> emptySets = new List<int>();
      while (artPack.cards.Count < 3) {
        // Select a set at random from Throne of Eldraine (ELD) forward
        int eldraineIndex = 0;
        for (int i = 0; i < cardCollection.Count; i++)
        {
          if (cardCollection[i].setCode == "ELD")
          {
            eldraineIndex = i;
            break;
          }
        }
        int randomIndex = UnityEngine.Random.Range(eldraineIndex, cardCollection.Count);
        if (!emptySets.Contains(randomIndex)) {
          CardSet set = cardCollection[randomIndex];
          // Query all legendaries in set
          List<CardInfo> allAlternateArts = set.getAllAlternateArts();
          if (allAlternateArts.Count > 0) {
            // Select a card at random
            int randomAlternateArtIndex = UnityEngine.Random.Range(0, allAlternateArts.Count);
            artPack.cards.Add(allAlternateArts[randomAlternateArtIndex].id);
          }
          else
          {
            emptySets.Add(randomIndex);
          }
        }
      }
      return artPack;
    }
}
