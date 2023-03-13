using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckListPanel : MonoBehaviour
{
    private Decklist deck;
    public GameObject entryPrefab;
    public GameObject cardList;
    public GameObject deckNameInputObject;
    public GameObject cardCount;
    public GameObject manaCurveObject;
    public GameObject highlightCardObject;
    public GameObject highlightCardBackObject;
    public GameObject coverCardObject;
    private TMP_InputField deckNameInput;
    private TMP_Text cardCountText;
    private ManaCurveDisplay manaCurve;
    private CoverCard coverCard;
    public bool sideboardActive;

    // Start is called before the first frame update
    void Start()
    {
      deck = PlayerManager.Instance.selectedDeck;
      deckNameInput = deckNameInputObject.GetComponent<TMP_InputField>();
      cardCountText = cardCount.GetComponent<TMP_Text>();
      manaCurve = manaCurveObject.GetComponent<ManaCurveDisplay>();
      coverCard = coverCardObject.GetComponent<CoverCard>();
      sideboardActive = false;

      initializePanel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the initial values for the panel (name / card count / card list / mana curve)
    private void initializePanel()
    {
      deckNameInput.text = deck.name;
      cardCountText.text = "Cards: " + deck.getNumCards();
      populateCardList();
      manaCurve.updateManaCurve(deck);
    }

    public void updatePanel()
    {
      // Remove all previous entries
      int numInstances = cardList.transform.childCount;
      for (int i = 0; i < numInstances; i++)
      {
        DestroyImmediate(cardList.transform.GetChild(0).gameObject);
      }
      // Rebuild list
      cardCountText.text = "Cards: " + deck.getNumCards();
      populateCardList();
      manaCurve.updateManaCurve(deck);
      coverCard.updateDropdown();
    }

    // populate the cardList with the selected deck
    public void populateCardList()
    {
      // Deck / Sideboard logic
      List<string> listOfCards = sideboardActive ? new List<string>(deck.sideboard) : new List<string>(deck.cards);
      List<int> listOfFrequencies = sideboardActive ? new List<int>(deck.sideboardFrequencies) : new List<int>(deck.cardFrequencies);

      if (listOfCards.Count > 0)
      {
        // Sort by manaValue
        int maxValue = 0;
        foreach (string cardId in listOfCards)
        {
          CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
          if (card.manaValue > maxValue)
          {
            maxValue = card.manaValue;
          }
        }
        List<int>[] valuesArray = new List<int>[maxValue + 1];
        for (int i = 0; i < valuesArray.Length; i++)
        {
            valuesArray[i] = new List<int>();
        }
        for (int i = 0; i < listOfCards.Count; i++)
        {
          CardInfo card = PlayerManager.Instance.getCardFromLookup(listOfCards[i]);
          valuesArray[card.manaValue].Add(i);
        }
        List<string> sortedCards = new List<string>();
        List<int> sortedFrequencies = new List<int>();
        for (int i = 0; i < valuesArray.Length; i++)
        {
          if (valuesArray[i].Count > 0)
          {
            foreach (int index in valuesArray[i])
            {
              sortedCards.Add(listOfCards[index]);
              sortedFrequencies.Add(listOfFrequencies[index]);
            }
          }
        }

        // Instantiate each entry (no  lands)
        for (int i = 0; i < sortedCards.Count; i++)
        {
          CardInfo card = PlayerManager.Instance.getCardFromLookup(sortedCards[i]);
          if (!card.types.Contains("Land"))
          {
            // Instantiate a list entry prefab in the card list
            GameObject entryInstance = Instantiate(entryPrefab, cardList.transform);
            entryInstance.GetComponent<CardListEntry>().setValues(card.id, sortedFrequencies[i]);
          }
        }

        // Instantiate each entry (only lands)
        for (int i = 0; i < sortedCards.Count; i++)
        {
          CardInfo card = PlayerManager.Instance.getCardFromLookup(sortedCards[i]);
          if (card.types.Contains("Land"))
          {
            // Instantiate a list entry prefab in the card list
            GameObject entryInstance = Instantiate(entryPrefab, cardList.transform);
            entryInstance.GetComponent<CardListEntry>().setValues(card.id, sortedFrequencies[i]);
          }
        }
      }
    }

    // Update deck name
    public void updateDeckName()
    {
      if (deckNameInput.text == "")
      {
        deckNameInput.text = "No Name";
      }
      PlayerManager.Instance.selectedDeck.name = deckNameInput.text;

      updatePanel();
    }

    // Show card highlight next to this panel
    public void highlightCard(string id)
    {
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      if (targetCard.hasEnglishTranslation())
      {
        targetCard = PlayerManager.Instance.getCardFromLookup(targetCard.variations[0]);
      }
      highlightCardObject.SetActive(true);
      highlightCardObject.GetComponent<WebCard>().texturizeCard(targetCard);
      if (targetCard.hasBackSide())
      {
        CardInfo targetBackCard = PlayerManager.Instance.getCardFromLookup(targetCard.backId);
        highlightCardBackObject.SetActive(true);
        highlightCardBackObject.GetComponent<WebCard>().texturizeCard(targetBackCard);
        if (targetCard.layout == "flip")
        {
          highlightCardBackObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
          highlightCardBackObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
      }
    }

    public void hideHighlight()
    {
      highlightCardObject.SetActive(false);
      highlightCardBackObject.SetActive(false);
    }

    public void showSideboard()
    {
      sideboardActive = true;
      updatePanel();
    }

    public void showDeck()
    {
      sideboardActive = false;
      updatePanel();
    }
}
