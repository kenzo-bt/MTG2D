using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    public GameObject browserObject;
    public GameObject cardPrefab;
    public GameObject player;
    public List<string> cards;
    private CardBrowser browser;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addCard(string id)
    {
      cards.Add(id);
    }

    public void initializeStack()
    {
      cards = new List<string>();
      browser = browserObject.GetComponent<CardBrowser>();
    }

    public void showStack()
    {
      // Empty browser
      browser.emptyBrowser();
      // Add cards to browser
      Debug.Log("Cards in this stack: " + cards.Count);
      foreach (string cardID in cards)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardID);
        Debug.Log("Creating -> " + card.name);
        GameObject cardInstance = Instantiate(cardPrefab, browser.carousel.transform);
        cardInstance.GetComponent<WebCard>().texturizeCard(card);
        cardInstance.GetComponent<StackCard>().player = player;
      }
      // Open card browser
      browser.showBrowser(gameObject.name);
    }
}
