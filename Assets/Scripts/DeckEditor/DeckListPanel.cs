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
    private TMP_InputField deckNameInput;
    private TMP_Text cardCountText;
    private ManaCurveDisplay manaCurve;

    // Start is called before the first frame update
    void Start()
    {
      deck = PlayerManager.Instance.selectedDeck;
      deckNameInput = deckNameInputObject.GetComponent<TMP_InputField>();
      cardCountText = cardCount.GetComponent<TMP_Text>();
      manaCurve = manaCurveObject.GetComponent<ManaCurveDisplay>();

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
    }

    // populate the cardList with the selected deck
    public void populateCardList()
    {
      // Sort by manaValue
      int maxValue = 0;
      foreach (CardInfo card in deck.cards)
      {
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
      for (int i = 0; i < deck.cards.Count; i++)
      {
        valuesArray[deck.cards[i].manaValue].Add(i);
      }
      List<CardInfo> sortedCards = new List<CardInfo>();
      List<int> sortedFrequencies = new List<int>();
      for (int i = 0; i < valuesArray.Length; i++)
      {
        if (valuesArray[i].Count > 0)
        {
          foreach (int index in valuesArray[i])
          {
            sortedCards.Add(deck.cards[index]);
            sortedFrequencies.Add(deck.cardFrequencies[index]);
          }
        }
      }

      // Instantiate each entry (no  lands)
      for (int i = 0; i < sortedCards.Count; i++)
      {
        if (!sortedCards[i].types.Contains("Land"))
        {
          // Instantiate a list entry prefab in the card list
          GameObject entryInstance = Instantiate(entryPrefab, cardList.transform);
          entryInstance.GetComponent<CardListEntry>().setValues(sortedCards[i].id, sortedFrequencies[i]);
        }
      }

      // Instantiate each entry (only lands)
      for (int i = 0; i < sortedCards.Count; i++)
      {
        if (sortedCards[i].types.Contains("Land"))
        {
          // Instantiate a list entry prefab in the card list
          GameObject entryInstance = Instantiate(entryPrefab, cardList.transform);
          entryInstance.GetComponent<CardListEntry>().setValues(sortedCards[i].id, sortedFrequencies[i]);
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
      Debug.Log("Panel -> Received request to show card with ID: " + id);
      // Activate highlight card
      CardInfo targetCard = PlayerManager.Instance.getCardFromLookup(id);
      highlightCardObject.SetActive(true);
      highlightCardObject.GetComponent<WebCard>().texturizeCard(targetCard);
    }

    public void hideHighlight()
    {
      Debug.Log("Panel -> Received request to hide highlight.");
      // Deactivate highlight card
      highlightCardObject.SetActive(false);
    }
}
