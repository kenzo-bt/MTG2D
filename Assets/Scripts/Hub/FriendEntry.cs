using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendEntry : MonoBehaviour
{
    public GameObject statusObject;
    public GameObject challengeObject;
    public GameObject acceptObject;
    public GameObject nameObject;
    public GameObject panelObject;
    private TMP_Text friendText;
    private Image status;
    private Image challengeIcon;
    private Color onlineColor;
    private Color offlineColor;
    public string friendName;
    public int friendID;

    void Awake()
    {
      friendText = nameObject.GetComponent<TMP_Text>();
      status = statusObject.GetComponent<Image>();
      challengeIcon = challengeObject.GetComponent<Image>();
      onlineColor = new Color(0.23f, 0.77f, 0f, 1f);
      offlineColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setData(string name, int id)
    {
      friendName = name;
      friendID = id;
      friendText.text = friendName;
    }

    public void setPanelObject(GameObject panel)
    {
      panelObject = panel;
    }

    public void removeFriend()
    {
      panelObject.GetComponent<FriendsPanel>().removeFriend(friendID);
    }

    public void sendChallenge()
    {
      panelObject.GetComponent<Matchmaker>().sendChallenge(friendID);
    }

    public void turnToChallenger()
    {
      challengeObject.SetActive(false);
      acceptObject.SetActive(true);
    }

    public void turnToIdle()
    {
      challengeObject.SetActive(true);
      acceptObject.SetActive(false);
    }

    public void acceptChallenge()
    {
      // Set opponent ID to the ID of this player
      PlayerManager.Instance.opponentID = friendID;
      Debug.Log("OpponentID: " + PlayerManager.Instance.opponentID);
      // Set the challenge to accepted in the server
      panelObject.GetComponent<Matchmaker>().acceptChallenge(friendID);
    }
}
