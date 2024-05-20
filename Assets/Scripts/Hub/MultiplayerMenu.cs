using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerMenu : MonoBehaviour
{
  public GameObject lobbyPanel;
  private float speed;
  private float showPanelX;
  private float hidePanelX;
  private Vector3 initialPosition;
  private Vector3 targetPosition;
  public GameObject objectivesPanel;
  private bool panelVisible;

  void Start()
  {
    // Panel Movement
    initialPosition = lobbyPanel.transform.localPosition;
    speed = 500f;
    showPanelX = initialPosition.x - 400f;
    hidePanelX = initialPosition.x;
    targetPosition = lobbyPanel.transform.localPosition;
    panelVisible = false;
  }

  void Update()
  {
    // Move panel if show/hide
    var step =  speed * Time.deltaTime;
    lobbyPanel.transform.localPosition = Vector3.MoveTowards(lobbyPanel.transform.localPosition, targetPosition, step);
  }

  public void togglePanel()
  {
    if (panelVisible)
    {
      hidePanel();
      objectivesPanel.GetComponent<ObjectivesPanel>().showPanel();
    }
    else
    {
      showPanel();
      objectivesPanel.GetComponent<ObjectivesPanel>().hidePanel();
    }
  }

  public void showPanel()
  {
    targetPosition = new Vector3(showPanelX, 0f, 0f);
    panelVisible = true;
  }

  public void hidePanel()
  {
    targetPosition = new Vector3(hidePanelX, 0f, 0f);
    panelVisible = false;
  }
}
