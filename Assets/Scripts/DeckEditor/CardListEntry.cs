using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardListEntry : MonoBehaviour
{
    private string cardId;
    private string cardName;
    private int quantity;
    CardInfo card;
    public GameObject nameObject;
    public GameObject quantityObject;
    public GameObject manaCostObject;
    public GameObject manaPrefab;
    private TMP_Text nameText;
    private TMP_Text quantityText;
    private Image nameBackground;

    // Start is called before the first frame update
    void Awake()
    {
        cardName = "";
        cardId = "";
        quantity = 0;
        card = null;
        nameText = nameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        quantityText = quantityObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        nameBackground = nameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set the name and quantity properties
    public void setValues(string id, int num)
    {
      card = PlayerManager.Instance.getCardFromLookup(id);
      cardId = card.id;
      cardName = card.name;
      quantity = num;
      createManaCost();
      colorize();
      updateEntry();
    }

    // Colorize the entry name header
    public void colorize()
    {
      string computedColour = "A"; // Fallback
      if (card.types.Contains("Artifact"))
      {
        computedColour = "A";
      }
      else if (card.colourIdentity.Count > 1)
      {
        computedColour = "M";
      }
      else
      {
        if (card.colourIdentity[0] == "W") { computedColour = "W"; }
        else if (card.colourIdentity[0] == "U") { computedColour = "U"; }
        else if (card.colourIdentity[0] == "B") { computedColour = "B"; }
        else if (card.colourIdentity[0] == "R") { computedColour = "R"; }
        else if (card.colourIdentity[0] == "G") { computedColour = "G"; }
      }
      Texture2D background = Resources.Load("Images/Entries/" + computedColour) as Texture2D;
      nameBackground.sprite = Sprite.Create(background, new Rect(0, 0, background.width, background.height), new Vector2(0.5f, 0.5f));
    }

    // Pass name and quantity to the game objects
    public void updateEntry()
    {
      nameText.text = cardName;
      quantityText.text = quantity + "x";
    }

    // Increase card frequency
    public void addFrequency()
    {
      // Update master data
      Decklist deck = PlayerManager.Instance.selectedDeck;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      deck.addCard(card);
      // Refresh decklist panel
      transform.parent.parent.parent.gameObject.GetComponent<DeckListPanel>().updatePanel();
    }

    // Decrease card frequency
    public void removeFrequency()
    {
      // Update master data
      Decklist deck = PlayerManager.Instance.selectedDeck;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      deck.removeCard(card);
      // Refresh decklist panel
      transform.parent.parent.parent.gameObject.GetComponent<DeckListPanel>().updatePanel();
    }

    // Creates the mana cost string
    public void createManaCost()
    {
      List<string> costString = new List<string>(card.manaCost.Split('}'));
      costString.Remove("");
      for (int i = 0; i < costString.Count; i++)
      {
        costString[i] = costString[i].Substring(1);
      }
      foreach (string symbol in costString)
      {
        GameObject manaSymbol = Instantiate(manaPrefab, manaCostObject.transform);
        manaSymbol.GetComponent<ManaSymbol>().paintSymbol(symbol);
      }
    }
}
