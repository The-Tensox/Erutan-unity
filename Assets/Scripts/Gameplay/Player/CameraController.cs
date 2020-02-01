using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Player
{
    public class CameraController : MonoBehaviour {
    
        [SerializeField] private float MovementSpeed = 1.0f;
        [SerializeField] private float DezoomSpeed = 1000f;
        [SerializeField] private float RotationSpeed = 3.0f;

        private float _eulerX;
        private float _eulerY;
    
        void Update () {
            // Zoom / dezoom
            transform.position += Vector3.up * -Input.GetAxis("Mouse ScrollWheel") * DezoomSpeed;
            
            //Get Forward face
            Vector3 dir = transform.forward;
            //Reset/Ignore y axis
            dir.y = 0;
            dir.Normalize();

            // Move position with arrows around
            transform.position += (transform.right * Input.GetAxis("Horizontal") + dir * Input.GetAxis("Vertical")) * MovementSpeed;

            // Rotate camera on Y axis
            if(Input.GetMouseButton(1)) {
                transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * RotationSpeed, 0));
                _eulerX = transform.rotation.eulerAngles.x;
                _eulerY = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(_eulerX, _eulerY, 0);
            }
        }
    }
}