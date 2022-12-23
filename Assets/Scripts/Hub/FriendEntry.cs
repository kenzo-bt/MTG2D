using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendEntry : MonoBehaviour
{
    public GameObject statusObject;
    public GameObject challengeObject;
    public GameObject nameObject;
    private TMP_Text friendText;
    private Image status;
    private Image challengeIcon;
    private Color onlineColor;
    private Color offlineColor;
    private string friendName;
    private int friendId;

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
      friendId = id;
      friendText.text = friendName;
    }
}
