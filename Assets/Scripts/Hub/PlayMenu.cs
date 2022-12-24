using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayMenu : MonoBehaviour
{
    private CanvasGroup playMenu;
    public GameObject selectPanel;
    public GameObject deckBrowser;
    public GameObject opponentName;

    // Start is called before the first frame update
    void Start()
    {
        playMenu = GetComponent<CanvasGroup>();
        playMenu.alpha = 0;
        playMenu.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Show play menu
    public void show()
    {
      playMenu.alpha = 1;
      playMenu.blocksRaycasts = true;
      string oppName = "---";
      for(int i = 0; i < PlayerManager.Instance.friendIDs.Count; i++)
      {
        if (PlayerManager.Instance.friendIDs[i] == PlayerManager.Instance.opponentID)
        {
          oppName = PlayerManager.Instance.friendNames[i];
          PlayerManager.Instance.opponentName = oppName;
        }
      }
      opponentName.GetComponent<TMP_Text>().text = oppName;
      selectPanel.GetComponent<SelectPanel>().showPanel();
    }

    // Hide play menu
    public void hide()
    {
      playMenu.alpha = 0;
      playMenu.blocksRaycasts = false;
      selectPanel.GetComponent<SelectPanel>().hidePanel();
      deckBrowser.GetComponent<DeckBrowser>().hideBrowser();
    }
}
