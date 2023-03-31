using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void highlightButton()
    {
      Color highlightColor = new Color(0.18f, 0.5f, 0.56f, 1f);
      GetComponent<Image>().color = highlightColor;
    }

    public void unhighlightButton()
    {
      GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
    }
}
