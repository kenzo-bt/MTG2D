using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    public GameObject browserObject;
    public GameObject cardPrefab;
    public GameObject player;
    public List<string> cards;
    public List<bool> cardsVisibility;
    private CardBrowser browser;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addCard(string id, bool visibility)
    {
      cards.Add(id);
      cardsVisibility.Add(visibility);
    }

    public void removeCard(int index)
    {
      cards.RemoveAt(index);
      cardsVisibility.RemoveAt(index);
    }

    public void initializeStack()
    {
      cards = new List<string>();
      cardsVisibility  = new List<bool>();
      browser = browserObject.GetComponent<CardBrowser>();
    }

    public void showStack()
    {
      // Empty browser
      browser.emptyBrowser();
      // Add cards to browser
      for (int i = 0; i < cards.Count; i++)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cards[i]);
        GameObject cardInstance = Instantiate(cardPrefab, browser.carousel.transform);
        if (cardsVisibility[i])
        {
          cardInstance.GetComponent<WebCard>().texturizeCard(card);
        }
        cardInstance.GetComponent<StackCard>().player = player;
        cardInstance.GetComponent<StackCard>().visible = cardsVisibility[i];
        cardInstance.GetComponent<StackCard>().hideIfInvisible();
      }
      // Open card browser
      browser.showBrowser(gameObject.name);
    }

    // Get list of card IDs in this stack (! Add visibilities here)
    public List<string> getStackIds()
    {
      List<string> allCards = new List<string>();
      for (int i = 0; i < cards.Count; i++)
      {
        string cardId = cards[i];
        if (!cardsVisibility[i])
        {
          cardId += "-H";
        }
        allCards.Add(cardId);
      }
      return allCards;
    }
}
