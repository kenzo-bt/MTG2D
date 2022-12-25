using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
    }
}
