using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public class Hasher : MonoBehaviour
{
  public string getHash(string input)
  {
    MD5 md5 = MD5.Create();
    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
    byte[] hash = md5.ComputeHash(inputBytes);

    // Convert byte array to hex string
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < hash.Length; i++)
    {
      sb.Append(hash[i].ToString("X2"));
    }

    return sb.ToString();
  }
}
