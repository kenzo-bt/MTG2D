using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    private GameObject displayImage;
    private GameObject displayText;
    public string deckName;
    public string coverCard;

    // Awake is called on instantiation
    void Awake()
    {
      displayImage = transform.GetChild(0).gameObject;
      displayText = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Sets and shows the display cover and deckname
    public void setDisplayData(string deckName, string coverCard)
    {
      this.deckName = deckName;
      this.coverCard = coverCard;
      displayText.GetComponent<TMP_Text>().text = deckName;
      Image image = displayImage.GetComponent<Image>();
      Texture2D cardTexture = Resources.Load("Images/Cards/" + coverCard) as Texture2D;
      image.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f));
    }

    // Set this deck as the user selectedDeck
    public void setAsSelectedDeck()
    {
      Debug.Log(deckName);
      foreach (Decklist deck in PlayerManager.Instance.allDecks)
      {
        if (deck.name == deckName)
        {
          PlayerManager.Instance.selectedDeck = deck;
          break;
        }
      }

      DeckDisplay selectPanelDeck = GameObject.Find("SelectPanelDeck").GetComponent<DeckDisplay>();
      string selectedName = PlayerManager.Instance.selectedDeck.name;
      string selectedCover = PlayerManager.Instance.selectedDeck.cards[0];
      selectPanelDeck.setDisplayData(selectedName, selectedCover);
      selectPanelDeck.debugDeckName();
      // Hide the deck add button
      GameObject.Find("AddDeckButton").GetComponent<CanvasGroup>().alpha = 0;
    }

    // Debug deck name
    public void debugDeckName()
    {
      Debug.Log(deckName);
    }

    //
}
