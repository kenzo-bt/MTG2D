using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsPanel : MonoBehaviour
{
  public float speed;
  public bool expanded;
  public GameObject toggleImage;
  // Start is called before the first frame update
  void Start()
  {
    speed = 500f;
    expanded = true;
  }

  public IEnumerator hidePanel()
  {
    Vector3 targetPosition = new Vector3(transform.localPosition.x + GetComponent<RectTransform>().sizeDelta.x, transform.localPosition.y, transform.localPosition.z);
    while (transform.localPosition.x != targetPosition.x)
    {
      var step =  speed * Time.deltaTime;
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
      yield return null;
    }
    toggleImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
  }

  public IEnumerator showPanel()
  {
    Vector3 targetPosition = new Vector3(transform.localPosition.x - GetComponent<RectTransform>().sizeDelta.x, transform.localPosition.y, transform.localPosition.z);
    while (transform.localPosition.x != targetPosition.x)
    {
      var step =  speed * Time.deltaTime;
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
      yield return null;
    }
    toggleImage.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
  }

  public void togglePanel()
  {
    if (expanded)
    {
      StartCoroutine(hidePanel());
    }
    else
    {
      StartCoroutine(showPanel());
    }
    expanded = !expanded;
  }
}
