using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlefieldCard : MonoBehaviour, IPointerClickHandler
{
    public GameObject player;
    public GameObject contextMenu;
    private bool tapped;
    // Start is called before the first frame update
    void Awake()
    {
      tapped = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showContextMenu()
    {
      contextMenu.SetActive(true);
    }

    public void hideContextMenu()
    {
      contextMenu.SetActive(false);
    }

    public void moveCard(string destination)
    {
      // Tell player to move me to a certain destination
      string cardId = GetComponent<WebCard>().cardId;
      string areaName = transform.parent.gameObject.name;
      player.GetComponent<Player>().moveBattlefieldCard(cardId, transform.parent.gameObject.name, destination);
    }

    public void tapCard()
    {
      //GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
      if (tapped)
      {
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
      }
      else
      {
        transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
      }
      tapped = !tapped;
      //GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Left)
      {
        tapCard();
      }
      else if (eventData.button == PointerEventData.InputButton.Right)
      {
        if (contextMenu.activeSelf)
        {
          hideContextMenu();
        }
        else
        {
          showContextMenu();
        }
      }
    }

    public void showInGameHighlight()
    {
      string id = GetComponent<WebCard>().cardId;
      player.GetComponent<Player>().showHighlightCard(id);
    }

    public void hideInGameHighlight()
    {
      player.GetComponent<Player>().hideHighlightCard();
    }
}
