using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckBrowser : MonoBehaviour
{
    public GameObject yourDecks;
    public GameObject starterDecksObject;
    public GameObject proDecksObject;
    public GameObject deckDisplayPrefab;
    public GameObject starterDeckPrefab;
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
}
