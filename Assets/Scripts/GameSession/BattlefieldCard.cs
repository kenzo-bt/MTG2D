using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattlefieldCard : MonoBehaviour, IPointerClickHandler
{
    public GameObject player;
    public GameObject contextMenu;
    public GameObject flipButton;
    public GameObject cardImage;
    public bool tapped;
    public bool flipped;
    public bool flipped180;
    // Start is called before the first frame update
    void Awake()
    {
      tapped = false;
      flipped = false;
      flipped180 = false;
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
      int siblingIndex = transform.GetSiblingIndex();
      string cardId = GetComponent<WebCard>().cardId;
      string areaName = transform.parent.gameObject.name;
      player.GetComponent<Player>().moveBattlefieldCard(cardId, siblingIndex, transform.parent.gameObject.name, destination);
    }

    public void tapCard()
    {
      if (tapped)
      {
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
      }
      else
      {
        transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
      }
      tapped = !tapped;
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

    public void flipCard()
    {
      CardInfo card = PlayerManager.Instance.getCardFromLookup(GetComponent<WebCard>().cardId);
      if (card.layout == "flip")
      {
        flip180();
      }
      else
      {
        if (flipped)
        {
          // Texturize as face card
          GetComponent<WebCard>().showFront();
        }
        else
        {
          // Texturize as back side
          GetComponent<WebCard>().showBack();
        }
        flipped = !flipped;
      }
    }

    public void flip180()
    {
      if (flipped180)
      {
        cardImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
      }
      else
      {
        cardImage.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
      }
      flipped180 = !flipped180;
    }

    public void enableFlipButton()
    {
      flipButton.SetActive(true);
    }
}
