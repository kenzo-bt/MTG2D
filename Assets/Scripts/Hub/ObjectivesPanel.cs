using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesPanel : MonoBehaviour
{
    public GameObject objectiveTrackerPrefab;
    private float speed;
    public float showPanelX;
    public float hidePanelX;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
      // Panel movement speed and initial position
      speed = 500f;
      showPanelX = gameObject.transform.localPosition.x;
      targetPosition = gameObject.transform.localPosition;
      // Update the trackers after coming back from another scene
      if (PlayerManager.Instance.loggedIn) {
        updateObjectives();
      }
    }

    // Update is called once per frame
    void Update()
    {
      // Move panel if show/hide
      var step =  speed * Time.deltaTime;
      gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, targetPosition, step);
    }

    public void updateObjectives()
    {
      DailyObjectives dailies = PlayerManager.Instance.dailyObjectives;
      for (int i = 0; i < dailies.objectives.Count; i++) {
        Objective dailyObjective = dailies.objectives[i];
        // Render the objective tracker only if it is still in-progress
        if (dailyObjective.progress < dailyObjective.quantity) {
          GameObject objectiveInstance = Instantiate(objectiveTrackerPrefab, gameObject.transform);
          objectiveInstance.GetComponent<ObjectiveTracker>().setData(dailyObjective.target, dailyObjective.colour, dailyObjective.progress, dailyObjective.quantity);
        }
      }
    }

    public void showPanel()
    {
      targetPosition = new Vector3(showPanelX, gameObject.transform.localPosition.y, 0f);
    }

    public void hidePanel()
    {
      // Update panel hide position based on new panel width
      hidePanelX = gameObject.transform.localPosition.x + gameObject.GetComponent<RectTransform>().sizeDelta.x;
      // Set new target position
      targetPosition = new Vector3(hidePanelX, gameObject.transform.localPosition.y, 0f);
    }
}
