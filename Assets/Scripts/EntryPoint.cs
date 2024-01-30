using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Spawning;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FishNet.Transporting;
using ParrelSync;

namespace Game.Views.GamePlay
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        private Tugboat tugboat;

        private async void Awake()
        {
            tugboat = networkManager.TransportManager.GetTransport<Tugboat>();

            if (!ClonesManager.IsClone())
            {
                await StartServer();
            }

            await StartClient();
        }

        private async UniTask StartServer()
        {
            tugboat.StartConnection(true);

            await UniTask.WaitUntil(() =>
                tugboat.GetConnectionState(true) == LocalConnectionState.Started);
        }

        private async UniTask StartClient()
        {
            tugboat.StartConnection(false);

            await UniTask.WaitUntil(() =>
                tugboat.GetConnectionState(false) == LocalConnectionState.Started);
        }
    }
}
