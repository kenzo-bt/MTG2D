using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    public GameObject displayText;
    public GameObject displayCard;
    public GameObject displayDialog;
    public string deckName;
    public CardInfo coverCard;

    // Awake is called on instantiation
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets and shows the display cover and deckname
    public void setDisplayData(string deckName, CardInfo coverCard)
    {
      this.deckName = deckName;
      this.coverCard = coverCard;
      displayText.GetComponent<TMP_Text>().text = deckName;
      // Texturize card
      displayCard.GetComponent<WebCard>().texturizeCard(coverCard);
    }

    // Set this deck as the user selectedDeck
    public void setAsSelectedDeck()
    {
      foreach (Decklist deck in PlayerManager.Instance.allDecks)
      {
        if (deck.name == deckName)
        {
          PlayerManager.Instance.selectedDeck = deck;
          break;
        }
      }
    }

    // Show selected deck in selector screen (Main hub)
    public void showSelectedInHub()
    {
      setAsSelectedDeck();
      // Update the deck display in the selector screen
      DeckDisplay selectPanelDeck = GameObject.Find("SelectPanelDeck").GetComponent<DeckDisplay>();
      string selectedName = PlayerManager.Instance.selectedDeck.name;
      selectPanelDeck.setDisplayData(selectedName, PlayerManager.Instance.selectedDeck.getCoverCard());
      // Hide the add deck button
      GameObject.Find("AddDeckButton").GetComponent<CanvasGroup>().alpha = 0;
    }

    // Enter deck editor
    public void editCurrentDeck()
    {
      setAsSelectedDeck();
      SceneManager.LoadScene("DeckEditor");
    }

    // Delete this deck
    public void deleteThisDeck()
    {
      Debug.Log("My name: " + deckName);
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;
      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name == deckName)
        {
          allDecks.RemoveAt(i);
          break;
        }
      }
      PlayerManager.Instance.savePlayerDecks();
      gameObject.SetActive(false);
      GameObject grid = transform.parent.gameObject;
      Debug.Log("Parent: " + grid.name);
      LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.gameObject.GetComponent<RectTransform>());
    }

    public void hideDialog()
    {
      displayDialog.SetActive(false);
    }

    public void showDialog()
    {
      displayDialog.SetActive(true);
    }

    // Debug deck name
    public void debugDeckName()
    {
      Debug.Log(deckName);
    }
}
