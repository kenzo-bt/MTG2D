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

    // Save deck changes
    public void saveDeck()
    {
      PlayerManager.Instance.selectedDeck.name = getValidName(PlayerManager.Instance.selectedDeck.name);
      PlayerManager.Instance.savePlayerDecks();
      SceneManager.LoadScene("DeckBrowser");
    }

    // Generate a valid deck name if there are repeats
    public string getValidName(string name)
    {
      List<Decklist> allDecks = PlayerManager.Instance.allDecks;

      int nameHit = 0;
      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name == name)
        {
          nameHit += 1;
        }
      }
      if (nameHit == 1)
      {
        return name;
      }

      int copyHit = 0;
      string copyName = name + " (v.";
      for (int i = 0; i < allDecks.Count; i++)
      {
        if (allDecks[i].name.Contains(copyName))
        {
          copyHit += 1;
        }
      }
      return copyName + (copyHit + 1) + ")";
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
