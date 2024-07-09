using MagicOnion.Server.Hubs;
using MagicOnionShared;
using UnityEngine;

namespace MagicOnionServer;

public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
{
    // this class is instantiated per connected so fields are cache area of connection.
    private IGroup? _room;
    private Player? _self;
    private IInMemoryStorage<Player>? _storage;

    public async ValueTask<Player[]> JoinAsync(string roomName, int uuid, Vector3 position, Quaternion rotation)
    {
        _self = new Player { UUID = uuid, Position = position, Rotation = rotation };

        // Group can bundle many connections and it has inmemory-storage so add any type per group.
        (_room, _storage) = await Group.AddAsync(roomName, _self);

        // Typed Server->Client broadcast.
        Broadcast(_room).OnJoin(_self);

        return _storage.AllValues.ToArray();
    }

    public async ValueTask LeaveAsync()
    {
        if (_room != null)
        {
            await _room.RemoveAsync(this.Context);
            Broadcast(_room).OnLeave(_self);
        }
    }

    public async ValueTask MoveAsync(Vector3 position, Quaternion rotation)
    {
        await ValueTask.CompletedTask;
        if (_self != null)
        {
            _self.Position = position;
            _self.Rotation = rotation;
            if (_room != null)
                Broadcast(_room).OnMove(_self);
        }
    }

    // You can hook OnConnecting/OnDisconnected by override.
    protected override ValueTask OnDisconnected()
    {
        // on disconnecting, if automatically removed this connection from group.
        return ValueTask.CompletedTask;
    }
}