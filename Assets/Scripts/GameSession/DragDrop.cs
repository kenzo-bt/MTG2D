using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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
      float yPos = GetComponent<RectTransform>().localPosition.y;
      if (yPos > 500f)
      {
        string cardId = GetComponent<WebCard>().cardId;
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        player.GetComponent<Player>().removeCardFromHand(transform.GetSiblingIndex());
        player.GetComponent<Player>().dropCardInBattlefield(card);
        player.GetComponent<Player>().hideHighlightCard();
      }
      else
      {
        GetComponent<RectTransform>().localPosition = beginPosition;
      }
    }
}
