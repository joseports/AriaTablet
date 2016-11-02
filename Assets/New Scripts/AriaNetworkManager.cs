using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.New_Scripts
{
    public class AriaNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            GameObject player = null;
            UnityEngine.Networking.PlayerController playerController = conn.playerControllers[(int) playerControllerId];

            if (playerController != null)
            {
                player = playerController.gameObject;
            }
            if (player != null)
            {
                if (numPlayers == 1)
                {
                    SetupServer(player);
                }
                else
                {
                    player.name = "Client";
                }
            }
        }


        void SetupServer(GameObject player)
        {
            var mesh = player.GetComponentInChildren<MeshRenderer>();
            mesh.material.color = Color.magenta;
            player.name = "Server";
        }

    }
}
