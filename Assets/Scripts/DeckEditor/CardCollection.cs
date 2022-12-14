using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public GameObject filtersObject;
    public GameObject displayObject;
    private List<string> allCardIds;
    private List<string> filteredIds;
    private int cardsPerPage;
    private int currentPage;

    // Start is called before the first frame update
    void Start()
    {
      currentPage = 0;
      cardsPerPage = 10;
      allCardIds = new List<string>();
      loadCollection();
      filteredIds = new List<string>(allCardIds);
      updateCollectionDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Update the display
    public void updateCollectionDisplay()
    {
      List<CardInfo> cardsToDisplay = new List<CardInfo>();
      int index = currentPage * cardsPerPage;
      for (int i = index; i < (index + cardsPerPage); i++)
      {
        if (i >= filteredIds.Count)
        {
          break;
        }

        // Get card from lookup
        CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(filteredIds[i]);
        cardsToDisplay.Add(targetCard);
      }

      for (int i = 0; i < cardsToDisplay.Count; i++)
      {
        // Texturize the card display card
        WebCard card = displayObject.transform.GetChild(i).GetChild(0).GetComponent<WebCard>();
        card.texturizeCard(cardsToDisplay[i]);
      }
    }

    // Load all card ids in collection into the filteredIds list
    public void loadCollection()
    {
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        foreach (CardInfo card in set.cards)
        {
          allCardIds.Add(card.id);
        }
      }
      //Debug.Log(filteredIds.Count + " card IDs loaded.");
    }

    // Browse to the next page of the collection
    public void browseToNextPage()
    {
      if (((currentPage + 1) * cardsPerPage) < filteredIds.Count)
      {
        currentPage++;
        updateCollectionDisplay();
      }
    }

    // Browse to the previous page of the collection
    public void browseToPreviousPage()
    {
      if (currentPage > 0)
      {
        currentPage--;
        updateCollectionDisplay();
      }
    }
}
