using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedFilters : MonoBehaviour
{
    public GameObject[] manaToggles;
    private RectTransform rect;
    private Vector3 targetPosition;
    private float speed;
    private float hiddenPos;
    private float shownPos;
    // Start is called before the first frame update
    void Start()
    {
      speed = 1000f;
      rect = GetComponent<RectTransform>();
      hiddenPos = rect.localPosition.y;
      shownPos = rect.localPosition.y + rect.sizeDelta.y;
      hide();
      selectAllManaFilters();
    }

    // Update is called once per frame
    void Update()
    {
      var step =  speed * Time.deltaTime; // calculate distance to move
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
    }

    public void show()
    {
      targetPosition = new Vector3(rect.localPosition.x, shownPos, rect.localPosition.z);
    }

    public void hide()
    {
      targetPosition = new Vector3(rect.localPosition.x, hiddenPos, rect.localPosition.z);
    }

    public void selectAllManaFilters()
    {
      foreach (GameObject filter in manaToggles)
      {
        filter.GetComponent<IconFilter>().filterClicked();
      }
    }
}
