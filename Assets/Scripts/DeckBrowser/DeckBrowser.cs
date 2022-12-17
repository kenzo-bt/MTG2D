using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBrowser : MonoBehaviour
{

    public GameObject yourDecks;
    public GameObject starterDecks;
    public GameObject deckDisplayPrefab;
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
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;
      foreach (Decklist deck in allDecks)
      {
        GameObject deckDisplayInstance = Instantiate(deckDisplayPrefab, yourDecks.transform);
        DeckDisplay display = deckDisplayInstance.GetComponent<DeckDisplay>();
        display.setDisplayData(deck.name, deck.getCoverCard());
      }
    }

    // Add a deck
    public void addDeck()
    {
      Decklist newDeck = new Decklist();
      PlayerManager.Instance.allDecks.Add(newDeck);
      PlayerManager.Instance.selectedDeck = newDeck;
      SceneManager.LoadScene("DeckEditor");
    }
}
