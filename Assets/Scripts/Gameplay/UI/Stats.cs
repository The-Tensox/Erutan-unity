using System.Collections;
using System.Collections.Generic;
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
            GameplayManager.Instance.OnStatistics += UpdateStatistics;
        }

        private void Destroy()
        {
            GameplayManager.Instance.OnStatistics -= UpdateStatistics;
        }

        private void UpdateStatistics(StatisticsPacket packet) 
        {
            _speedStatistics[0].text = packet.Speed.Average.ToString("0.##");
            _speedStatistics[1].text = packet.Speed.Minimum.ToString("0.##");
            _speedStatistics[2].text = packet.Speed.Maximum.ToString("0.##");
            _lifeStatistics[0].text = packet.Life.Average.ToString("0.##");
            _lifeStatistics[1].text = packet.Life.Minimum.ToString("0.##");
            _lifeStatistics[2].text = packet.Life.Maximum.ToString("0.##");
        }
    }
}