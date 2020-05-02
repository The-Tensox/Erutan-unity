using System;
using Sessions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class DebugHelper : MonoBehaviour
    {
        [SerializeField] private GameObject managersPrefab;
        private void Start()
        {
            // If not started from login (debug), instanciate managers & connect
            if (SessionManager.Instance == null)
            {
                Instantiate(managersPrefab).GetComponent<HelloWorldScript>().Connect();
            }
        }
    }
}