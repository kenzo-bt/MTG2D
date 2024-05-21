using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JsonNetworking : MonoBehaviour
{
    // Not sure if it makes sense to make this behaviour generic, especially if contextual variables are important to sequential operations.

    public void sendJson(string url, string json)
    {
      StartCoroutine(sendToServer(url, json));
    }

    IEnumerator sendToServer(string url, string json)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(json);
      // Create the web request and set the correct properties
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = UnityWebRequest.kHttpVerbPOST;
      request.uploadHandler = new UploadHandlerRaw (bytes);
      request.uploadHandler.contentType = "application/json";
      yield return request.SendWebRequest();
      // Debug the results
      if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
      {
        Debug.Log(request.error);
      }
      // Dispose of the request to prevent memory leaks
      request.Dispose();
    }
}
