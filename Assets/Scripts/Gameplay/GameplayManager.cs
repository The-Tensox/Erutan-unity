using System;
using System.Collections;
using System.Collections.Generic;
using Erutan.Scripts.Gameplay.Nature;
using Erutan.Scripts.Protos;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using UnityEngine;
using static Erutan.Scripts.Protos.Packet.Types;

public class GameplayManager : Singleton<GameplayManager>
{    

    #region PUBLIC EVENTS

    //GAME
    public event Action OnGameStarted;

    //AI
    public event Action<CreateObjectPacket> OnObjectCreated;
    public event Action<UpdatePositionPacket> OnObjectMoved;
    public event Action<UpdateRotationPacket> OnObjectRotated;
    public event Action<DestroyObjectPacket> OnObjectDestroyed;

    #endregion
    /*
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject animalPrefab;
    [SerializeField] private GameObject foodPrefab;
    private float _groundSize;
    private Transform _food;
    */
    // Start is called before the first frame update
    void Start()
    {
        SessionManager.Instance.Client.ReceivedPacket += Handler;
        /*
        Pool.Preload(animalPrefab, 10);
        _groundSize = Pool.Spawn(groundPrefab).GetComponent<Collider>().bounds.size.x;
        var f = Pool.Spawn(foodPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        _food = f.transform;
        f.GetComponent<Food>().Radius = _groundSize / 3;
        StartCoroutine(SpawnAnimals());
        */
    }
    /*
    IEnumerator SpawnAnimals() {
        while (true) {
            var p = Random.insideUnitCircle * _groundSize / 4;
            var animal = Pool.Spawn(animalPrefab, new Vector3(p.x, 1, p.y), Quaternion.identity);
            animal.GetComponent<Animal>().Food = _food;
            yield return new WaitForSeconds(4.0f);
        }
    }
    */
    protected override void OnDestroy()
    {
        SessionManager.Instance.Client.ReceivedPacket -= Handler;
    }

    private void Handler(Packet packet) {
        //Record.Log($"Receiving packet: {packet}");
        switch (packet.TypeCase) {
            case Packet.TypeOneofCase.CreateObject:
                OnObjectCreated?.Invoke(packet.CreateObject);
                break;
            case Packet.TypeOneofCase.UpdatePosition:
                OnObjectMoved?.Invoke(packet.UpdatePosition);
                break;
            case Packet.TypeOneofCase.UpdateRotation:
                OnObjectRotated?.Invoke(packet.UpdateRotation);
                break;
            case Packet.TypeOneofCase.DestroyObject:
                OnObjectDestroyed?.Invoke(packet.DestroyObject);
                break;
            default:
                // TODO: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions
                Record.Log($"Unimplemented packet handler ! {packet.TypeCase}", LogLevel.Error);
                break;
        }
    }
}
