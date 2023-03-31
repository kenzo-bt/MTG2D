using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropSmall : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public GameObject player;
    Vector3 beginPosition;
    // Start is called before the first frame update
    void Start()
    {
      beginPosition = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      beginPosition = GetComponent<RectTransform>().localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
      GetComponent<RectTransform>().anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      string cardId = GetComponent<WebCard>().cardId;
      CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
      bool cardVisibility = GetComponent<HandCard>().shown;
      float yPos = GetComponent<RectTransform>().localPosition.y;
      if (yPos > 350f)
      {
        player.GetComponent<Player>().removeCardFromHand(transform.GetSiblingIndex());
        player.GetComponent<Player>().dropCardInBattlefield(card);
        player.GetComponent<Player>().hideHighlightCard();
      }
      else
      {
        foreach (Transform child in transform.parent)
        {
          float thisPosX = GetComponent<RectTransform>().localPosition.x;
          float childPosX = child.gameObject.GetComponent<RectTransform>().localPosition.x;
          if (thisPosX < childPosX)
          {
            // Remove and insert
            player.GetComponent<Player>().removeCardFromHand(transform.GetSiblingIndex());
            player.GetComponent<Player>().insertCardInHand(child.GetSiblingIndex(), card, cardVisibility);
            player.GetComponent<Player>().hideHighlightCard();
            return;
          }
        }
        // Add to end of hand
        int endIndex = transform.parent.childCount - 1;
        player.GetComponent<Player>().removeCardFromHand(transform.GetSiblingIndex());
        player.GetComponent<Player>().insertCardInHand(endIndex, card, cardVisibility);
        player.GetComponent<Player>().hideHighlightCard();
      }
    }
}
