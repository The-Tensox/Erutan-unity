using System.Collections;
using Erutan;
using GamePlay.Object;
using Sessions;
using UnityEngine;
using Utils;
using Component = Erutan.Component;

namespace Gameplay.UI
{
    public class Actions : MonoBehaviour
    {
        [SerializeField] private GameObject fireballPrefab;

        private float _lastFireball;
        
        private bool _isDragging;
        
        private GameObject _draggedObject;


        private void Update() {
            if (_draggedObject != null && _isDragging) {
                DragObject();
            }
            if (Time.time > _lastFireball && Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                _lastFireball = Time.time + 0.2f; // 2 sec cooldown
                ThrowFireBall();
            }

            // if(Input.GetMouseButton(0)) {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //
            //     if(!Physics.Raycast (ray, out var hit))
            //         return;
            //     /*
            //     var id = Convert.ToUInt64(hit.transform.name);
            //     ObjectManager.Instance.Entities.TryGetValue(id, out var obj);
            //     Record.Log($"{obj}");
            //     */
            // }
        }
        
        private void ThrowFireBall()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var camPos = Camera.main.transform.position;
            var go = Pool.Spawn(fireballPrefab, Vector3.Lerp(camPos, ray.direction, 0.1f),
                Quaternion.LookRotation(ray.direction));
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward*1000f);
            // go.transform.localScale *= 10;
        }
        

        private void DragObject()
        {
            var v3 = Input.mousePosition;
            v3.z = 100.0f;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            _draggedObject.transform.position = v3;
        }

        public void StartDraggingCreateObject() {
            _isDragging = true;
            _draggedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _draggedObject.transform.localScale = new UnityEngine.Vector3(5, 5, 5);
            _draggedObject.GetComponent<Renderer>().material.color = Color.red;
        }
        
        public void StopDraggingCreateObject() {
            Destroy(_draggedObject);
            _isDragging = false;
            //Record.Log($"StopDraggingCreateObject {Input.mousePosition}");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000.0f))
                return;
            var hitPos = hit.transform.position;
            ObjectManager.Instance.CreateObject(new Protometry.Vector3
            {
                X = hitPos.x,
                Y = 1,
                Z = hitPos.z
            });
        }

        public void Armageddon()
        {
            var p = new Packet {Armageddon = new Packet.Types.ArmageddonPacket()};
            SessionManager.Instance.Client.Send(p);
        }


    }
}