using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeckBrowser : MonoBehaviour
{
    public GameObject yourDecks;
    public GameObject starterDecksObject;
    public GameObject proDecksObject;
    public GameObject deckDisplayPrefab;
    public GameObject starterDeckPrefab;
    public GameObject deckImportDialogObject;
    public GameObject deckImportInputObject;
    public GameObject importErrorTextObject;
    private TMP_InputField deckInput;

    // Start is called before the first frame update
    void Start()
    {
      loadDecks();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Load decks into browser
    public void loadDecks()
    {
      // Player Decks
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;
      foreach (Decklist deck in allDecks)
      {
        GameObject deckDisplayInstance = Instantiate(deckDisplayPrefab, yourDecks.transform);
        DeckDisplay display = deckDisplayInstance.GetComponent<DeckDisplay>();
        display.setDisplayData(deck.name, deck.getCoverCard());
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(yourDecks.GetComponent<RectTransform>());

      // Starter Decks
      List<Decklist> starterDecks = PlayerManager.Instance.starterDecks;
      foreach (Decklist deck in starterDecks)
      {
        GameObject deckDisplayInstance = Instantiate(starterDeckPrefab, starterDecksObject.transform);
        DeckDisplay display = deckDisplayInstance.GetComponent<DeckDisplay>();
        display.setDisplayData(deck.name, deck.getCoverCard());
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(starterDecksObject.GetComponent<RectTransform>());

      // Featured Pro decks
      List<Decklist> proDecks = PlayerManager.Instance.proFeaturedDecks;
      foreach (Decklist deck in proDecks)
      {
        GameObject deckDisplayInstance = Instantiate(starterDeckPrefab, proDecksObject.transform);
        DeckDisplay display = deckDisplayInstance.GetComponent<DeckDisplay>();
        display.setDisplayData(deck.name, deck.getCoverCard());
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(proDecksObject.GetComponent<RectTransform>());
    }

    // Add a deck
    public void addDeck()
    {
      Decklist newDeck = new Decklist();
      PlayerManager.Instance.allDecks.Add(newDeck);
      PlayerManager.Instance.selectedDeck = newDeck;
      SceneManager.LoadScene("DeckEditor");
    }

    // Toggle visibility of the deck browser
    public void toggleVisibility()
    {
      gameObject.SetActive(!gameObject.activeSelf);
    }

    // Show/Hide
    public void showBrowser(){
      gameObject.SetActive(true);
    }
    public void hideBrowser(){
      gameObject.SetActive(false);
    }

    // Toggle visibility of deck importer dialog
    public void toggleImporterVisibility()
    {
      if (deckImportDialogObject.GetComponent<CanvasGroup>().alpha == 0)
      {
        deckImportDialogObject.GetComponent<CanvasGroup>().alpha = 1;
        deckImportDialogObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
      }
      else
      {
        deckImportDialogObject.GetComponent<CanvasGroup>().alpha = 0;
        deckImportDialogObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
      }
    }

    // Process the inputted text
    public void processDeckImportText()
    {
      importErrorTextObject.GetComponent<TMP_Text>().text = "";
      deckInput = deckImportInputObject.GetComponent<TMP_InputField>();
      List<string> inputLines = new List<string>(deckInput.text.Split('\n'));
      List<string> cardIds = new List<string>();
      List<int> cardFrequencies = new List<int>();
      List<string> sideboardIds = new List<string>();
      List<int> sideboardFrequencies = new List<int>();

      if (inputLines.Count > 1)
      {
        bool importError = false;
        bool sideboard = false;
        for (int i = 0; i < inputLines.Count; i++)
        {
          // Skip the first line: "Deck"
          if (inputLines[i].Trim() == "Deck" || inputLines[i].Trim() == "Commander")
          {
            continue;
          }
          if (inputLines[i].Trim() == "Sideboard")
          {
            sideboard = true;
            continue;
          }
          // Parse the card line if not empty
          if (inputLines[i].Trim() != "")
          {
            string[] lineData = inputLines[i].Split("(");
            string freqAndName = lineData[0];
            string setAndSerial = lineData[1];
            int freqAndNameSeparator = freqAndName.IndexOf(" ");
            int frequency = Int32.Parse(freqAndName.Split(" ")[0]);
            string name = freqAndName.Substring(freqAndNameSeparator).Trim();
            string setCode = setAndSerial.Split(")")[0];
            // Debug.Log($"Card: {name} {setCode} x{frequency}");

            // Remove second name in dual cards
            if (name.Contains(" // "))
            {
              name = name.Split(" // ")[0];
            }

            // Fix differing set codes
            switch(setCode)
            {
              case "DAR":
                setCode = "DOM";
                break;
              default:
                break;
            }

            // Find the card Id and add it to the list
            bool setFound = false;
            bool cardFound = false;
            foreach (CardSet set in PlayerManager.Instance.cardCollection)
            {
              if (setCode == set.setCode)
              {
                // Find the card ID
                foreach (CardInfo card in set.cards)
                {
                  string cardName = card.name;
                  if (cardName.Contains(" // "))
                  {
                    cardName = cardName.Split(" // ")[0];
                  }
                  if (name == cardName)
                  {
                    // Add the ID and frequency to the list
                    if (sideboard)
                    {
                      sideboardIds.Add(card.id);
                      sideboardFrequencies.Add(frequency);
                    }
                    else
                    {
                      cardIds.Add(card.id);
                      cardFrequencies.Add(frequency);
                    }
                    cardFound = true;
                    break;
                  }
                }
                setFound = true;
                break;
              }
            }
            if (!setFound || !cardFound)
            {
              string fallbackCardId = findFallbackCard(name);
              if (fallbackCardId != "")
              {
                // Add the ID and frequency to the list
                if (sideboard)
                {
                  sideboardIds.Add(fallbackCardId);
                  sideboardFrequencies.Add(frequency);
                }
                else
                {
                  cardIds.Add(fallbackCardId);
                  cardFrequencies.Add(frequency);
                }
              }
              else
              {
                importError = true;
                setImportErrorText($"Card '{name}' not found");
              }
            }
          }
        }
        if (!importError)
        {
          // Create the deck object and open the editor
          Decklist importedDeck = new Decklist();
          importedDeck.cards = cardIds;
          importedDeck.cardFrequencies = cardFrequencies;
          if (sideboardIds.Count > 0)
          {
            importedDeck.sideboard = sideboardIds;
            importedDeck.sideboardFrequencies = sideboardFrequencies;
          }
          importedDeck.coverId = cardIds[0];
          PlayerManager.Instance.allDecks.Add(importedDeck);
          PlayerManager.Instance.selectedDeck = importedDeck;
          SceneManager.LoadScene("DeckEditor");
        }
      }
      else
      {
        setImportErrorText("Wrong format");
      }
    }

    public void setImportErrorText(string errorText)
    {
      importErrorTextObject.GetComponent<TMP_Text>().text = $"Error: {errorText}";
    }

    private string findFallbackCard(string name)
    {
      foreach (CardSet set in PlayerManager.Instance.cardCollection)
      {
        foreach (CardInfo card in set.cards)
        {
          string cardName = card.name;
          if (cardName.Contains(" // "))
          {
            cardName = cardName.Split(" // ")[0];
          }
          if (name == cardName)
          {
            return card.id;
          }
        }
      }
      return "";
    }
}
