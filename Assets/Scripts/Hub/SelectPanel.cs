using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private float showPosX;
    private float hidePosX;

    // Start is called before the first frame update
    void Start()
    {
      speed = 500f;
      showPosX = transform.localPosition.x;
      hidePosX = transform.localPosition.x + GetComponent<RectTransform>().sizeDelta.x;
      transform.localPosition = new Vector3(hidePosX, 0f, 0f);
      targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
      var step =  speed * Time.deltaTime; // calculate distance to move
      transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step);
    }

    public void showPanel()
    {
      targetPosition = new Vector3(showPosX, 0f, 0f);
    }

    public void hidePanel()
    {
      targetPosition = new Vector3(hidePosX, 0f, 0f);
    }
}
