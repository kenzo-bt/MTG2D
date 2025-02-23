using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppBattlefieldCard : MonoBehaviour
{
    public GameObject player;
    public GameObject hideLayer;
    public GameObject cardImage;
    public GameObject counterTracker;
    public bool hidden;
    public bool faceDown;

    // Start is called before the first frame update
    void Awake()
    {
      hidden = false;
      faceDown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showInGameHighlight()
    {
      string id = GetComponent<WebCard>().cardId;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
      if (!hidden && !faceDown)
      {
        if (card.hasEnglishTranslation())
        {
          id = card.variations[0];
        }
        player.GetComponent<Player>().showHighlightCard(id);
      }
      if (faceDown && card.text.Contains("Disguise {"))
      {
        player.GetComponent<Player>().showHighlightCard("f5b1634e-a14b-51d6-a679-356f7f0ac60f");
      }
    }

    public void hideInGameHighlight()
    {
      string id = GetComponent<WebCard>().cardId;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
      if (!hidden && !faceDown)
      {
        player.GetComponent<Player>().hideHighlightCard();
      }
      if (faceDown && card.text.Contains("Disguise {"))
      {
        player.GetComponent<Player>().hideHighlightCard();
      }
    }

    public void tapCard()
    {
      transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
    }

    public void flip180()
    {
      cardImage.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
    }

    public void hideCard()
    {
      hideLayer.GetComponent<CanvasGroup>().alpha = 1f;
      hidden = true;
    }

    public void showCounterTracker()
    {
      counterTracker.SetActive(true);
    }

    public void turnFaceDown()
    {
      faceDown = true;
    }
}
