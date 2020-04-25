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
        public Dictionary<ulong, Entity> Entities { get; private set; }

        #region MONO

        private void Start()
        {
            Entities = new Dictionary<ulong, Entity>();
            GameplayManager.Instance.OnEntityUpdated += UpdateEntity;
            GameplayManager.Instance.OnEntityMoved += MoveEntity;
            GameplayManager.Instance.OnEntityRotated += RotateEntity;
            GameplayManager.Instance.OnEntityDestroyed += DestroyEntity;
        }

        protected override void OnDestroy()
        {
            GameplayManager.Instance.OnEntityUpdated -= UpdateEntity;
            GameplayManager.Instance.OnEntityMoved -= MoveEntity;
            GameplayManager.Instance.OnEntityRotated -= RotateEntity;
            GameplayManager.Instance.OnEntityDestroyed -= DestroyEntity;
        }

        #endregion

        #region PUBLIC METHODS

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Update entity (~=POST + PUT)
        /// </summary>
        /// <param name="packet"></param>
        private void UpdateEntity(UpdateEntityPacket packet)
        {
            Entity entity = null;
            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            var scale = Vector3.zero;
            var colors = new[] {Color.white};
            Protometry.Mesh mesh = null;
            
            // Update entity case
            if (Entities.ContainsKey(packet.EntityId))
            {
                entity = Entities[packet.EntityId];
            }

            foreach(var c in packet.Components) {
                switch (c.TypeCase) {
                    case Component.TypeOneofCase.Space:
                        position = c.Space.Position.ToVector3();
                        rotation = c.Space.Rotation.ToQuaternion();
                        scale = c.Space.Scale.ToVector3();
                        break;
                    case Component.TypeOneofCase.Render:
                        colors = c.Render.Colors.Select(col => col.ToColor()).ToArray();
                        mesh = c.Render.Mesh;
                        break;
                    case Component.TypeOneofCase.Health:
                        if (entity)
                        {
                            var rend = entity.GetComponent<Mesh>();
                            if (rend)
                            {
                                for (var i = 0; i < rend.colors.Length; i++)
                                {
                                    rend.colors[i].r = Mathf.Clamp(4.0f * (float) c.Health.Life / 100f, 0.2f, 0.8f);
                                }
                            }
                        }
                        break;
                }
            }

            // Create entity case
            if (!entity)
            {
                entity = InstanciateMesh(mesh, colors).AddComponent<Entity>();
                entity.Id = packet.EntityId;
                entity.gameObject.name = $"{entity.Id}";
                Entities[entity.Id] = entity;
            }
            //if (color.a < 1) {
            //    Record.Log($"packet {color} pos {position}");
            //}
            var transform1 = entity.transform;
            transform1.position = position;
            transform1.rotation = rotation;
            transform1.localScale = scale;
            entity.Components = packet.Components;
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
            if (Entities.ContainsKey(packet.EntityId)) {
                var entity = Entities[packet.EntityId];
                Destroy(entity.gameObject);
                Entities.Remove(packet.EntityId);
            } else {
                Record.Log($"Tried to destroy in-existent entity");
            }
        }
        

        #endregion

        private GameObject InstanciateMesh(Protometry.Mesh receivedMesh, Color[] colors) {
            var go = new GameObject();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            
            // Override standard shader to allow transparency
            var m = new Material(Shader.Find("Standard")) {color = colors[0]};
            m.SetOverrideTag("RenderType", "Transparent");
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            m.SetFloat("_Mode", 2.0f);
            meshRenderer.sharedMaterial = m;
            
            var meshFilter = go.AddComponent<MeshFilter>();
            // TODO: helper erutan.mesh -> unityengine.mesh
            var mesh = new UnityEngine.Mesh
            {
                vertices = receivedMesh.Vertices.ToList().Select(e => e.ToVector3()).ToArray(),
                triangles = receivedMesh.Tris.ToArray()
            };
            
            if (receivedMesh.Normals != null) {
                mesh.normals = receivedMesh.Normals.ToList().Select(e => e.ToVector3()).ToArray();
                foreach (var u in receivedMesh.Uvs)
                {
                    mesh.uv.Append(new Vector2((float) u.X, (float) u.Y));
                }
            } else {
                mesh.Optimize ();
		        mesh.RecalculateNormals ();
            }
            meshFilter.mesh = mesh;
            mesh.colors = colors;


            return go;
        }
    }

}
