using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBrowser : MonoBehaviour
{
    public GameObject deckDisplayPrefab;
    public int currentPage;
    public int decksPerPage;

    // Start is called before the first frame update
    void Start()
    {
      currentPage = 0;
      showDecks(currentPage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Fetch previous page of decks
    public void browseBack()
    {
      if (currentPage > 0)
      {
        currentPage--;
        removeDecks();
        showDecks(currentPage);
      }
    }

    // Fetch next page of decks
    public void browseForward()
    {
      if (((currentPage + 1) * decksPerPage) < PlayerManager.Instance.allDecks.Count)
      {
        currentPage++;
        removeDecks();
        showDecks(currentPage);
      }
    }

    // Remove existing deck displays from browser
    private void removeDecks()
    {
      int numChildren = transform.childCount;

      for (int i = 0; i < numChildren; i++)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
    }

    // Order decks centered around deckbrowser width
    private void orderDecks()
    {
      float deckDisplayWidth = deckDisplayPrefab.GetComponent<RectTransform>().sizeDelta.x;
      float browserWidth = deckDisplayWidth * transform.childCount;
      float maxBrowserWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      if (browserWidth >= maxBrowserWidth)
      {
        browserWidth = maxBrowserWidth;
      }
      float individualSpace = browserWidth / transform.childCount;
      float positionX = 0 - (browserWidth / 2) + (individualSpace / 2);

      foreach (Transform child in transform)
      {
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }
    }

    // Show a page of decks in the browser
    private void showDecks(int page)
    {
      int startingIndex = page * decksPerPage;
      for (int i = startingIndex; i < (startingIndex + decksPerPage); i++)
      {
        if (i >= PlayerManager.Instance.allDecks.Count)
        {
          break;
        }

        Decklist deck = PlayerManager.Instance.allDecks[i];
        GameObject deckDisplayInstance = Instantiate(deckDisplayPrefab, transform);
        DeckDisplay display = deckDisplayInstance.GetComponent<DeckDisplay>();
        display.setDisplayData(deck.name, deck.cards[0]);
      }

      orderDecks();
    }
}
