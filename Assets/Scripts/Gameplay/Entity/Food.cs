using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Nature
{
    public class Food : Entity
    {
        /*
        [HideInInspector] public float Radius;
        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Animal")) {
                var p = Random.insideUnitCircle * Radius;
                transform.SetPositionAndRotation(new Vector3(p.x, transform.position.y, p.y), Quaternion.identity);
                collision.gameObject.GetComponent<Animal>().Life += 20f;
            }
        }
        */
    }
}