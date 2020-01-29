using System.Collections.Generic;
using Erutan.Scripts.Utils;
using UnityEngine;
using static Erutan.Scripts.Protos.Packet.Types;

namespace Erutan.Scripts.Gameplay.Nature
{
    public class ObjectManager : Singleton<ObjectManager>
    {
        [SerializeField]
        private GameObject AIPrefab;
        [SerializeField]
        private GameObject FoodPrefab;
        [SerializeField]
        private GameObject GroundPrefab;
        private List<NatureObject> _natureObjects;


        #region MONO

        private void Start()
        {
            _natureObjects = new List<NatureObject>();
            Pool.Preload(AIPrefab, 20);
            Pool.Preload(FoodPrefab, 20);
            GameplayManager.Instance.OnObjectCreated += CreateObject;
            GameplayManager.Instance.OnObjectMoved += MoveObject;
            GameplayManager.Instance.OnObjectRotated += RotateObject;
            GameplayManager.Instance.OnObjectDestroyed += DestroyObject;
        }

        protected override void OnDestroy()
        {
            GameplayManager.Instance.OnObjectCreated -= CreateObject;
            GameplayManager.Instance.OnObjectMoved -= MoveObject;
            GameplayManager.Instance.OnObjectRotated -= RotateObject;
            GameplayManager.Instance.OnObjectDestroyed -= DestroyObject;
        }

        #endregion

        #region PUBLIC METHODS

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Spawns new unit and adds it to dictionary
        /// </summary>
        /// <param name="packet"></param>
        private void CreateObject(CreateObjectPacket packet)
        {
            var position = packet.Object.Position;
            var rotation = packet.Object.Rotation;
            var scale = packet.Object.Scale;
            NatureObject natureObject = null;
            GameObject prefab = null;
            switch (packet.Object.Type) {
                case Protos.NetObject.Types.Type.Animal:
                    prefab = AIPrefab;
                    break;
                case Protos.NetObject.Types.Type.Food:
                    prefab = FoodPrefab;
                    break;
                case Protos.NetObject.Types.Type.Ground:
                    prefab = GroundPrefab;
                    break;
                default:
                    Record.Log($"Unknown object type {packet.Object.Type}", LogLevel.Error);
                    break;
            }
            natureObject = Pool.Spawn(prefab, new Vector3((float)position.X, (float)position.Y, (float)position.Z),
                new Quaternion((float)rotation.X, (float)rotation.Y, (float)rotation.Z, (float)rotation.W)).GetComponent<NatureObject>();
            natureObject.transform.localScale = new Vector3((float)scale.X, (float)scale.Y, (float)scale.Z);
            natureObject.OwnerId = packet.Object.OwnerId;
            natureObject.Id = packet.Object.ObjectId;
            _natureObjects.Add(natureObject);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void MoveObject(UpdatePositionPacket packet)
        {
            var natureObject = _natureObjects.Find(x => x.Id == packet.ObjectId);

            // Maybe we received a move packet but the natureObject died ?
            if (natureObject.gameObject.activeInHierarchy) natureObject.Move(packet.Position);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void RotateObject(UpdateRotationPacket packet)
        {
            var natureObject = _natureObjects.Find(x => x.Id == packet.ObjectId);
            if (natureObject.gameObject.activeInHierarchy) natureObject.Rotate(packet.Rotation);
        }

        private void DestroyObject(DestroyObjectPacket packet)
        {
            var natureObject = _natureObjects.Find(x => x.Id == packet.ObjectId);
            Pool.Despawn(natureObject.gameObject);
        }

        #endregion
    }

}