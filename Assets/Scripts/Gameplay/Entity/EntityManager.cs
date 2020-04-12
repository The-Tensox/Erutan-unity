using System.Collections.Generic;
using Erutan;
using Erutan.Scripts.Utils;
using UnityEngine;
using System.Linq;
using static Erutan.Packet.Types;

namespace Erutan.Scripts.Gameplay.Entity
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
            GameplayManager.Instance.OnEntityUpdated += UpdateEntity;
            GameplayManager.Instance.OnEntityMoved += MoveEntity;
            GameplayManager.Instance.OnEntityRotated += RotateEntity;
            GameplayManager.Instance.OnEntityDestroyed += DestroyEntity;
            GameplayManager.Instance.OnAnimalUpdated += AnimalUpdated;
        }

        protected override void OnDestroy()
        {
            GameplayManager.Instance.OnEntityCreated -= CreateEntity;
            GameplayManager.Instance.OnEntityUpdated -= UpdateEntity;
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
            Shape shape = null;
            //Record.Log($"Packet {packet.Components}");
            foreach(var c in packet.Components) {
                switch (c.TypeCase) {
                    case Component.TypeOneofCase.Space:
                        position = c.Space.Position.ToVector3();
                        rotation = c.Space.Rotation.ToQuaternion();
                        scale = c.Space.Scale.ToVector3();
                        shape = c.Space.Shape;
                        break;
                    case Component.TypeOneofCase.Render:
                        color = new Color(c.Render.Red, c.Render.Green, c.Render.Blue);
                        break;
                }
            }

            // TODO: could handle the case "null shape"
            var entity = InstanciateMesh(shape, color).AddComponent<Entity>();

            //var entity = Pool.Spawn(CubePrefab, position, rotation).GetComponent<Entity>();
            //entity.GetComponent<Renderer>().material.color = color;

            entity.transform.localScale = scale;
            entity.Id = packet.EntityId;
            entity.gameObject.name = $"{entity.Id}";
            entity.Components = packet.Components;
            Entities[entity.Id] = entity;
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="packet"></param>
        private void UpdateEntity(UpdateEntityPacket packet)
        {
            var entity = Entities[packet.EntityId];
            entity.Components = packet.Components;
            foreach(var c in packet.Components) {
                switch (c.TypeCase) {
                    case Component.TypeOneofCase.Space:
                        var position = c.Space.Position.ToVector3();
                        var rotation = c.Space.Rotation.ToQuaternion();
                        var scale = c.Space.Scale.ToVector3();
                        entity.transform.position = position;
                        entity.transform.rotation = rotation;
                        entity.transform.localScale = scale;
                        break;
                    case Component.TypeOneofCase.Render:
                        entity.GetComponent<Renderer>().material.color = new Color(c.Render.Red, c.Render.Green, c.Render.Blue);
                        break;
                    case Component.TypeOneofCase.Health:
                        var renderer = entity.GetComponent<Renderer>();
                        var color = renderer.material.color;
                        color.r = (float)(Mathf.Clamp(4.0f*(float)(c.Health.Life) / 100f, 0.2f, 0.8f));
                        renderer.material.color = color;
                        break;
                }
            }
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
            Entities.Remove(packet.EntityId);
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

        private GameObject InstanciateMesh(Shape shape, Color color) {
            var go = new GameObject();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            meshRenderer.sharedMaterial.color = color;
            var meshFilter = go.AddComponent<MeshFilter>();
            var mesh = new Mesh();

            mesh.vertices = shape.Vertices.ToList().Select(e => e.ToVector3()).ToArray();
            mesh.triangles = shape.Tris.ToArray();

            if (shape.Normals != null) {
                mesh.normals = shape.Normals.ToList().Select(e => e.ToVector3()).ToArray();
                mesh.uv = shape.Uvs.ToList().Select(e => e.ToVector2()).ToArray();
            } else {
                mesh.Optimize ();
		        mesh.RecalculateNormals ();
            }
            
            meshFilter.mesh = mesh;
            return go;
        }
    }

}