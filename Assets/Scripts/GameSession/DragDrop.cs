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
      Debug.Log("Being dragged...");
      GetComponent<RectTransform>().anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      Debug.Log("Drag stopped...");
      float yPos = GetComponent<RectTransform>().localPosition.y;
        Debug.Log("Local Y position was: " + yPos);
      if (yPos > 500f)
      {
        string cardId = GetComponent<WebCard>().cardId;
        Debug.Log("CardID: " + cardId);
        CardInfo card = PlayerManager.Instance.getCardFromLookup(cardId);
        player.GetComponent<Player>().dropCardInBattlefield(card);
      }
      else
      {
        GetComponent<RectTransform>().localPosition = beginPosition;
      }
    }
}
