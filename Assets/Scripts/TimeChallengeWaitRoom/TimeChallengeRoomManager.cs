using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeChallengeRoomManager : MonoBehaviour
{
    public GameObject roomTitleObject;
    public GameObject statusMessageObject;
    public GameObject playerGridObject;
    public GameObject playerEntryPrefab;
    public GameObject startChallengeButton;
    public List<int> playerIds;
    public string hostName;
    public string setName;
    public string setCode;
    private int objectiveDeckId;
    private bool startingChallenge;

    // Start is called before the first frame update
    void Start()
    {
      playerIds = new List<int>();
      hostName = "";
      setName = "";
      setCode = "";
      objectiveDeckId = -1;
      startingChallenge = false;
      StartCoroutine(getRoomInfoFromServer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator getRoomInfoFromServer()
    {
      while (true)
      {
        if (!startingChallenge)
        {
          string url = PlayerManager.Instance.apiUrl + "timechallenge/" + PlayerManager.Instance.timeChallengeHostID;
          using (UnityWebRequest request = UnityWebRequest.Get(url))
          {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
              Debug.Log(request.error);
              if (request.responseCode == 400)
              {
                GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(request.downloadHandler.text);
                Debug.Log(serverError.Error);
                SceneManager.LoadScene("Hub");
              }
            }
            TimeChallenge timeChallenge = JsonUtility.FromJson<TimeChallenge>(request.downloadHandler.text);
            PlayerManager.Instance.objectiveId = timeChallenge.objectiveId;
            if (timeChallenge.started == true)
            {
              yield return moveToDeckEditor();
            }
            if (PlayerManager.Instance.myID == PlayerManager.Instance.timeChallengeHostID)
            {
              if (!startChallengeButton.activeSelf)
              {
                startChallengeButton.SetActive(true);
              }
              statusMessageObject.GetComponent<TMP_Text>().text = "";
            }
            else
            {
              statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for host to start challenge";
            }
            // Update set code
            if (setCode == "")
            {
              setCode = timeChallenge.set;
            }
            // Update title
            if (hostName == "")
            {
              hostName = timeChallenge.hostName;
              roomTitleObject.GetComponent<TMP_Text>().text = hostName + "\'s Time Challenge Room";
            }
            // Populate room grid
            if (playerIds.Count != timeChallenge.players.Count)
            {
              playerIds = new List<int>();
              clearPlayerGrid();
              foreach (TimeChallengePlayer player in timeChallenge.players)
              {
                GameObject playerEntry = Instantiate(playerEntryPrefab, playerGridObject.transform);
                playerEntry.GetComponent<DraftPlayerEntry>().setUsername(player.username);
                playerEntry.GetComponent<DraftPlayerEntry>().setReady();
                playerIds.Add(player.id);
              }
            }
          }
        }
        yield return new WaitForSeconds(3f);
      }
    }

    public void clearPlayerGrid()
    {
      int numPlayers = playerGridObject.transform.childCount;
      for (int i = 0; i < numPlayers; i++)
      {
        DestroyImmediate(playerGridObject.transform.GetChild(0).gameObject);
      }
    }

    public void beginChallenge()
    {
      startingChallenge = true;
      // Create card lists
      List<Pack> cardLists = new List<Pack>();
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        if (set.setCode == setCode)
        {
          List<CardInfo> allRares = set.getAllRaresNoArtifacts();
          List<int> selectedIndexes = new List<int>();
          foreach (int playerId in playerIds)
          {
            Pack selectedCards = new Pack();
            selectedCards.cards = new List<string>();
            // Select 3 rare/mythics from set
            int numCardsAdded = 0;
            while (numCardsAdded < 3)
            {
              int randomIndex = UnityEngine.Random.Range(0, allRares.Count);
              if (!selectedIndexes.Contains(randomIndex))
              {
                selectedIndexes.Add(randomIndex);
                selectedCards.cards.Add(allRares[randomIndex].id);
                numCardsAdded += 1;
              }
            }
            cardLists.Add(selectedCards);
          }
          break;
        }
      }
      CardLists allLists = new CardLists();
      allLists.cardLists = cardLists;
      // Send create request to server
      StartCoroutine(beginChallengeInServer(allLists));
    }

    private IEnumerator beginChallengeInServer(CardLists lists)
    {
      string url = PlayerManager.Instance.apiUrl + "timechallenge/" + PlayerManager.Instance.timeChallengeHostID + "/start";
      string listsJson = JsonUtility.ToJson(lists);
      byte[] bytes = Encoding.UTF8.GetBytes(listsJson);
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
        if (request.responseCode == 400)
        {
          GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(request.downloadHandler.text);
          Debug.Log(serverError.Error);
        }
      }
      else
      {
        yield return moveToDeckEditor();
      }
      request.Dispose();
    }

    private IEnumerator moveToDeckEditor()
    {
      string url = PlayerManager.Instance.apiUrl + "timechallenge/" + PlayerManager.Instance.timeChallengeHostID + "/getcards/" + PlayerManager.Instance.myID;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(request.downloadHandler.text);
            Debug.Log(serverError.Error);
          }
        }
        else
        {
          // Create objective deck entry in server
          yield return registerObjectiveDeck();
          // Initialize new deck and move to deck editor
          Pack myRares = JsonUtility.FromJson<Pack>(request.downloadHandler.text);
          PlayerManager.Instance.timeChallengeRares = myRares.cards;
          PlayerManager.Instance.timeChallengeSetCode = setCode;
          Decklist newDeck = new Decklist();
          newDeck.cards = new List<string>();
          newDeck.cardFrequencies = new List<int>();
          newDeck.isTimeChallenge = true;
          newDeck.timeChallengeCardSet = setCode;
          newDeck.isTimeChallengeEditable = true;
          newDeck.objectiveId = PlayerManager.Instance.objectiveId;
          newDeck.objectiveDeckId = objectiveDeckId;
          PlayerManager.Instance.allDecks.Add(newDeck);
          PlayerManager.Instance.selectedDeck = newDeck;
          SceneManager.LoadScene("DeckEditor");
        }
      }
    }

    private IEnumerator registerObjectiveDeck()
    {
      string url = PlayerManager.Instance.apiUrl + "objective/deck/" + PlayerManager.Instance.objectiveId;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(request.downloadHandler.text);
            Debug.Log(serverError.Error);
          }
        }
        else
        {
          PlayObjectiveDeck objectiveDeckEntry = JsonUtility.FromJson<PlayObjectiveDeck>(request.downloadHandler.text);
          objectiveDeckId = objectiveDeckEntry.id;
        }
      }
    }

    public void returnToHub()
    {
      StartCoroutine(leaveChallengeRoom());
    }

    public IEnumerator leaveChallengeRoom()
    {
      string url = PlayerManager.Instance.apiUrl + "timechallenge/" + PlayerManager.Instance.timeChallengeHostID + "/exit/" + PlayerManager.Instance.myID;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(request.downloadHandler.text);
            Debug.Log(serverError.Error);
          }
        }
        else
        {
          SceneManager.LoadScene("Hub");
        }
      }
    }

}
