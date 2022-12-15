using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardCollection : MonoBehaviour
{
    public GameObject filtersObject;
    public GameObject displayObject;
    public GameObject searchInputObject;
    private List<string> allCardIds;
    private List<string> filteredIds;
    private int cardsPerPage;
    private int currentPage;
    private static List<string> colourFilters; // B, G, R, W, bl(U)e
    private bool onlyLands;
    private bool onlyMultiColoured;
    private bool onlyColourless;
    private string searchInputText;
    private static List<string> searchKeywords;

    // Start is called before the first frame update
    void Start()
    {
      currentPage = 0;
      cardsPerPage = 10;
      colourFilters = new List<string>();
      allCardIds = new List<string>();
      loadCollection();
      // TODO: pre-apply filters based on the selected deck's colours
      onlyLands = false;
      onlyMultiColoured = false;
      onlyColourless = false;
      searchKeywords = new List<string>();
      searchInputText = searchInputObject.GetComponent<TMP_InputField>().text;

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
      hideAllCards();
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

    // Make all cards in the display transparent
    private void hideAllCards()
    {
      foreach (Transform child in displayObject.transform)
      {
        child.GetChild(0).GetComponent<WebCard>().makeTransparent();
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

    // Filter down the collection based on the selected filters
    private void filterCollection()
    {
      // Reset filteredIds to include all cards
      filteredIds = new List<string>(allCardIds);

      // Apply filters
      if (onlyLands)
      {
        filteredIds = new List<string>(filteredIds.FindAll(isLand));
      }
      else
      {
        filteredIds = new List<string>(filteredIds.FindAll(notLand));
      }

      if (onlyMultiColoured)
      {
        filteredIds = new List<string>(filteredIds.FindAll(isMultiColour));
      }

      if (onlyColourless)
      {
        filteredIds = new List<string>(filteredIds.FindAll(isColourless));
      }
      else
      {
        filteredIds = new List<string>(filteredIds.FindAll(hasSelectedColours));
      }

      if (searchInputText != "")
      {
        filteredIds = new List<string>(filteredIds.FindAll(matchesSearchText));
      }

      // Go back to page one on the display
      currentPage = 0;
      // Update the display with the filtered cards
      updateCollectionDisplay();
    }

    //// Filter togglers

    public void toggleColourFilter(string colour)
    {
      if (colourFilters.Contains(colour))
      {
        colourFilters.Remove(colour);
      }
      else
      {
        colourFilters.Add(colour);
      }
      filterCollection();
    }

    public void toggleLands()
    {
      onlyLands = !onlyLands;
      filterCollection();
    }

    public void toggleMulticolor()
    {
      onlyMultiColoured = !onlyMultiColoured;
      filterCollection();
    }

    public void toggleColourless()
    {
      onlyColourless = !onlyColourless;
      filterCollection();
    }

    public void updateSearchText()
    {
      searchInputText = searchInputObject.GetComponent<TMP_InputField>().text.ToLower();
      searchKeywords = new List<string>(searchInputText.Split(','));
      for (int i = 0; i < searchKeywords.Count; i++)
      {
        searchKeywords[i] = searchKeywords[i].Trim();
        Debug.Log(searchKeywords[i] + " -> " + searchKeywords[i].Length);
      }
      filterCollection();
    }

    //// Filter predicates

    private static bool isLand(string id)
    {
      return PlayerManager.Instance.getCardFromLookup(id).types.Contains("Land");
    }

    private static bool notLand(string id)
    {
      return !PlayerManager.Instance.getCardFromLookup(id).types.Contains("Land");
    }

    private static bool isMultiColour(string id)
    {
      if (PlayerManager.Instance.getCardFromLookup(id).colours.Count > 1 || PlayerManager.Instance.getCardFromLookup(id).colourIdentity.Count > 1)
      {
        return true;
      }
      return false;
    }

    private static bool isColourless(string id)
    {
      if (PlayerManager.Instance.getCardFromLookup(id).colours.Count == 0 || PlayerManager.Instance.getCardFromLookup(id).colourIdentity.Count == 0)
      {
        return true;
      }
      return false;
    }

    private static bool hasSelectedColours(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      foreach (string colour in colourFilters)
      {
        if (targetCard.colours.Contains(colour) || targetCard.colourIdentity.Contains(colour))
        {
          return true;
        }
      }
      return false;
    }

    private static bool matchesSearchText(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      foreach (string keyword in searchKeywords)
      {
        if (!targetCard.text.ToLower().Contains(keyword) && !targetCard.name.ToLower().Contains(keyword))
        {
          return false;
        }
      }
      return true;
    }
}
