using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DraftPanel : MonoBehaviour
{
    public GameObject entryList;
    public GameObject entryPrefab;
    public GameObject createOverlay;
    public GameObject setDropbox;
    public GameObject capacityInput;
    public GameObject errorMessage;
    public GameObject joinButton;
    private List<string> setCodes;
    private List<string> setNames;
    private int capacity;
    private int dropdownIndex;
    // Start is called before the first frame update
    void Start()
    {
      setCodes = new List<string>();
      setNames = new List<string>();
      capacity = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showPanel()
    {
      refreshEntries();
      CanvasGroup cg = GetComponent<CanvasGroup>();
      cg.alpha = 1;
      cg.blocksRaycasts = true;
    }

    public void hidePanel()
    {
      CanvasGroup cg = GetComponent<CanvasGroup>();
      cg.alpha = 0;
      cg.blocksRaycasts = false;
    }

    public void openCreateOverlay()
    {
      // Populate set dropbox
      setNames = new List<string>();
      setCodes = new List<string>();
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        setNames.Add(set.setName);
        setCodes.Add(set.setCode);
      }
      setDropbox.GetComponent<TMP_Dropdown>().ClearOptions();
      setDropbox.GetComponent<TMP_Dropdown>().AddOptions(setNames);
      setDropbox.GetComponent<TMP_Dropdown>().value = 0;
      // Show overlay
      createOverlay.GetComponent<CanvasGroup>().alpha = 1;
      createOverlay.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void closeCreateOverlay()
    {
      createOverlay.GetComponent<CanvasGroup>().alpha = 0;
      createOverlay.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void createDraft()
    {
      // Check if set/capacity has been set correctly
      string capacityString = capacityInput.GetComponent<TMP_InputField>().text;
      try
      {
        capacity = Int32.Parse(capacityString);
        dropdownIndex = setDropbox.GetComponent<TMP_Dropdown>().value;
        if (capacity >= 0)
        {
          errorMessage.GetComponent<TMP_Text>().text = "";
          StartCoroutine(createDraftInServer());
        }
        else
        {
          errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer as the room capacity.";
        }
      }
      catch (FormatException)
      {
        errorMessage.GetComponent<TMP_Text>().text = "Please enter a valid non-negative integer as the room capacity.";
      }
    }

    public IEnumerator createDraftInServer()
    {
      // Initialize the draft
      Draft myDraft = new Draft();
      myDraft.hostId = PlayerManager.Instance.myID;
      myDraft.hostName = PlayerManager.Instance.myName;
      myDraft.capacity = capacity;
      myDraft.set = setCodes[dropdownIndex];
      myDraft.setName = setNames[dropdownIndex];
      myDraft.players = new List<int>();
      myDraft.players.Add(myDraft.hostId);
      // Send to draft to server
      string draftJson = JsonUtility.ToJson(myDraft);
      byte[] bytes = Encoding.UTF8.GetBytes(draftJson);
      string url = PlayerManager.Instance.apiUrl + "drafts";
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      // Debug the results
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      else
      {
        // Send to draft room
        Debug.Log("Draft created successfully in server. Proceeding to draft room...");
        closeCreateOverlay();
        refreshEntries();
      }
      // Dispose of the request to prevent memory leaks
      request.Dispose();
    }

    public void refreshEntries()
    {
      deleteEntries();
      StartCoroutine(fetchDraftsFromServer());
    }

    public IEnumerator fetchDraftsFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "drafts";
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
          AllDrafts allDrafts = JsonUtility.FromJson<AllDrafts>(serverJson);
          foreach (Draft draft in allDrafts.drafts)
          {
            string hostName = draft.hostName;
            string setName = draft.setName;
            string capacity = "" + draft.players.Count + " / " + draft.capacity;
            GameObject entry = Instantiate(entryPrefab, entryList.transform);
            entry.GetComponent<DraftListEntry>().setInfo(hostName, setName, capacity);
          }
        }
      }
    }

    public void deleteEntries()
    {
      int numEntries = entryList.transform.childCount;
      for (int i = 0; i < numEntries; i++)
      {
        DestroyImmediate(entryList.transform.GetChild(0).gameObject);
      }
    }

    public void updateSelection()
    {
      bool draftSelected = false;
      foreach (Transform entry in entryList.transform)
      {
        if (entry.gameObject.GetComponent<DraftListEntry>().selected)
        {
          draftSelected = true;
          break;
        }
      }
      if (draftSelected)
      {
        joinButton.SetActive(true);
      }
      else
      {
        joinButton.SetActive(false);
      }
    }
}
