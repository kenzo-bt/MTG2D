using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpeningDisplayCard : MonoBehaviour
{
    private GameObject displayCard;
    private GameObject displayCardBack;
    public GameObject duplicateCoinsObject;
    public GameObject duplicateCoinsText;
    public float targetScale;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator shrink(float time)
    {
      RectTransform objectTransform = GetComponent<RectTransform>();
      while (objectTransform.localScale.x > targetScale)
      {
          float newScaleValue = objectTransform.localScale.x - (Time.deltaTime / time);
          objectTransform.localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
          yield return null;
      }
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
      displayCardBack = GameObject.Find("OpeningDisplayCardBack");
      displayCard.GetComponent<CanvasGroup>().alpha = 1;
      displayCard.GetComponent<WebCard>().texturizeCard(card);
      if (card.hasBackSide())
      {
        CardInfo backCard = PlayerManager.Instance.getCardFromLookup(card.backId);
        displayCardBack.GetComponent<CanvasGroup>().alpha = 1;
        displayCardBack.GetComponent<WebCard>().texturizeCard(backCard);
      }
    }

    public void hideCardInDisplay()
    {
      displayCard.GetComponent<CanvasGroup>().alpha = 0;
      displayCardBack.GetComponent<CanvasGroup>().alpha = 0;
    }
}
