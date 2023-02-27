using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBrowser : MonoBehaviour
{
    public GameObject player;
    public GameObject carousel;
    public string currentlyDisplaying;
    public bool shuffle;
    // Start is called before the first frame update
    void Start()
    {
      currentlyDisplaying = "";
      shuffle = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Empty browser
    public void emptyBrowser()
    {
      int numCards = carousel.transform.childCount;
      for (int i = 0; i < numCards; i++)
      {
        DestroyImmediate(carousel.transform.GetChild(0).gameObject);
      }
    }

    public void exitBrowser()
    {
      processCardDestinations();
      hideBrowser();
    }

    public void processCardDestinations()
    {
      int numChildren = carousel.transform.childCount;
      int cardIndex = 0;
      for (int i = 0; i < numChildren; i++)
      {
        StackCard card = carousel.transform.GetChild(cardIndex).gameObject.GetComponent<StackCard>();
        if (card != null && card.destination != "")
        {
          Debug.Log("Move card: " + carousel.transform.GetChild(cardIndex).gameObject.GetComponent<WebCard>().cardName + " -> " + card.destination);
          card.moveCard(currentlyDisplaying);
          DestroyImmediate(carousel.transform.GetChild(cardIndex).gameObject);
        }
        else
        {
          cardIndex++;
        }
      }
    }

    public void showBrowser(string source)
    {
      currentlyDisplaying = source;
      GetComponent<CanvasGroup>().alpha = 1f;
      GetComponent<CanvasGroup>().blocksRaycasts = true;
      float carouselWidth = carousel.GetComponent<RectTransform>().sizeDelta.x;
      carousel.GetComponent<RectTransform>().localPosition = new Vector3((-carouselWidth / 2), 0f, 0f);
    }

    public void hideBrowser()
    {
      if (shuffle)
      {
        player.GetComponent<Player>().shuffleDeck();
        shuffle = false;
      }
      GetComponent<CanvasGroup>().alpha = 0f;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
