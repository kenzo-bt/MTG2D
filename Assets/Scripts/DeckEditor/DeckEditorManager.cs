using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckEditorManager : MonoBehaviour
{
    private Decklist initialDeck;

    // Start is called before the first frame update
    void Start()
    {
      initialDeck = new Decklist(PlayerManager.Instance.selectedDeck);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Exit with no changes
    public void exitNoChanges()
    {
      // Prompt user if they are sure. If yes -> reset to initialDeck. If no -> stay here
      PlayerManager.Instance.selectedDeck.name = initialDeck.name;
      PlayerManager.Instance.selectedDeck.cards = new List<CardInfo>(initialDeck.cards);
      PlayerManager.Instance.selectedDeck.cardFrequencies = new List<int>(initialDeck.cardFrequencies);
    }
}
