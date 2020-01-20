using Scripts.Sessions;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class HelloWorldScript : MonoBehaviour {

  private float _lastSent;
  private void Start() {
    Connect();
  }

  private void Update() {
    SendPosition();
  }

  public async void Connect()
  {
    var reply = await SessionManager.Instance.ConnectAsync();

    Record.Log(reply ? $"Connected" : "Failed to connect");
  }

  private async void SendPosition()
  {
    if ((Time.time - _lastSent) > 5f) {
      transform.position = Random.insideUnitSphere;
      var reply = await SessionManager.Instance.SendPosition(transform.position);
      Record.Log(reply ? $"Sent position" : "Failed to send position");
      _lastSent = Time.time;
    }
  }

  async void OnApplicationQuit()
  {
    await SessionManager.Instance.DisconnectAsync();
    Debug.Log("Application ending after " + Time.time + " seconds");
  }
}
