using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppBattlefieldCard : MonoBehaviour
{
    public GameObject player;
    public GameObject hideLayer;
    public bool hidden;

    // Start is called before the first frame update
    void Awake()
    {
      hidden = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showInGameHighlight()
    {
      if (!hidden)
      {
        string id = GetComponent<WebCard>().cardId;
        player.GetComponent<Player>().showHighlightCard(id);
      }
    }

    public void hideInGameHighlight()
    {
      if (!hidden)
      {
        player.GetComponent<Player>().hideHighlightCard();
      }
    }

    public void tapCard()
    {
      transform.localRotation = Quaternion.Euler(0f, 0f, 25f);
    }

    public void hideCard()
    {
      hideLayer.GetComponent<CanvasGroup>().alpha = 1f;
      hidden = true;
    }
}
