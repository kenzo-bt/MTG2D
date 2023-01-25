using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoverCard : MonoBehaviour
{
    public GameObject coverCardObject;
    public GameObject dropdownObject;
    private WebCard coverCard;
    private TMP_Dropdown coverDropdown;

    // Start is called before the first frame update
    void Start()
    {
      coverCard = coverCardObject.GetComponent<WebCard>();
      coverDropdown = dropdownObject.GetComponent<TMP_Dropdown>();
      updateDropdown();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateDropdown()
    {
      if (PlayerManager.Instance.selectedDeck.cards.Count > 0)
      {
        List<string> cardNames = new List<string>();
        foreach (string cardId in PlayerManager.Instance.selectedDeck.cards)
        {
          CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
          cardNames.Add(card.name);
        }
        coverDropdown.ClearOptions();
        coverDropdown.AddOptions(cardNames);
        CardInfo coverCard = PlayerManager.Instance.getCardFromLookup(PlayerManager.Instance.selectedDeck.coverId);
        string coverName = coverCard.name;
        int index = cardNames.IndexOf(coverName);
        if (index == -1)
        {
          index = 0;
        }
        coverDropdown.value = index;

        updateCoverCard();
      }
      else
      {
        PlayerManager.Instance.selectedDeck.coverId = "";
        coverCard.texturizeCard(PlayerManager.Instance.selectedDeck.getCoverCard());
        dropdownObject.SetActive(false);
      }
    }

    public void updateCoverCard()
    {
      string selectedName = coverDropdown.options[coverDropdown.value].text;
      string selectedId = "";
      foreach (string cardId in PlayerManager.Instance.selectedDeck.cards)
      {
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        if (card.name == selectedName)
        {
          selectedId = card.id;
          break;
        }
      }
      if (selectedId != "")
      {
        PlayerManager.Instance.selectedDeck.coverId = selectedId;
      }
      coverCard.texturizeCard(PlayerManager.Instance.selectedDeck.getCoverCard());
      dropdownObject.SetActive(false);
    }

    public void toggleDropdown()
    {
      if (PlayerManager.Instance.selectedDeck.cards.Count > 0)
      {
        dropdownObject.SetActive(!dropdownObject.activeSelf);
      }
    }
}
