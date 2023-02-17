using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftEditorManager : MonoBehaviour
{
    public GameObject collectionObject;
    private DraftCollection collection;
    // Start is called before the first frame update
    void Start()
    {
      createDeck();
      collection = collectionObject.GetComponent<DraftCollection>();
      collection.initializeCollection();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Create an empty deck
    public void createDeck()
    {
      Decklist newDeck = new Decklist();
      newDeck.isDraft = true;
      PlayerManager.Instance.allDecks.Add(newDeck);
      PlayerManager.Instance.selectedDeck = newDeck;
    }
}
