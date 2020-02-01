using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Erutan.Scripts.Gameplay.UI
{
    public class Actions : MonoBehaviour
    {
        private bool _isDragging;
        
        private GameObject _draggedObject;


        private void Update() {
            if (_draggedObject != null && _isDragging) {
                Record.Log($"StartDragging {Input.mousePosition}");
                var v3 = Input.mousePosition;
                v3.z = 100.0f;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                _draggedObject.transform.position = v3;
            }
        }

        public void StartDraggingCreateObject() {
            _isDragging = true;
            _draggedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _draggedObject.transform.localScale = new Vector3(5, 5, 5);
            _draggedObject.GetComponent<Renderer>().material.color = Color.red;
        }
        
        public void StopDraggingCreateObject() {
            Destroy(_draggedObject);
            _isDragging = false;
            Record.Log($"StopDraggingCreateObject {Input.mousePosition}");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000.0f))
                return;
            var p = new Protos.Packet();
            p.Metadata = new Protos.Metadata();
            var t = new Protos.Packet.Types.CreateObjectPacket();
            //Record.Log($"hit {hit.point}");
            //Debug.DrawRay(ray.origin, hit.transform.position, Color.green, 10);

            t.Object = new Protos.NetObject(){
                ObjectId = gameObject.GetInstanceID().ToString(),
                OwnerId = gameObject.GetInstanceID().ToString(),// TODO: server token//SessionManager.Instance.Client.
                Position = new Protos.NetVector3(){X = hit.transform.position.x, Y = 1, Z = hit.transform.position.z},
                Rotation = new Protos.NetQuaternion(){X = 0, Y = 0, Z = 0, W = 0},
                Scale = new Protos.NetVector3(){X = 1, Y = 1, Z = 1},
                Type = Protos.NetObject.Types.Type.Animal
            };
            p.CreateObject = t;
            SessionManager.Instance.Client.Send(p);
        }
    }
}