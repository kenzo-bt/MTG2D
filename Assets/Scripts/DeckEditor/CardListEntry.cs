using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardListEntry : MonoBehaviour
{
    private string cardId;
    private string cardName;
    private int quantity;
    public GameObject nameObject;
    public GameObject quantityObject;
    private TMP_Text nameText;
    private TMP_Text quantityText;

    // Start is called before the first frame update
    void Awake()
    {
        cardName = "";
        cardId = "";
        quantity = 0;
        nameText = nameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        quantityText = quantityObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the name and quantity properties
    public void setValues(string id, int num)
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
      cardId = card.id;
      cardName = card.name;
      quantity = num;
      updateEntry();
    }

    // Pass name and quantity to the game objects
    public void updateEntry()
    {
      nameText.text = cardName;
      quantityText.text = quantity + "x";
    }

    // Increase card frequency
    public void addFrequency()
    {
      // Update master data
      Decklist deck = PlayerManager.Instance.selectedDeck;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      deck.addCard(card);
      // Refresh decklist panel
      transform.parent.parent.parent.gameObject.GetComponent<DeckListPanel>().updatePanel();
    }

    // Decrease card frequency
    public void removeFrequency()
    {
      // Update master data
      Decklist deck = PlayerManager.Instance.selectedDeck;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      deck.removeCard(card);
      // Refresh decklist panel
      transform.parent.parent.parent.gameObject.GetComponent<DeckListPanel>().updatePanel();
    }
}
