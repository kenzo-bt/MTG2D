using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditorManager : MonoBehaviour
{
    private Decklist initialDeck;
    public GameObject exitPopup;

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
    public void tryExit()
    {
      exitPopup.SetActive(true);
    }

    public void dontExit()
    {
      exitPopup.SetActive(false);
    }

    public void exitNoChange()
    {
      PlayerManager.Instance.selectedDeck.name = initialDeck.name;
      PlayerManager.Instance.selectedDeck.cards = new List<CardInfo>(initialDeck.cards);
      PlayerManager.Instance.selectedDeck.cardFrequencies = new List<int>(initialDeck.cardFrequencies);
      SceneManager.LoadScene("DeckBrowser");
    }
}
