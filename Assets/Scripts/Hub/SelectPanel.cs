using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float showPosX;
    private float hidePosX;
    public GameObject playButtonObject;
    public GameObject deckDisplayObject;
    private DeckDisplay deckDisplay;
    private AcceptButton playButton;

    // Start is called before the first frame update
    void Start()
    {
      speed = 500f;
      showPosX = transform.localPosition.x;
      hidePosX = transform.localPosition.x + GetComponent<RectTransform>().sizeDelta.x;
      transform.localPosition = new Vector3(hidePosX, 0f, 0f);
      targetPosition = transform.localPosition;

      deckDisplay = deckDisplayObject.GetComponent<DeckDisplay>();
      playButton = playButtonObject.GetComponent<AcceptButton>();
      if (PlayerManager.Instance.selectedDeck != null)
      {
        updateSelectedDeck();
        deckDisplayObject.SetActive(true);
      }
      else
      {
        playButton.disable();
        deckDisplayObject.SetActive(false);
      }
    }

    // Update is called once per frame
    void Update()
    {
      var step =  speed * Time.deltaTime; // calculate distance to move
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
    }

    public void showPanel()
    {
      targetPosition = new Vector3(showPosX, 0f, 0f);
    }

    public void hidePanel()
    {
      targetPosition = new Vector3(hidePosX, 0f, 0f);
    }

    public void updateSelectedDeck()
    {
      Decklist selectedDeck = PlayerManager.Instance.selectedDeck;
      Debug.Log("Selected deck: " + selectedDeck.name);
      playButton.enable();
      deckDisplay.setDisplayData(selectedDeck.name, selectedDeck.getCoverCard());
      deckDisplayObject.SetActive(true);
    }
}
