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
      bool found = false;
      foreach (Decklist deck in PlayerManager.Instance.allDecks)
      {
        if (deck.name == deckName)
        {
          PlayerManager.Instance.selectedDeck = deck;
          found = true;
          break;
        }
      }
      if (!found)
      {
        foreach (Decklist deck in PlayerManager.Instance.starterDecks)
        {
          if (deck.name == deckName)
          {
            PlayerManager.Instance.selectedDeck = deck;
            break;
          }
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
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;
      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name == deckName)
        {
          Debug.Log("Removing deck: " + deckName);
          allDecks.RemoveAt(i);
          break;
        }
      }
      PlayerManager.Instance.savePlayerDecks();
      gameObject.SetActive(false);
      GameObject grid = transform.parent.gameObject;
      LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.gameObject.GetComponent<RectTransform>());
    }

    // Copy starter deck
    public void copyStarterDeck()
    {
      bool found = false;
      Decklist copy = new Decklist();
      List<Decklist> starterDecks = PlayerManager.Instance.starterDecks;
      List<Decklist> proDecks = PlayerManager.Instance.proFeaturedDecks;
      foreach (Decklist deck in starterDecks)
      {
        if (deck.name == deckName)
        {
          found = true;
          copy.name = deckName;
          copy.cards = new List<string>(deck.cards);
          copy.cardFrequencies = new List<int>(deck.cardFrequencies);
          copy.sideboard = new List<string>(deck.sideboard);
          copy.sideboardFrequencies = new List<int>(deck.sideboardFrequencies);
          copy.coverId = deck.coverId;
          break;
        }
      }
      if (!found)
      {
        foreach (Decklist deck in proDecks)
        {
          if (deck.name == deckName)
          {
            copy.name = deckName;
            copy.cards = new List<string>(deck.cards);
            copy.cardFrequencies = new List<int>(deck.cardFrequencies);
            copy.sideboard = new List<string>(deck.sideboard);
            copy.sideboardFrequencies = new List<int>(deck.sideboardFrequencies);
            copy.coverId = deck.coverId;
            break;
          }
        }
      }
      PlayerManager.Instance.allDecks.Add(copy);
      PlayerManager.Instance.selectedDeck = copy;
      SceneManager.LoadScene("DeckEditor");
    }

    public void hideDialog()
    {
      displayDialog.SetActive(false);
    }

    public void showDialog()
    {
      displayDialog.SetActive(true);
    }

    // Update the select deck display in the Hub
    public void updateDeckInDisplay()
    {
      GameObject.Find("SelectPanel").GetComponent<SelectPanel>().updateSelectedDeck();
    }

    // Debug deck name
    public void debugDeckName()
    {
      Debug.Log(deckName);
    }
}
