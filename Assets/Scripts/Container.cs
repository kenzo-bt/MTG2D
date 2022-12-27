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
      float childWidth = childPrefab.GetComponent<RectTransform>().sizeDelta.x * childPrefab.GetComponent<RectTransform>().localScale.x;
      float allChildrenWidth = childWidth * transform.childCount;
      float areaWidth = transform.GetComponent<RectTransform>().sizeDelta.x;
      float spacingOffset = 0f;
      if (allChildrenWidth > areaWidth)
      {
        allChildrenWidth = areaWidth;
        spacingOffset = (childWidth - (allChildrenWidth / transform.childCount)) / transform.childCount;
      }
      float individualSpace = (allChildrenWidth / transform.childCount)- spacingOffset;
      float positionX = 0 - (allChildrenWidth / 2);

      foreach (Transform child in transform)
      {
        child.localPosition = new Vector3(positionX, child.localPosition.y, child.localPosition.z);
        positionX += individualSpace;
      }
    }
}
