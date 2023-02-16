using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class DraftCollection : MonoBehaviour
{
    public GameObject displayObject;
    public GameObject addButton;
    public GameObject saveButton;
    public GameObject panelObject;
    public GameObject leftPlayerObject;
    public GameObject rightPlayerObject;
    public GameObject statusMessageObject;
    private int cardsPerPage;
    private int currentPage;
    private List<Pack> initialPacks;
    private List<string> cardIds;
    private List<int> playerIds;
    private int leftPlayerId;
    private int rightPlayerId;
    private int selectedIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initializeCollection()
    {
      currentPage = 0;
      cardsPerPage = 10;
      cardIds = new List<string>();
      playerIds = new List<int>();
      initialPacks = new List<Pack>();
      leftPlayerId = -1;
      rightPlayerId = -1;
      selectedIndex = -1;
      // Get all 3 initial packs
      StartCoroutine(fetchInitialPacksFromServer());
      // Determine left and right players
      StartCoroutine(fetchDraftPlayers());
    }

    public IEnumerator fetchInitialPacksFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/draftPacks";
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
          DraftPacks packs = JsonUtility.FromJson<DraftPacks>(serverJson);
          foreach (Pack pack in packs.draftPacks)
          {
            initialPacks.Add(pack);
          }
          openPack();
        }
      }
    }

    public void openPack()
    {
      cardIds = new List<string>(initialPacks[0].cards);
      initialPacks.RemoveAt(0);
      updateDisplay();
      foreach (Transform child in displayObject.transform)
      {
        child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
      }
      statusMessageObject.GetComponent<TMP_Text>().text = "";
    }

    public void fetchDraftPack()
    {
      StartCoroutine(consumePackFromQueue());
    }

    public IEnumerator consumePackFromQueue()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + PlayerManager.Instance.myID + "/draftQueue/consume";
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          Debug.Log("There was a connection or protocol error in consumePackFromQueue(). Retrying...");
          StartCoroutine(consumePackFromQueue());
        }
        else
        {
          string serverJson = request.downloadHandler.text;
          Pack draftPack = JsonUtility.FromJson<Pack>(serverJson);
          cardIds = new List<string>(draftPack.cards);
          updateDisplay();
          foreach (Transform child in displayObject.transform)
          {
            child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
          }
          if (draftPack.cards.Count == 0)
          {
            statusMessageObject.GetComponent<TMP_Text>().text = "Waiting for " + rightPlayerObject.GetComponent<TMP_Text>().text + " to pass a pack.";
            Debug.Log("Queue was empty.");
            yield return new WaitForSeconds(5);
            Debug.Log("Trying to fetch from queue again...");
            StartCoroutine(consumePackFromQueue());
          }
          else
          {
            statusMessageObject.GetComponent<TMP_Text>().text = "";
          }
        }
      }
    }

    public IEnumerator fetchDraftPlayers()
    {
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID + "/players";
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
          AllPlayers draftPlayers = JsonUtility.FromJson<AllPlayers>(serverJson);
          playerIds = new List<int>(draftPlayers.players);
          for (int i = 0; i < playerIds.Count; i++)
          {
            if (playerIds[i] == PlayerManager.Instance.myID)
            {
              leftPlayerId = (i > 0) ? playerIds[i - 1] : playerIds[playerIds.Count - 1];
              rightPlayerId = (i < (playerIds.Count - 1)) ? playerIds[i + 1] : playerIds[0];
              break;
            }
          }
          StartCoroutine(initializeLeftPlayerName());
          StartCoroutine(initializeRightPlayerName());
        }
      }
    }

    public IEnumerator initializeLeftPlayerName()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + leftPlayerId;
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
          User user = JsonUtility.FromJson<User>(serverJson);
          leftPlayerObject.GetComponent<TMP_Text>().text = user.username;
        }
      }
    }

    public IEnumerator initializeRightPlayerName()
    {
      string url = PlayerManager.Instance.apiUrl + "users/" + rightPlayerId;
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
          User user = JsonUtility.FromJson<User>(serverJson);
          rightPlayerObject.GetComponent<TMP_Text>().text = user.username;
        }
      }
    }

    // Update the display
    public void updateDisplay()
    {
      hideAllCards();
      List<CardInfo> cardsToDisplay = new List<CardInfo>();
      int index = currentPage * cardsPerPage;
      for (int i = index; i < (index + cardsPerPage); i++)
      {
        if (i >= cardIds.Count)
        {
          break;
        }

        // Get card from lookup
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(cardIds[i]);
        cardsToDisplay.Add(targetCard);
      }

      for (int i = 0; i < cardsToDisplay.Count; i++)
      {
        // Texturize the card display card
        GameObject collectionCardObject = displayObject.transform.GetChild(i).gameObject;
        WebCard card = collectionCardObject.transform.GetChild(1).GetComponent<WebCard>();
        CollectionCard collectionCard = collectionCardObject.GetComponent<CollectionCard>();

        collectionCard.setId(cardsToDisplay[i].id);
        card.makeVisible();
        card.texturizeCard(cardsToDisplay[i]);
      }
    }

    // Make all cards in the display transparent
    private void hideAllCards()
    {
      foreach (Transform child in displayObject.transform)
      {
        child.GetChild(1).GetComponent<WebCard>().makeTransparent();
      }
    }

    // Browse to the next page of the collection
    public void browseToNextPage()
    {
      if (((currentPage + 1) * cardsPerPage) < cardIds.Count)
      {
        currentPage++;
        updateDisplay();
      }
      foreach (Transform child in displayObject.transform)
      {
        child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
      }
      if (selectedIndex >= (currentPage * 10) && selectedIndex < ((currentPage + 1) * 10))
      {
        displayObject.transform.GetChild(selectedIndex - (currentPage * 10)).gameObject.GetComponent<DraftCollectionCard>().highlightCard();
      }
    }

    // Browse to the previous page of the collection
    public void browseToPreviousPage()
    {
      if (currentPage > 0)
      {
        currentPage--;
        updateDisplay();
      }
      foreach (Transform child in displayObject.transform)
      {
        child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
      }
      if (selectedIndex >= (currentPage * 10) && selectedIndex < ((currentPage + 1) * 10))
      {
        displayObject.transform.GetChild(selectedIndex - (currentPage * 10)).gameObject.GetComponent<DraftCollectionCard>().highlightCard();
      }
    }

    // Save the index of the selected card and activate the add button
    public void selectCardByIndex(int index)
    {
      if (index == -1)
      {
        selectedIndex = -1;
        addButton.SetActive(false);
      }
      else
      {
        // Selected index arithmetic
        selectedIndex = index + (10 * currentPage);
        addButton.SetActive(true);
      }
    }

    // Add selected card to deck
    public void addSelectedToDeck()
    {
      string cardId = cardIds[selectedIndex];
      PlayerManager.Instance.selectedDeck.addCard(cardId);
      panelObject.GetComponent<DeckListPanel>().updatePanel();
      addButton.SetActive(false);
      selectedIndex = -1;
      currentPage = 0;
      cardIds.Remove(cardId);
      if (cardIds.Count == 0)
      {
        if (initialPacks.Count > 0) // Open the next pack
        {
          openPack();
        }
        else // Save deck and exit
        {
          updateDisplay();
          foreach (Transform child in displayObject.transform)
          {
            child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
          }
          saveButton.SetActive(true);
          if (PlayerManager.Instance.draftHostID == PlayerManager.Instance.myID)
          {
            StartCoroutine(deleteDraftInServer());
          }
        }
      }
      else
      {
        StartCoroutine(sendPackToLeftPlayer());
      }
    }

    public IEnumerator sendPackToLeftPlayer()
    {
      Pack packToSend = new Pack();
      packToSend.cards = new List<string>(cardIds);
      string packJson = JsonUtility.ToJson(packToSend);
      string postUrl = PlayerManager.Instance.apiUrl + "users/" + leftPlayerId + "/draftQueue";
      byte[] bytes = Encoding.UTF8.GetBytes(packJson);
      UnityWebRequest postRequest = new UnityWebRequest(postUrl);
      postRequest.method = UnityWebRequest.kHttpVerbPOST;
      postRequest.uploadHandler = new UploadHandlerRaw (bytes);
      postRequest.uploadHandler.contentType = "application/json";
      yield return postRequest.SendWebRequest();
      if(postRequest.result == UnityWebRequest.Result.ConnectionError || postRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(postRequest.error);
        Debug.Log("There was a connection or protocol error in sendPackToLeftPlayer(). Retrying...");
        StartCoroutine(sendPackToLeftPlayer());
      }
      else
      {
        fetchDraftPack();
      }
      postRequest.Dispose();
    }

    // Save deck changes
    public void saveDeck()
    {
      PlayerManager.Instance.selectedDeck.name = getValidName(PlayerManager.Instance.selectedDeck.name);
      PlayerManager.Instance.savePlayerDecks();
      SceneManager.LoadScene("Hub");
    }

    // Generate a valid deck name if there are repeats
    public string getValidName(string name)
    {
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;

      int nameHit = 0;
      if (name == "")
      {
        name = "DraftDeck";
        nameHit += 1;
      }

      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name == name)
        {
          nameHit += 1;
        }
      }
      if (nameHit == 1)
      {
        return name;
      }

      int copyHit = 0;
      string copyName = name + " (v.";
      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name.Contains(copyName))
        {
          copyHit += 1;
        }
      }
      return copyName + (copyHit + 1) + ")";
    }

    public IEnumerator deleteDraftInServer()
    {
      string url = PlayerManager.Instance.apiUrl + "drafts/" + PlayerManager.Instance.draftHostID;
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbDELETE;
      yield return request.SendWebRequest();
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        Debug.Log("Successfully deleted draft in server");
      }
      request.Dispose();
    }
}
