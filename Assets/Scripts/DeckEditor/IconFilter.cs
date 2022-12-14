using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconFilter : MonoBehaviour
{
    public GameObject iconObject;
    public GameObject selectedObject;
    private Image icon;
    private Image selectedFilter;
    private bool amSelected;
    // Start is called before the first frame update
    void Start()
    {
      amSelected = false;
      icon = iconObject.GetComponent<Image>();
      selectedFilter = selectedObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void filterClicked()
    {
      Color filterColor = selectedFilter.color;
      Color iconColor = icon.color;
      if (amSelected)
      {
        filterColor.a = 0f;
        iconColor.a = 0.5f;
      }
      else
      {
        filterColor.a = 1f;
        iconColor.a = 1f;
      }
      selectedFilter.color = filterColor;
      icon.color = iconColor;
      amSelected = !amSelected;
    }
}
