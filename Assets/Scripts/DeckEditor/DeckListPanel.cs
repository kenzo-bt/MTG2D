using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckListPanel : MonoBehaviour
{
    private Decklist deck;
    public GameObject entryPrefab;
    public GameObject cardList;
    public GameObject deckName;
    public GameObject cardCount;
    public GameObject manaCurveObject;
    private TMP_Text deckNameText;
    private TMP_Text cardCountText;
    private ManaCurveDisplay manaCurve;

    // Start is called before the first frame update
    void Start()
    {
      deck = PlayerManager.Instance.selectedDeck;
      deckNameText = deckName.GetComponent<TMP_Text>();
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
      deckNameText.text = deck.name;
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
      for (int i = 0; i < deck.cards.Count; i++)
      {
        // Instantiate a list entry prefab in the card list
        GameObject entryInstance = Instantiate(entryPrefab, cardList.transform);
        entryInstance.GetComponent<CardListEntry>().setValues(deck.cards[i].id, deck.cardFrequencies[i]);
      }
    }
}
