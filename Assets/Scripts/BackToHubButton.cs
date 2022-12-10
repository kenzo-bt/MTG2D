using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHubButton : MonoBehaviour
{
    // Go back to the main hub
    public void backToHub()
    {
      SceneManager.LoadScene("Hub");
    }
}
