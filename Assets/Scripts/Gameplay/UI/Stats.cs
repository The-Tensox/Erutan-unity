using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Gameplay.Entity;
using Erutan.Scripts.Utils;
using UnityEngine;
using static Erutan.Scripts.Protos.Packet.Types;

namespace Erutan.Scripts.Gameplay.UI 
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] private GameObject Speed;
        [SerializeField] private GameObject Life;
        

        private TMPro.TextMeshProUGUI[] _speedStatistics;
        private TMPro.TextMeshProUGUI[] _lifeStatistics;
        private void Start()
        {
            _speedStatistics = Speed.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            _lifeStatistics = Life.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        }

        private void Update()
        {
            if (Time.frameCount % 60 == 0) { // TODO: tweak ...
                double averageSpeed = 0, minimumSpeed = double.MaxValue, maximumSpeed = double.MinValue;
                double averageLife = 0, minimumLife = double.MaxValue, maximumLife = double.MinValue;
                foreach(var entity in EntityManager.Instance.Entities.Values) {
                    foreach(var component in entity.Components) {
                        if (component.Speed != null) {
                            averageSpeed = (averageSpeed + component.Speed.MoveSpeed) / 2;
                            minimumSpeed = component.Speed.MoveSpeed < minimumSpeed ? component.Speed.MoveSpeed : minimumSpeed;
                            maximumSpeed = component.Speed.MoveSpeed > maximumSpeed ? component.Speed.MoveSpeed : maximumSpeed;
                        }

                        if (component.Health != null) {
                            averageLife = (averageLife + component.Health.Life) / 2;
                            minimumLife = component.Health.Life < minimumLife ? component.Health.Life : minimumLife;
                            maximumLife = component.Health.Life > maximumLife ? component.Health.Life : maximumLife;
                        }
                    }
                }
                _speedStatistics[0].text = averageSpeed.ToString("0.##");
                _speedStatistics[1].text = minimumSpeed.ToString("0.##");
                _speedStatistics[2].text = maximumSpeed.ToString("0.##");
                _lifeStatistics[0].text = averageLife.ToString("0.##");
                _lifeStatistics[1].text = minimumLife.ToString("0.##");
                _lifeStatistics[2].text = maximumLife.ToString("0.##");
            }
        }
    }
}