using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCard : MonoBehaviour
{
    public GameObject player;
    public GameObject visibilityIcon;
    public bool shown;

    // Start is called before the first frame update
    void Awake()
    {
      shown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void toggleShowCard()
    {
      if (shown)
      {
        hideHandCard();
      }
      else
      {
        showHandCard();
      }
      shown = !shown;
    }

    public void showHandCard()
    {
      visibilityIcon.GetComponent<CanvasGroup>().alpha = 1f;
      int index = transform.GetSiblingIndex();
      player.GetComponent<Player>().showHandCard(index);
    }

    public void hideHandCard()
    {
      visibilityIcon.GetComponent<CanvasGroup>().alpha = 0.2f;
      int index = transform.GetSiblingIndex();
      player.GetComponent<Player>().hideHandCard(index);
    }
}
