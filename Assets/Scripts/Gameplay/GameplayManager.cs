using System;
using Erutan.Scripts.Protos;
using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using static Erutan.Scripts.Protos.Packet.Types;

namespace Erutan.Scripts.Gameplay
{
    public class GameplayManager : Singleton<GameplayManager>
    {    

        #region PUBLIC EVENTS

        // Game
        public event Action OnGameStarted;

        // Physical world 
        public event Action<CreateEntityPacket> OnEntityCreated;
        public event Action<UpdateEntityPacket> OnEntityUpdated;
        public event Action<UpdatePositionPacket> OnEntityMoved;
        public event Action<UpdateRotationPacket> OnEntityRotated;
        public event Action<DestroyEntityPacket> OnEntityDestroyed;

        // Animals & Environment
        public event Action<UpdateAnimalPacket> OnAnimalUpdated;


        #endregion

        // Start is called before the first frame update
        void Start()
        {
            SessionManager.Instance.Client.ReceivedPacket += Handler;
        }

        protected override void OnDestroy()
        {
            SessionManager.Instance.Client.ReceivedPacket -= Handler;
        }

        private void Handler(Packet packet) {
            //Record.Log($"Receiving packet: {packet.TypeCase}");
            switch (packet.TypeCase) {
                case Packet.TypeOneofCase.CreateEntity:
                    OnEntityCreated?.Invoke(packet.CreateEntity);
                    break;
                case Packet.TypeOneofCase.UpdateEntity:
                    OnEntityUpdated?.Invoke(packet.UpdateEntity);
                    break;
                case Packet.TypeOneofCase.UpdatePosition:
                    OnEntityMoved?.Invoke(packet.UpdatePosition);
                    break;
                case Packet.TypeOneofCase.UpdateRotation:
                    OnEntityRotated?.Invoke(packet.UpdateRotation);
                    break;
                case Packet.TypeOneofCase.DestroyEntity:
                    OnEntityDestroyed?.Invoke(packet.DestroyEntity);
                    break;
                case Packet.TypeOneofCase.UpdateAnimal:
                    OnAnimalUpdated?.Invoke(packet.UpdateAnimal);
                    break;
                default:
                    // TODO: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions
                    Record.Log($"Unimplemented packet handler ! {packet.TypeCase}", LogLevel.Error);
                    break;
            }
        }
    }
}