using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        display.setDisplayData(deck.name, deck.cards[0]);
      }
    }
}
