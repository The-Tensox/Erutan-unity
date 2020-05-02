using System.Collections.Generic;
using System.Data;
using System.Linq;
using Erutan;
using Gameplay;
using Gameplay.Player;
using Sessions;
using UnityEngine;
using Utils;
using static Erutan.Packet.Types;
using Component = Erutan.Component;

namespace GamePlay.Object
{
    public class ObjectManager : Singleton<ObjectManager>
    {
        public Dictionary<ulong, Object> Objects { get; private set; }
        private GameObject _preloaded;

        #region MONO

        private void Start()
        {
            Objects = new Dictionary<ulong, Object>();
            GameplayManager.Instance.ObjectUpdated += OnObjectUpdated;
            GameplayManager.Instance.ObjectMoved += OnObjectMoved;
            GameplayManager.Instance.ObjectRotated += OnObjectRotated;
            GameplayManager.Instance.ObjectDestroyed += OnObjectDestroyed;
            GameplayManager.Instance.PlayerCreated += OnPlayerCreated;
        }

        private void OnPlayerCreated(CreatePlayerPacket p)
        {
            var uep = new UpdateObjectPacket {ObjectId = p.ObjectId};
            uep.Components.Add(p.Components);
            var go = OnObjectUpdated(uep);
            var isSelf = false;
            foreach (var c in p.Components)
            {
                switch (c.TypeCase)
                {
                    case Component.TypeOneofCase.NetworkBehaviour:
                        isSelf = c.NetworkBehaviour.OwnerToken == SessionManager.Instance.Client.Token;
                        break;
                }
            }
            // Only add camera, etc if it's self client, we don't want to have other clients view !
            if (isSelf)
            {
                go.AddComponent<Camera>();
                go.AddComponent<CameraController>();
                go.AddComponent<AudioListener>(); // No audio anyway ..
                go.AddComponent<TransformNetwork>().id = p.ObjectId;
                var cullingSize = 50; // TODO: config or something
                go.AddComponent<Cull>().size = new Vector3(cullingSize, cullingSize, cullingSize);
                go.tag = "MainCamera";
            }
        }

        protected override void OnDestroy()
        {
            GameplayManager.Instance.ObjectUpdated -= OnObjectUpdated;
            GameplayManager.Instance.ObjectMoved -= OnObjectMoved;
            GameplayManager.Instance.ObjectRotated -= OnObjectRotated;
            GameplayManager.Instance.ObjectDestroyed -= OnObjectDestroyed;
        }

        #endregion

        #region PUBLIC METHODS
        
        public void UpdateObject(ulong objectId, Protometry.Vector3 actualPosition,
            Protometry.Vector3 newPosition)
        {
            UpdateObject(objectId, actualPosition, new Protometry.Quaternion
            {
                X = 0,
                Y = 0,
                Z = 0,
                W = 0
            },
            newPosition, new Protometry.Quaternion
            {
                X = 0,
                Y = 0,
                Z = 0,
                W = 0
            });
        }

        /// <summary>
        /// UpdateObject is a client to server method to update an object
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="actualPosition"></param>
        /// <param name="actualRotation"></param>
        /// <param name="newPosition"></param>
        /// <param name="newRotation"></param>
        public void UpdateObject(ulong objectId, 
            Protometry.Vector3 actualPosition, Protometry.Quaternion actualRotation,
            Protometry.Vector3 newPosition, Protometry.Quaternion newRotation)
        {
            var p = new Packet {};
            // Update
            if (Objects.ContainsKey(objectId))
            {
                var obj = Objects[objectId];
                var trans = obj.transform;
                var packetType = new UpdateSpaceRequestPacket {ObjectId = objectId};
                foreach (var c in obj.Components)
                {
                    switch (c.TypeCase)
                    {
                        case Component.TypeOneofCase.Space:
                            packetType.ActualSpace = c.Space;
                            break;
                    }
                }
                
                // The requested new space will correspond to local client object's transform
                var newSpace = new Component.Types.SpaceComponent
                {
                    Position = newPosition,
                    Rotation = newRotation,
                    Scale = trans.localScale.ToVector3()
                };
                
                packetType.ActualSpace.Position = actualPosition;
                packetType.ActualSpace.Rotation = actualRotation;
                packetType.NewSpace = newSpace;
                p.UpdateSpaceRequest = packetType;
                SessionManager.Instance.Client.Send(p);
            }
            else
            {
                // No id, create
                var packetType = new UpdateObjectPacket();

                // Create components from scratch
                packetType.Components.Add(new Component
                {
                    Space = new Component.Types.SpaceComponent
                    {
                        Position = actualPosition,
                        Rotation = actualRotation,
                        Scale = new Protometry.Vector3 {X = 1, Y = 1, Z = 1}
                    }
                });
                packetType.Components.Add(new Component
                {
                    BehaviourType = new Component.Types.BehaviourTypeComponent
                    {
                        // TODO: hardcoded animal tag because we can only create animals yet
                        Tag = Component.Types.BehaviourTypeComponent.Types.Tag.Animal
                    }
                });
                p.UpdateObject = packetType;
                SessionManager.Instance.Client.Send(p);
            }
        }

        public void CreateObject(Protometry.Vector3 position, Protometry.Quaternion rotation)
        {
            UpdateObject(ulong.MaxValue, position, rotation, position, rotation);
        }
        
        public void CreateObject(Protometry.Vector3 position)
        {
            UpdateObject(ulong.MaxValue, position, position);
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Update object (~=POST + PUT)
        /// </summary>
        /// <param name="packet"></param>
        private GameObject OnObjectUpdated(UpdateObjectPacket packet)
        {
            Object obj = null;
            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            var scale = Vector3.zero;
            var colors = new[] {Color.white};
            Protometry.Mesh mesh = null;
            
            // Update object case
            if (Objects.ContainsKey(packet.ObjectId))
            {
                obj = Objects[packet.ObjectId];
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
                        if (obj)
                        {
                            var rend = obj.GetComponent<Mesh>();
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

            // Create object case
            if (!obj)
            {
                Record.Log($"Creating object");
                // GameObject go;
                // if (_preloaded != null) {
                //     go = Pool.Spawn(_preloaded);
                //     go.GetComponent<MeshRenderer>().sharedMaterial.color = colors[0];
                //     go.GetComponent<MeshFilter>().mesh.colors = colors;
                //     _preloaded = go;
                // } else {
                //     go = InstanciateMesh(mesh, colors);
                //     Pool.Preload(go, 10);
                // }
                //     
                // obj = go.AddComponent<Object>();
                obj = InstanciateMesh(mesh, colors).AddComponent<Object>();

                obj.Id = packet.ObjectId;
                obj.gameObject.name = $"{obj.Id}";
                Objects[obj.Id] = obj;
            }
            //if (color.a < 1) {
            //    Record.Log($"packet {color} pos {position}");
            //}
            var transform1 = obj.transform;
            transform1.position = position;
            transform1.rotation = rotation;
            transform1.localScale = scale;
            obj.Components = packet.Components;
            return obj.gameObject;
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void OnObjectMoved(UpdatePositionPacket packet)
        {
            var obj = Objects[packet.ObjectId];

            // Maybe we received a move packet but the object died ?
            if (obj.gameObject.activeInHierarchy) obj.Move(packet.Position);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        private void OnObjectRotated(UpdateRotationPacket packet)
        {
            var obj = Objects[packet.ObjectId];
            if (obj.gameObject.activeInHierarchy) obj.Rotate(packet.Rotation);
        }

        private void OnObjectDestroyed(DestroyObjectPacket packet)
        {
            if (Objects.ContainsKey(packet.ObjectId)) {
                var obj = Objects[packet.ObjectId];
                Destroy(obj.gameObject);
                // Pool.Despawn(obj.gameObject);
                Objects.Remove(packet.ObjectId);
            } else {
                Record.Log($"Tried to destroy in-existent object");
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
            go.AddComponent<BoxCollider>();

            return go;
        }
    }

}
