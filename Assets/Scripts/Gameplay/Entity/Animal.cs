using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Protos;
using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Nature
{
    public class Animal : Entity
    {

        /*
        [HideInInspector] public Transform Food;
        private float _life;
        public float Life 
        { 
            get 
            {
                return _life;
            }
            set 
            {
                _life = value;
                if (_life < 0) Pool.Despawn(gameObject);
                if (_life > 100) _life = 100;
                //Record.Log($"life {_life}, {_renderer.material.color.r}");
                var c = _renderer.material.color;
                c.r = 0.4f + _life / 100f;
                _renderer.material.color = c;
                if (_life > 80f && (_target != Food || _target.GetComponent<Animal>().Life < 80f)) {
                    var otherAnimals = transform.parent.GetComponentsInChildren<Animal>();
                    if (otherAnimals == null) return;
                    float minDistance = float.MaxValue;
                    Animal closestAnimal = null;
                    foreach(var animal in otherAnimals) {
                        if (animal.gameObject != gameObject && animal.Life > 80f) {
                            var distance = Vector3.Distance(animal.transform.position, transform.position);
                            if (distance < minDistance) {
                                minDistance = distance;
                                closestAnimal = animal;
                            }
                        }
                    }
                    if (closestAnimal != null) _target = closestAnimal.transform;
                }
            }
        }
        private Renderer _renderer;
        private Transform _target;

        void Start() {
            _renderer = GetComponent<Renderer>();
            Life = 20;
        }

        // Update is called once per frame
        void Update()
        {
            if (Food != null) {
                Life += -Time.deltaTime;
                transform.LookAt(Food);
                var myPos = transform.position;
                transform.Translate(new Vector3(0, 0, Time.deltaTime * 30f));
            }
        }
        */
    }
}