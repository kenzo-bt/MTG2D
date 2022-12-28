using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppStackCard : MonoBehaviour
{
  public GameObject hiddenLayerObject;
  private CanvasGroup hiddenLayer;
  public bool visible;
  // Start is called before the first frame update
  void Awake()
  {
    visible = true;
    hiddenLayer = hiddenLayerObject.GetComponent<CanvasGroup>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void hideIfInvisible()
  {
    if (!visible)
    {
      hiddenLayer.alpha = 1f;
    }
  }
}
