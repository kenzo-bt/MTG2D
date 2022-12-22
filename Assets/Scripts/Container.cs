using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject childPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void orderChildrenCenter()
    {
      int numChildren = transform.childCount;
      float childWidth = childPrefab.GetComponent<RectTransform>().sizeDelta.x;
      float currentWidth = childWidth * numChildren;
      float maxWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      if (currentWidth >= maxWidth)
      {
        currentWidth = maxWidth;
      }
      float individualSpace = currentWidth / numChildren;
      float positionX = 0 - (currentWidth / 2) + (individualSpace / 2);

      foreach (Transform child in transform)
      {
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }
    }
}
