using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OppCardStack : MonoBehaviour
{
  public GameObject browserObject;
  public GameObject cardPrefab;
  public GameObject counterObject;
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
      cardInstance.GetComponent<OppStackCard>().visible = cardsVisibility[i];
      cardInstance.GetComponent<OppStackCard>().hideIfInvisible();
    }
    // Open card browser
    browser.showBrowser(gameObject.name);
  }

  // Get list of card IDs in this stack (! Add visibilities here)
  public List<string> getStackIds()
  {
    return new List<string>(cards);
  }

  public void showNumCards()
  {
    counterObject.GetComponent<CanvasGroup>().alpha = 1;
    counterObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    counterObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + cards.Count;
  }

  public void hideNumCards()
  {
    counterObject.GetComponent<CanvasGroup>().alpha = 0;
    counterObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    counterObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "";
  }
}
