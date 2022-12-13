using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject buttonText;
    private float hideTextX;
    private float showTextX;
    private float speed;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
      speed = 500f;
      hideTextX = buttonText.transform.localPosition.x;
      showTextX = buttonText.transform.localPosition.x + buttonText.GetComponent<RectTransform>().sizeDelta.x;
      targetPosition = buttonText.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
      var step =  speed * Time.deltaTime; // calculate distance to move
      buttonText.transform.localPosition = Vector3.MoveTowards(buttonText.transform.localPosition, targetPosition, step);
    }

    public void expandButtonText()
    {
      targetPosition = new Vector3(showTextX, buttonText.transform.localPosition.y, buttonText.transform.localPosition.z);
    }

    public void collapseButtonText()
    {
      targetPosition = new Vector3(hideTextX, buttonText.transform.localPosition.y, buttonText.transform.localPosition.z);
    }
}
