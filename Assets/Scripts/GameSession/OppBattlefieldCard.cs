using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppBattlefieldCard : MonoBehaviour
{
    public GameObject player;
    public GameObject hideLayer;
    public GameObject cardImage;
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
      if (!hidden && !faceDown)
      {
        string id = GetComponent<WebCard>().cardId;
        CardInfo card = PlayerManager.Instance.getCardFromLookup(id);
        if (card.hasEnglishTranslation())
        {
          id = card.variations[0];
        }
        player.GetComponent<Player>().showHighlightCard(id);
      }
    }

    public void hideInGameHighlight()
    {
      if (!hidden && !faceDown)
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

    public void turnFaceDown()
    {
      faceDown = true;
    }
}
