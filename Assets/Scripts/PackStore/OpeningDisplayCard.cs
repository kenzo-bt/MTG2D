using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpeningDisplayCard : MonoBehaviour
{
    private GameObject displayCard;
    public GameObject duplicateCoinsObject;
    public GameObject duplicateCoinsText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showDuplicateCoins(int numCoins)
    {
      duplicateCoinsObject.GetComponent<CanvasGroup>().alpha = 1;
      duplicateCoinsText.GetComponent<TMP_Text>().text = "" + numCoins;
    }

    public void showCardInDisplay() {
      string cardId = GetComponent<WebCard>().cardId;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      displayCard = GameObject.Find("OpeningDisplayCard");
      displayCard.GetComponent<CanvasGroup>().alpha = 1;
      displayCard.GetComponent<WebCard>().texturizeCard(card);
    }

    public void hideCardInDisplay()
    {
      displayCard.GetComponent<CanvasGroup>().alpha = 0;
    }
}
