using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftCollectionCard : MonoBehaviour
{
    public GameObject highlightObject;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void toggleHighlight()
    {
      if (highlightObject.activeSelf)
      {
        // Signal collection to deselect card and hide the add button
        transform.parent.parent.gameObject.GetComponent<DraftCollection>().selectCardByIndex(-1);
        unHighlightCard();
      }
      else
      {
        // Signal collection to store the index of the selected card and activate the add button
        transform.parent.parent.gameObject.GetComponent<DraftCollection>().selectCardByIndex(transform.GetSiblingIndex());
        foreach (Transform child in transform.parent)
        {
          child.gameObject.GetComponent<DraftCollectionCard>().unHighlightCard();
        }
        highlightCard();
      }
    }

    public void highlightCard()
    {
      highlightObject.SetActive(true);
    }

    public void unHighlightCard()
    {
      highlightObject.SetActive(false);
    }
}
