﻿using System;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.NameMenu;
using Hazel.Udp;
using Hazel;
using System.Net;
using BaldiNetworking;
using UnityEngine;
using System.Collections.Generic;

namespace BaldiMultiplayer
{


	[BepInPlugin("mtm101.rulerp.baldiplus.bbmultiplayer", "Baldi's Basics Plus Multiplayer", "0.0.0.0")]
	public class BasePlugin : BaseUnityPlugin
	{

		public static Connection connection;

		public static bool AmIHost = false;

		public static GameData GameDat;

		public static List<PlayerClient> Players = new List<PlayerClient>();

		void DataRecieved(DataReceivedEventArgs data)
		{


			data.Message.ReadByte(); //2 bytes of bullshit
			data.Message.ReadByte();
			byte packet_type = data.Message.ReadByte();
			byte second_tag = data.Message.ReadByte();
			switch (packet_type)
			{
				case (byte)TopRPCs.ServerPacket:
					switch (second_tag)
					{
						case (byte)ServerRPCs.WelcomeSendData:
							AmIHost = data.Message.ReadBoolean();
							GameDat = GameData.Deserialize(data.Message);
							NameMenuManager.AllowContinue(true);
							break;
						default:
							break;
					}
					break;
				default:
					Debug.LogWarning("Unknown/Unimplemented Packet Type:" + data.Message.Tag);
					break;
			}
		}

		void Connect(MenuObject ject)
		{
			IPAddress address = IPAddress.Loopback;


			IPEndPoint endPoint = new IPEndPoint(address, 25565);

			connection = new UdpClientConnection(endPoint);

			connection.DataReceived += DataRecieved;

			connection.Connect();


		}


		void Awake()
		{
			Harmony harmony = new Harmony("mtm101.rulerp.baldiplus.bbmultiplayer");
			harmony.PatchAll();
			NameMenuManager.AddPreStartPage("mp_connect_button", true);
			NameMenuManager.AddToPage("mp_connect_button",new MenuGeneric("connect_button", "Connect", Connect));

		}
	}
}
