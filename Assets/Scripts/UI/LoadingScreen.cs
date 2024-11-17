using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingSpriteObject;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show()
    {
      GetComponent<CanvasGroup>().alpha = 1;
      GetComponent<CanvasGroup>().blocksRaycasts = true;
      loadingSpriteObject.SetActive(true);
    }

    public void hide()
    {
      GetComponent<CanvasGroup>().alpha = 0;
      GetComponent<CanvasGroup>().blocksRaycasts = false;
      loadingSpriteObject.SetActive(false);
    }
}
