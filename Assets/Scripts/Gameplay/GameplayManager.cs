using System;
using Erutan;
using Sessions;
using Utils;
using static Erutan.Packet.Types;

namespace Gameplay
{
    public class GameplayManager : Singleton<GameplayManager>
    {    

        #region PUBLIC EVENTS

        // Game
        public event Action OnGameStarted;

        // Physical world 
        public event Func<UpdateObjectPacket, UnityEngine.GameObject> ObjectUpdated;
        public event Action<UpdatePositionPacket> ObjectMoved;
        public event Action<UpdateRotationPacket> ObjectRotated;
        public event Action<DestroyObjectPacket> ObjectDestroyed;
        
        // Player
        public event Action<CreatePlayerPacket> PlayerCreated;


        #endregion

        // Start is called before the first frame update
        private void Start()
        {
            SessionManager.Instance.Client.ReceivedPacket += OnReceivedPacket;
        }

        protected override void OnDestroy()
        {
            SessionManager.Instance.Client.ReceivedPacket -= OnReceivedPacket;
        }

        private void OnReceivedPacket(Packet packet) {
            //Record.Log($"Receiving packet: {packet.TypeCase}");
            switch (packet.TypeCase) {
                case Packet.TypeOneofCase.UpdateObject:
                    ObjectUpdated?.Invoke(packet.UpdateObject);
                    break;
                case Packet.TypeOneofCase.UpdatePosition:
                    ObjectMoved?.Invoke(packet.UpdatePosition);
                    break;
                case Packet.TypeOneofCase.UpdateRotation:
                    ObjectRotated?.Invoke(packet.UpdateRotation);
                    break;
                case Packet.TypeOneofCase.DestroyObject:
                    ObjectDestroyed?.Invoke(packet.DestroyObject);
                    break;
                case Packet.TypeOneofCase.CreatePlayer:
                    PlayerCreated?.Invoke(packet.CreatePlayer);
                    break;
                default:
                    // TODO: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions
                    Record.Log($"Unimplemented packet handler ! {packet.TypeCase}", LogLevel.Error);
                    break;
            }
        }
    }
}