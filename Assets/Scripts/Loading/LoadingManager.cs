using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public string myRole;
    // Start is called before the first frame update
    void Start()
    {
      myRole = PlayerManager.Instance.role;
      if (myRole == "challenger")
      {
        //InvokeRepeating("checkIfGuestReady", 3.0f, 3.0f);
      }
      else if (myRole == "guest")
      {

      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void checkIfGuestReady()
    {
      StartCoroutine(checkGuestReadyInServer());
    }

    IEnumerator checkGuestReadyInServer()
    {
      // Get the challenge from server to check if accept value has changed

    }
    */
}
