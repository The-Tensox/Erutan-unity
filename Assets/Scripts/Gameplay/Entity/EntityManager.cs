using System.Collections.Generic;
using Erutan.Scripts.Protos;
using Erutan.Scripts.Utils;
using UnityEngine;
using static Erutan.Scripts.Protos.Packet.Types;

namespace Erutan.Scripts.Gameplay.Nature
{
    public class EntityManager : Singleton<EntityManager>
    {
        [SerializeField]
        private GameObject CubePrefab;
        private Dictionary<ulong, Entity> _entities;

        public Dictionary<ulong, Entity> Entities { get => _entities; set => _entities = value; }



        #region MONO

        private void Start()
        {
            Entities = new Dictionary<ulong, Entity>();
            Pool.Preload(CubePrefab, 20);
            GameplayManager.Instance.OnEntityCreated += CreateEntity;
            GameplayManager.Instance.OnEntityMoved += MoveEntity;
            GameplayManager.Instance.OnEntityRotated += RotateEntity;
            GameplayManager.Instance.OnEntityDestroyed += DestroyEntity;

            GameplayManager.Instance.OnAnimalUpdated += AnimalUpdated;
        }

        protected override void OnDestroy()
        {
            GameplayManager.Instance.OnEntityCreated -= CreateEntity;
            GameplayManager.Instance.OnEntityMoved -= MoveEntity;
            GameplayManager.Instance.OnEntityRotated -= RotateEntity;
            GameplayManager.Instance.OnEntityDestroyed -= DestroyEntity;

            GameplayManager.Instance.OnAnimalUpdated -= AnimalUpdated;
        }

        #endregion

        #region PUBLIC METHODS

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Spawns new unit and adds it to dictionary
        /// </summary>
        /// <param name="packet"></param>
        private void CreateEntity(CreateEntityPacket packet)
        {
            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            Vector3 scale = Vector3.zero;
            Color color = Color.white;
            Record.Log($"Packet {packet.Components}");
            foreach(var c in packet.Components) {
                switch (c.TypeCase) {
                    case Protos.Component.TypeOneofCase.Space:
                        position = new Vector3((float)c.Space.Position.X, (float)c.Space.Position.Y, (float)c.Space.Position.Z);
                        rotation = new Quaternion((float)c.Space.Rotation.X, 
                                                  (float)c.Space.Rotation.Y, 
                                                  (float)c.Space.Rotation.Z, 
                                                  (float)c.Space.Rotation.W);
                        scale = new Vector3((float)c.Space.Scale.X, (float)c.Space.Scale.Y, (float)c.Space.Scale.Z);
                        break;
                    case Protos.Component.TypeOneofCase.Render:
                        color = new Color(c.Render.Red, c.Render.Green, c.Render.Blue);
                        break;
                }
            }

            var entity = Pool.Spawn(CubePrefab, position, rotation).GetComponent<Entity>();
            entity.GetComponent<Renderer>().material.color = color;

            entity.transform.localScale = scale;
            entity.Id = packet.EntityId;
            entity.gameObject.name = $"{entity.Id}";
            Entities[entity.Id] = entity;
            Record.Log($"New object {entity}");
            
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void MoveEntity(UpdatePositionPacket packet)
        {
            var entity = Entities[packet.EntityId];

            // Maybe we received a move packet but the entity died ?
            if (entity.gameObject.activeInHierarchy) entity.Move(packet.Position);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void RotateEntity(UpdateRotationPacket packet)
        {
            var entity = Entities[packet.EntityId];
            if (entity.gameObject.activeInHierarchy) entity.Rotate(packet.Rotation);
        }

        private void DestroyEntity(DestroyEntityPacket packet)
        {
            var entity = Entities[packet.EntityId];
            Pool.Despawn(entity.gameObject);
        }

        private void AnimalUpdated(UpdateAnimalPacket packet)
        {
            var eater = Entities[packet.EntityId];
            var renderer = eater.GetComponent<Renderer>();
            var color = renderer.material.color;
            color.r = (float)(0.4f + packet.Life / 100f);
            renderer.material.color = color;
        }

        #endregion
    }

}