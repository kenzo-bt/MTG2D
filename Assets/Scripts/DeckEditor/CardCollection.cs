using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    private static List<int> manaFilters; // 0 -> 7+ mana value
    private static List<string> searchKeywords;
    private static List<string> rarities;
    private static List<string> typeFilters;
    private static List<string> setFilters;
    private bool onlyLands;
    private bool onlyMultiColoured;
    private bool onlyColourless;
    private string searchInputText;

    // Start is called before the first frame update
    void Start()
    {
      currentPage = 0;
      cardsPerPage = 10;
      colourFilters = new List<string>();
      manaFilters = new List<int>();
      allCardIds = new List<string>();
      loadCollection();
      // TODO: pre-apply filters based on the selected deck's colours OR load deck (less computation + more intuitive)
      onlyLands = false;
      onlyMultiColoured = false;
      onlyColourless = false;
      searchKeywords = new List<string>();
      rarities = new List<string>();
      typeFilters = new List<string>();
      setFilters = new List<string>();
      searchInputText = searchInputObject.GetComponent<TMP_InputField>().text;

      List<string> selectedDeckIds = new List<string>();
      foreach (CardInfo card in PlayerManager.Instance.selectedDeck.cards)
      {
        selectedDeckIds.Add(card.id);
      }
      filteredIds = new List<string>(selectedDeckIds);
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
        GameObject collectionCardObject = displayObject.transform.GetChild(i).gameObject;
        WebCard card = collectionCardObject.transform.GetChild(0).GetComponent<WebCard>();
        CollectionCard collectionCard = collectionCardObject.GetComponent<CollectionCard>();

        collectionCard.setId(cardsToDisplay[i].id);
        card.makeVisible();
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
          // Dont include DFC backside
          if (!card.isBack)
          {
            allCardIds.Add(card.id);
          }
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
    public void filterCollection()
    {
      //debugAllFilters();
      // Reset filteredIds to include all cards
      filteredIds = new List<string>(allCardIds);

      // Apply filters in waterfall fashion. Start with broadest filter, end with most computationally expensive
      if (setFilters.Count > 0)
      {
        filteredIds = new List<string>(filteredIds.FindAll(inSelectedSets));
      }

      if (rarities.Count > 0)
      {
        filteredIds = new List<string>(filteredIds.FindAll(hasSelectedRarities));
      }

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
        if (colourFilters.Count > 0)
        {
          filteredIds = new List<string>(filteredIds.FindAll(hasSelectedColours));
        }
      }

      if (manaFilters.Count > 0)
      {
        filteredIds = new List<string>(filteredIds.FindAll(hasSelectedManaValues));
      }

      if (searchInputText != "")
      {
        filteredIds = new List<string>(filteredIds.FindAll(matchesSearchText));
      }

      if (typeFilters.Count > 0)
      {
        filteredIds = new List<string>(filteredIds.FindAll(hasSelectedTypes));
      }

      filteredIds = new List<string>(filteredIds.FindAll(notToken));

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

    private static bool hasSelectedManaValues(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      int cardValue = targetCard.manaValue;
      if (cardValue > 7)
      {
        cardValue = 7;
      }
      return manaFilters.Contains(cardValue);
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

    private static bool hasSelectedTypes(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      foreach (string type in targetCard.types)
      {
        if (typeFilters.Contains(type))
        {
          return true;
        }
      }
      return false;
    }

    private static bool inSelectedSets(string id)
    {
      return setFilters.Contains(PlayerManager.Instance.getCardFromLookup(id).set);
    }

    private static bool hasSelectedRarities(string id)
    {
      return rarities.Contains(PlayerManager.Instance.getCardFromLookup(id).rarity);
    }

    private static bool notToken(string id)
    {
      return !PlayerManager.Instance.getCardFromLookup(id).isToken;
    }

    //// Receiving extra filters from Advanced filters

    public void addRarities(string rarityString)
    {
      rarities = new List<string>();
      if (rarityString.Length > 0)
      {
        rarities = new List<string>(rarityString.Split(','));
      }
    }

    public void addManaValues(string manaString)
    {
      manaFilters = new List<int>();
      if (manaString.Length > 0)
      {
        foreach (string value in manaString.Split(','))
        {
          manaFilters.Add(Int32.Parse(value));
        }
      }
    }

    public void addColours(string colourString)
    {
      colourFilters = new List<string>();
      if (colourString.Length > 0)
      {
        colourFilters = new List<string>(colourString.Split(','));
      }
    }

    public void addTypes(string typeString)
    {
      typeFilters = new List<string>();
      if (typeString.Length > 0)
      {
        typeFilters = new List<string>(typeString.Split(','));
      }
    }

    public void addSets(string setString)
    {
      setFilters = new List<string>();
      if (setString.Length > 0)
      {
        setFilters = new List<string>(setString.Split(','));
      }
    }

    // Debug all filters
    private void debugAllFilters()
    {
      Debug.Log("---------------------------");
      Debug.Log("Sets: [" + string.Join(",", setFilters) + "]");
      Debug.Log("Colours: [" + string.Join(",", colourFilters) + "]");
      Debug.Log("Mana: [" + string.Join(",", manaFilters) + "]");
      Debug.Log("Rarities: [" + string.Join(",", rarities) + "]");
      Debug.Log("Types: [" + string.Join(",", typeFilters) + "]");
      Debug.Log("Keywords: [" + string.Join(",", searchKeywords) + "]");
    }
}
