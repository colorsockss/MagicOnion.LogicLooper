using System.Threading.Tasks;
using MagicOnion;
using MemoryPack;
using UnityEngine;

namespace MagicOnionShared
{

    // Server -> Client definition
    public interface IGamingHubReceiver
    {
        // The method must have a return type of `void` and can have up to 15 parameters of any type.
        void OnJoin(Player? player);
        void OnLeave(Player? player);
        void OnMove(Player? player);
    }

    // Client -> Server definition
    // implements `IStreamingHub<TSelf, TReceiver>`  and share this type between server and client.
    public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
    {
        // The method must return `ValueTask`, `ValueTask<T>`, `Task` or `Task<T>` and can have up to 15 parameters of any type.
        ValueTask<Player[]> JoinAsync(string roomName, int uuid, Vector3 position, Quaternion rotation);
        ValueTask LeaveAsync();
        ValueTask MoveAsync(Vector3 position, Quaternion rotation);
    }
    
    [MemoryPackable]
    public partial class Player
    {
        public int UUID { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}