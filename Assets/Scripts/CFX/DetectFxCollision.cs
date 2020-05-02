using System;
using Erutan;
using GamePlay.Object;
using Sessions;
using UnityEngine;
using Utils;

namespace CFX
{
    public class DetectFxCollision : MonoBehaviour
    {
        [SerializeField]
        private GameObject onDestroyEffect;
        private void Start()
        {
            Pool.Despawn(gameObject, 5000);
        }

        private void OnParticleCollision(GameObject other)
        {
            var id = Convert.ToUInt64(other.transform.name);
            var otherPosition = other.transform.position;
            var region = new Protometry.Box()
            {
                Min = (otherPosition - other.transform.localScale).ToVector3(),
                Max = (otherPosition + other.transform.localScale).ToVector3()
            };
            // Record.Log($"{otherPosition} - {region}");
            var p = new Packet {DestroyObject = new Packet.Types.DestroyObjectPacket { ObjectId = id, Region = region}};
            SessionManager.Instance.Client.Send(p);
            Pool.Despawn(gameObject);
        }

        private void OnDisable()
        {
            Pool.Despawn(Pool.Spawn(onDestroyEffect, gameObject.transform.position, gameObject.transform.rotation), 2000);
        }
    }
}
