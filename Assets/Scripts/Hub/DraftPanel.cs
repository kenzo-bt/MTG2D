using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DraftPanel : MonoBehaviour
{
    public GameObject entryList;
    // Start is called before the first frame update
    void Start()
    {

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

    public void createDraft()
    {
      StartCoroutine(createDraftInServer());
    }

    public IEnumerator createDraftInServer()
    {
      // Initialize the draft
      Draft myDraft = new Draft();
      myDraft.hostId = PlayerManager.Instance.myID;
      myDraft.capacity = 3;
      myDraft.set = "DMR";
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
          Debug.Log("There are currently " + allDrafts.drafts.Count + " active drafts in the server...");
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
}
