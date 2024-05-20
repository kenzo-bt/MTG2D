using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesPanel : MonoBehaviour
{
    public GameObject objectiveTrackerPrefab;
    // Start is called before the first frame update
    void Start()
    {
      if (PlayerManager.Instance.loggedIn) {
        updateObjectives();
      }
    }

    // Update is called once per frame
    void Update()
    {

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
}
