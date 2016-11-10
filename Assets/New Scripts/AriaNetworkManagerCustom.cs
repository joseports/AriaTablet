using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.New_Scripts
{

    public class AriaNetworkManagerCustom : NetworkManager
    {
        public NetworkManager manager;
        [SerializeField]
        public bool showGUI = true;
        [SerializeField]
        public int offsetX;
        [SerializeField]
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
            GameObject player = null;
            UnityEngine.Networking.PlayerController playerController = conn.playerControllers[(int)playerControllerId];

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


        void OnGUI()
        {
            if (!showGUI)
                return;
            int xpos = 10 + offsetX;
            // int ypos = 40 + offsetY;
            int ypos = 90 + offsetY;
            int spacing = 35;
            int spacingextra = 70;
            //string conn = "10.128.93.241";
            string conn = "10.128.92.9";
            //string conn;

            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Host(H)"))
                {
                    manager.StartHost();
                }
                ypos += spacing;

                if (GUI.Button(new Rect(xpos, ypos, 105, 40), "LAN Client(C)"))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), manager.networkAddress);
                manager.networkAddress = conn;
                ypos += spacingextra;

                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Server Only(S)"))
                {
                    manager.StartServer();
                }
                ypos += spacing;
            }
            else
            {
                if (NetworkServer.active)
                {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
                    ypos += spacing;
                }
                if (NetworkClient.active)
                {
                    GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacing;
                }
            }




        }
    }


}

