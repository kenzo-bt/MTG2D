using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterExpander : MonoBehaviour
{
    public GameObject expanderButtons;
    public GameObject expanderArrow;
    private bool isExpanded;
    // Start is called before the first frame update
    void Start()
    {
      isExpanded = false;
      collapse();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Expand/Collapse the expander
    public void toggleExpander()
    {
      if (isExpanded)
      {
        collapse();
      }
      else
      {
        expand();
      }
      isExpanded = !isExpanded;
    }

    private void expand()
    {
      expanderArrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 90f);
      expanderButtons.SetActive(true);
      LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.gameObject.GetComponent<RectTransform>());
    }

    private void collapse()
    {
      expanderArrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 270f);
      expanderButtons.SetActive(false);
      LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.gameObject.GetComponent<RectTransform>());
    }
}
