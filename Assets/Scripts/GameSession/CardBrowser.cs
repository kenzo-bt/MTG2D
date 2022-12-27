using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBrowser : MonoBehaviour
{
    public GameObject carousel;
    public string currentlyDisplaying;
    // Start is called before the first frame update
    void Start()
    {
      currentlyDisplaying = "";
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
      foreach (Transform child in carousel.transform){
        StackCard card = child.gameObject.GetComponent<StackCard>();
        if (card.destination != "")
        {
          card.moveCard(currentlyDisplaying);
          DestroyImmediate(child.gameObject);
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
      GetComponent<CanvasGroup>().alpha = 0f;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
