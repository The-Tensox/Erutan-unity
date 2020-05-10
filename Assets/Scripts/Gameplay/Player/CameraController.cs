using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Player
{
    public class CameraController : MonoBehaviour {
    
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float dezoomSpeed = 100f;
        [SerializeField] private float rotationSpeed = 3.0f;

        private float _eulerX;
        private float _eulerY;
    
        void Update () {
            // Zoom / dezoom
            transform.position += Vector3.up * -Input.GetAxis("Mouse ScrollWheel") * dezoomSpeed;
            
            //Get Forward face
            Vector3 dir = transform.forward;
            //Reset/Ignore y axis
            dir.y = 0;
            dir.Normalize();

            // Move position with arrows around
            transform.position += (transform.right * Input.GetAxis("Horizontal") + dir * Input.GetAxis("Vertical")) * movementSpeed;

            // Rotate camera on Y axis
            if(Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
                transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed, 0));
                _eulerX = transform.rotation.eulerAngles.x;
                _eulerY = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(_eulerX, _eulerY, 0);
            }

            // Rotate camera on X axis
            if(Input.GetMouseButton(1) && Input.GetMouseButton(2)) {
                transform.Rotate(-new Vector3(Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0));
                _eulerX = transform.rotation.eulerAngles.x;
                _eulerY = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(_eulerX, _eulerY, 0);
            }
        }
    }
}