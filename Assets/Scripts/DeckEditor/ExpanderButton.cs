using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpanderButton : MonoBehaviour
{
    private bool isSelected;
    public GameObject indicator;
    // Start is called before the first frame update
    void Start()
    {
      isSelected = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Select/Deselect this expander button
    public void toggle()
    {
      Image indicatorFill = indicator.GetComponent<Image>();
      Color tmpColor = indicatorFill.color;
      if (isSelected)
      {
        tmpColor.a = 0f;
      }
      else
      {
        tmpColor.a = 1f;
      }
      indicatorFill.color = tmpColor;
      isSelected = !isSelected;
    }
}
