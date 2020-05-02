using System;
using Sessions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelloWorldScript : MonoBehaviour
{
  private void Awake()
  {
    SessionManager.Instance.Init("127.0.0.1", 50051);
    SessionManager.Instance.Connected += SessionOnConnected;
    // if (SessionManager.Instance.Client == null) Connect();
  }

  public void Connect()
  {
    SessionManager.Instance.Connect();
  }

  private void SessionOnConnected()
  {
    SceneManager.LoadScene("Main");
  }

  void OnApplicationQuit() {
    SessionManager.Instance.Client.Logout();
  }
}