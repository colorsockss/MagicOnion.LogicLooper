using System;
using System.Net.Http;
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnionShared;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameStarter : MonoBehaviour
{
    private IMyFirstService? _client;
    private GamingHubClient? _hub;

    [SerializeField]
    private string? _host;

    [SerializeField]
    private string? _port;

    private async void Start()
    {
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

        var httpHandler = new YetAnotherHttpHandler
        {
            Http2Only = true,
            SkipCertificateVerification = true,
        };

        var channelOptions = new GrpcChannelOptions
        {
            HttpClient = new HttpClient(httpHandler),
            DisposeHttpClient = true,
        };

        var channel = GrpcChannel.ForAddress($"{_host}:{_port}", channelOptions);
        _client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await _client.SumAsync(10, 20);
        Debug.Log($"result: {result}");
        _hub = new GamingHubClient();
        var uuid = Random.Range(0, int.MaxValue);
        await _hub.ConnectAsync(channel, "TestRoom", uuid);
        await _hub.MoveAsync(Random.onUnitSphere * 2, Quaternion.identity);
    }

    private void OnDestroy()
    {
        DisconnectAsync().Forget();
    }

    private async UniTask DisconnectAsync()
    {
        if (_hub != null)
        {
            await _hub.LeaveAsync();
            await _hub.DisposeAsync();
            await _hub.WaitForDisconnect();
        }
    }

    private void OnApplicationQuit()
    {
        DisconnectAsync().Forget();
    }
}