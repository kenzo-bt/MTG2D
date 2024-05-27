using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class TimedDeckPanel : MonoBehaviour
{
    public GameObject entryList;
    public GameObject entryPrefab;
    public GameObject createOverlay;
    public GameObject setDropbox;
    public GameObject errorMessage;
    public GameObject joinButton;
    public GameObject objectivesPanel;
    private List<string> setCodes;
    private List<string> setNames;
    private int dropdownIndex;
    // Start is called before the first frame update
    void Start()
    {
      setCodes = new List<string>();
      setNames = new List<string>();
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
      objectivesPanel.GetComponent<ObjectivesPanel>().showPanel();
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

    public void createChallenge()
    {
      StartCoroutine(createTimeChallengeInServer());
    }

    private IEnumerator createTimeChallengeInServer()
    {
      dropdownIndex = setDropbox.GetComponent<TMP_Dropdown>().value;
      string setCode = setCodes[dropdownIndex];
      Debug.Log("Starting a timed challenge with: " + setCode);
      string url = PlayerManager.Instance.apiUrl + "timechallenge/create/" + PlayerManager.Instance.myID + "/" + setCode;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            string serverJson = request.downloadHandler.text;
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(serverJson);
            errorMessage.GetComponent<TMP_Text>().text = serverError.Error;
          }
        }
        else
        {
          PlayerManager.Instance.timeChallengeHostID = PlayerManager.Instance.myID;
          SceneManager.LoadScene("TimeChallengeWaitRoom");
        }
      }
    }

    public void randomizeSet()
    {
      int randomIndex = UnityEngine.Random.Range(0, setCodes.Count);
      setDropbox.GetComponent<TMP_Dropdown>().value = randomIndex;
    }

    public void refreshEntries()
    {
      deleteEntries();
      StartCoroutine(fetchTimeChallengesFromServer());
    }

    public void deleteEntries()
    {
      int numEntries = entryList.transform.childCount;
      for (int i = 0; i < numEntries; i++)
      {
        DestroyImmediate(entryList.transform.GetChild(0).gameObject);
      }
    }

    public IEnumerator fetchTimeChallengesFromServer()
    {
      string url = PlayerManager.Instance.apiUrl + "timechallenges";
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
          AllTimeChallenges allChallenges = JsonUtility.FromJson<AllTimeChallenges>(serverJson);
          Debug.Log("Found " + allChallenges.timeChallenges.Count + " challenges in server");
          foreach (TimeChallenge challenge in allChallenges.timeChallenges)
          {
            int hostId = challenge.id;
            string hostName = challenge.hostName;
            string setCode = challenge.set;
            string playerNum = "" + challenge.players.Count;
            GameObject entry = Instantiate(entryPrefab, entryList.transform);
            entry.GetComponent<ChallengeListEntry>().setInfo(hostId, hostName, setCode, playerNum);
          }
        }
      }
    }

    public void updateSelection()
    {
      bool challengeSelected = false;
      foreach (Transform entry in entryList.transform)
      {
        if (entry.gameObject.GetComponent<ChallengeListEntry>().selected)
        {
          challengeSelected = true;
          PlayerManager.Instance.timeChallengeHostID = entry.gameObject.GetComponent<ChallengeListEntry>().hostID;
          break;
        }
      }
      if (challengeSelected)
      {
        joinButton.SetActive(true);
      }
      else
      {
        joinButton.SetActive(false);
      }
    }

    public void joinChallengeRoom()
    {
      StartCoroutine(sendTimeChallengeJoinRequest());
    }

    public IEnumerator sendTimeChallengeJoinRequest()
    {
      string url = PlayerManager.Instance.apiUrl + "timechallenge/join/" + PlayerManager.Instance.timeChallengeHostID + "/" + PlayerManager.Instance.myID;
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.Log(request.error);
          if (request.responseCode == 400)
          {
            string serverJson = request.downloadHandler.text;
            GenericServerError serverError = JsonUtility.FromJson<GenericServerError>(serverJson);
            Debug.Log(serverError.Error);
          }
        }
        else
        {
          SceneManager.LoadScene("TimeChallengeWaitRoom");
        }
      }
    }
}
