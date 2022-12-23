using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddFriend : MonoBehaviour
{
    public GameObject friendsPanelObject;
    public GameObject inputObject;
    public GameObject alertObject;
    private TMP_InputField input;
    private TMP_Text alert;
    private FriendsPanel friendsPanel;

    // Start is called before the first frame update
    void Awake()
    {
      input = inputObject.GetComponent<TMP_InputField>();
      alert = alertObject.GetComponent<TMP_Text>();
      friendsPanel = friendsPanelObject.GetComponent<FriendsPanel>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Add friend functionality
    public void addFriend()
    {
      if (input.text != "")
      {
        friendsPanel.addFriend(input.text);
      }
    }

    // Handle alert message
    public void setAlert(string message)
    {
      alert.text = message;
    }

    public void clearAlert()
    {
      alert.text = "";
    }

    // Show / Hide overlay
    public void hide()
    {
      if (gameObject.activeSelf)
      {
        clearAlert();
        gameObject.SetActive(false);
      }
    }

    public void show()
    {
      gameObject.SetActive(true);
    }
}
