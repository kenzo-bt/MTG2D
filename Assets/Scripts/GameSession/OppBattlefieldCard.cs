using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppBattlefieldCard : MonoBehaviour
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
