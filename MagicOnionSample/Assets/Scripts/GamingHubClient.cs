using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnionShared;
using UnityEngine;

public class GamingHubClient : IGamingHubReceiver
{
    private readonly Dictionary<int, GameObject> _players = new();
    private IGamingHub? _client;
    private int _uuid;

    public async ValueTask<GameObject> ConnectAsync(ChannelBase grpcChannel, string roomName, int uuid)
    {
        _uuid = uuid;
        _client = await StreamingHubClient.ConnectAsync<IGamingHub, IGamingHubReceiver>(grpcChannel, this);

        var roomPlayers = await _client.JoinAsync(roomName, uuid, Vector3.zero, Quaternion.identity);
        foreach (var player in roomPlayers)
        {
            (this as IGamingHubReceiver).OnJoin(player);
        }

        return _players[uuid];
    }

    // methods send to server.
    public async ValueTask LeaveAsync()
    {
        if (_client != null)
            await _client.LeaveAsync();
    }

    public async ValueTask MoveAsync(Vector3 position, Quaternion rotation)
    {
        if (_client != null)
            await _client.MoveAsync(position, rotation);
    }

    // dispose client-connection before channel.ShutDownAsync is important!
    public async ValueTask DisposeAsync()
    {
        if (_client != null)
            await _client.DisposeAsync();
    }

    // You can watch connection state, use this for retry etc.
    public async ValueTask WaitForDisconnect()
    {
        if (_client != null)
            await _client.WaitForDisconnect();
    }

    // Receivers of message from server.

    public void OnLeave(Player player)
    {
        Debug.Log("Leave Player:" + player.UUID);

        if (_players.TryGetValue(player.UUID, out var cube))
        {
            Object.Destroy(cube);
        }
    }

    public void OnMove(Player player)
    {
        Debug.Log($"Move Player:{player.UUID} Position:{player.Position}");

        if (_players.TryGetValue(_uuid, out var playerObject))
        {
            playerObject.transform.SetPositionAndRotation(player.Position, player.Rotation);
        }
    }

    public void OnJoin(Player player)
    {
        Debug.Log("Join Player:" + player.UUID);
        if (player.UUID == _uuid)
        {
            if (_players.TryGetValue(_uuid, out var playerObject) == false)
            {
                playerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                playerObject.name = player.UUID.ToString();
                playerObject.transform.SetPositionAndRotation(player.Position, player.Rotation);
                _players[player.UUID] = playerObject;
            }
        }
        else
        {
            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playerObject.name = player.UUID.ToString();
            playerObject.transform.SetPositionAndRotation(player.Position, player.Rotation);
            _players[player.UUID] = playerObject;
        }
    }
}