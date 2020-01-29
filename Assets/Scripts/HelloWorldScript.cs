using System.Collections.Generic;
using System.Threading.Tasks;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts
{
    public class HelloWorldScript : MonoBehaviour {
    
    private float _lastSent;
    private void Start() {
      Record.Log(SessionManager.Instance.Client.ToString());
      SessionManager.Instance.Client.Init();
    }

    void OnApplicationQuit() {
      SessionManager.Instance.Client.Logout();
    }
  }
}
