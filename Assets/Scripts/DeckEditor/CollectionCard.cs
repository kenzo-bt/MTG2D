using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionCard : MonoBehaviour
{
    public GameObject DeckListPanelObject;
    private DeckListPanel deckPanel;
    private string cardId;
    // Start is called before the first frame update
    void Start()
    {
      deckPanel = DeckListPanelObject.GetComponent<DeckListPanel>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Add card to currently selected deck
    public void addToDeck()
    {
      Decklist deck = PlayerManager.Instance.selectedDeck;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);

      deck.addCard(card);

      deckPanel.updatePanel();
    }

    // Set the card ID of the card being rendered inside
    public void setId(string id)
    {
      cardId = id;
    }

    // Debug card id
    public void debugCardId()
    {
      Debug.Log(cardId);
    }
}
