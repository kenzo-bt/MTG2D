using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StackCard : MonoBehaviour, IPointerClickHandler
{
    public GameObject player;
    public GameObject contextMenu;
    public GameObject destinationIndicator;
    public GameObject destinationText;
    public string destination;
    private Color clrBattle;
    private Color clrGrave;
    private Color clrExile;
    private Color clrHand;
    private Color clrDeck;
    private Image indicatorImage;
    private TMP_Text indicatorLetter;
    // Start is called before the first frame update
    void Awake()
    {
      destination = "";
      clrBattle = new Color(0.63f, 0f, 0.29f, 1f);
      clrGrave = new Color(0.45f, 0f, 1f, 1f);
      clrExile = new Color(0.38f, 0.38f, 0.38f, 1f);
      clrHand = new Color(0f, 0.57f, 1f, 1f);
      clrDeck = new Color(1f, 0.5f, 0.05f, 1f);
      indicatorImage = destinationIndicator.GetComponent<Image>();
      indicatorLetter = destinationText.GetComponent<TMP_Text>();
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

    public void moveCard(string source)
    {
      // Tell player to move me to a certain destination
      string cardId = GetComponent<WebCard>().cardId;
      player.GetComponent<Player>().moveStackCard(cardId, source, destination);
    }

    public void setDestination(string dest)
    {
      destination = dest;
      updateDestinationIndicator();
      hideContextMenu();
    }

    public void updateDestinationIndicator()
    {
      if (destination == "")
      {
        destinationIndicator.SetActive(false);
      }
      else
      {
        destinationIndicator.SetActive(true);
        if (destination == "Battlefield")
        {
          indicatorLetter.text = "F";
          indicatorImage.color = clrBattle;
        }
        else if (destination == "Grave")
        {
          indicatorLetter.text = "G";
          indicatorImage.color = clrGrave;
        }
        else if (destination == "Exile")
        {
          indicatorLetter.text = "E";
          indicatorImage.color = clrExile;
        }
        else if (destination == "Hand")
        {
          indicatorLetter.text = "H";
          indicatorImage.color = clrHand;
        }
        else if (destination == "Deck")
        {
          indicatorLetter.text = "D";
          indicatorImage.color = clrDeck;
        }
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == PointerEventData.InputButton.Left)
      {
        if (contextMenu.activeSelf)
        {
          hideContextMenu();
        }
        else
        {
          showContextMenu();
          destinationIndicator.SetActive(false);
        }
      }
    }
}
