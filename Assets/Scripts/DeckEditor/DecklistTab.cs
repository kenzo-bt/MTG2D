using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DecklistTab : MonoBehaviour
{
    public GameObject background;
    public GameObject text;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void activate()
    {
      foreach (Transform child in transform.parent)
      {
        child.gameObject.GetComponent<DecklistTab>().deactivate();
      }
      background.GetComponent<Image>().color = new Color(1f, 0.52f, 0f, 1f);
      text.GetComponent<TMP_Text>().color = new Color(0f, 0f, 0f, 1f);
    }

    private void deactivate()
    {
      background.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 1f);
      text.GetComponent<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
    }
}
