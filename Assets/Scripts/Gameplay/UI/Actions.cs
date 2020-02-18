using System;
using Erutan.Scripts.Gameplay.Entity;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.UI
{
    public class Actions : MonoBehaviour
    {
        private bool _isDragging;
        
        private GameObject _draggedObject;


        private void Update() {
            if (_draggedObject != null && _isDragging) {
                var v3 = Input.mousePosition;
                v3.z = 100.0f;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                _draggedObject.transform.position = v3;
            }
            if(Input.GetMouseButton(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(!Physics.Raycast (ray, out var hit))
                    return;
                /*
                var id = Convert.ToUInt64(hit.transform.name);
                EntityManager.Instance.Entities.TryGetValue(id, out var entity);
                Record.Log($"{entity}");
                */
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
            //Record.Log($"StopDraggingCreateObject {Input.mousePosition}");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000.0f))
                return;
            var p = new Protos.Packet();
            p.Metadata = new Protos.Metadata();
            var t = new Protos.Packet.Types.CreateEntityPacket();
            var c = new Protos.Component();
            c.Space = new Protos.Component.Types.SpaceComponent(){
                    Position = new Protos.NetVector3(){X = hit.transform.position.x, Y = 1, Z = hit.transform.position.z},
                    Rotation = new Protos.NetQuaternion(){X = 0, Y = 0, Z = 0, W = 0},
                    Scale = new Protos.NetVector3(){X = 1, Y = 1, Z = 1}
                };
            t.Components.Add(c);

            p.CreateEntity = t;
            SessionManager.Instance.Client.Send(p);
        }
    }
}