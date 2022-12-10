using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanel : MonoBehaviour
{
    public GameObject deckDisplayObject;
    private Vector3 targetPosition;
    private float speed;
    private float showPosX;
    private float hidePosX;

    // Start is called before the first frame update
    void Start()
    {
      speed = 500f;
      showPosX = 528.76f;
      hidePosX = 837.24f;
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
