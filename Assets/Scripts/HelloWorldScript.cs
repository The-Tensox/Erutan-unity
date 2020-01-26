using System.Collections.Generic;
using System.Threading.Tasks;
using Erutan.Scripts.Sessions;
using Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts
{
    public class HelloWorldScript : MonoBehaviour {
    
    private float _lastSent;
    private async void Start() {
      Record.Log(SessionManager.Instance.Client.ToString());
      await SessionManager.Instance.Client.Init();
    }

    void OnApplicationQuit() {
      SessionManager.Instance.Client.Logout();
    }
  }
}
