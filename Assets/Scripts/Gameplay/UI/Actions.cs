using System;
using Erutan.Scripts.Gameplay.Entity;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using Protometry;
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
            var p = new Packet {Metadata = new Metadata()};
            var t = new Packet.Types.CreateEntityPacket();
            var c = new Component
            {
                Space = new Component.Types.SpaceComponent()
                {
                    Position =
                        Helper.VectorN(new double[] {hit.transform.position.x, 1, hit.transform.position.z}),
                    Rotation = Helper.QuaternionN(new double[]{ 0, 0, 0, 0}),
                    Scale = Helper.VectorN(new double[] {1, 1, 1})
                }
            };
            t.Components.Add(c);

            p.CreateEntity = t;
            SessionManager.Instance.Client.Send(p);
        }
    }
}