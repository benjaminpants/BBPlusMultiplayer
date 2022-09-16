using System;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.NameMenu;
using Hazel.Udp;
using Hazel;
using System.Net;
using BaldiNetworking;
using UnityEngine;
using UnityEngine.SceneManagement;
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

		public static PlayerClient MyPlayer;

		public static byte MyPlayerID;

		public static bool AllowStart;


		void NameClicked(string name)
        {
			MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ClientPacket, (byte)ClientRPCs.SetName);
			Console.WriteLine(MyPlayer.PlayerID);
			writer.Write(MyPlayer.PlayerID);
			writer.Write(name);
			writer.EndMessage();
			connection.Send(writer);
			writer.Recycle();
		}

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
						case (byte)ServerRPCs.PlayerJoined:
							PlayerClient cl = new PlayerClient();
							cl.AmHost = data.Message.ReadBoolean();
							cl.PlayerID = data.Message.ReadByte();
							cl.Username = data.Message.ReadString();

							cl.NetState = PlayerNetState.Connected;
							Players.Add(cl);
							Console.WriteLine($"{cl.Username} has joined!");
							break;

						case (byte)ServerRPCs.PlayerLeft:
							byte toremove = data.Message.ReadByte();
							PlayerClient clienttoremove = Players.Find(x => x.PlayerID == toremove);
							if (clienttoremove == null)
                            {
								Console.WriteLine("Tried to remove non existant client(client disconnected before sending name?)");
								return;
                            }
							Console.WriteLine($"{clienttoremove.Username} has left!");
							Players.Remove(clienttoremove);
							break;


						case (byte)ServerRPCs.WelcomeSendData:
							Console.WriteLine("Succesfully recieved welcome message from server!");
							AmIHost = data.Message.ReadBoolean();
							GameDat = GameData.Deserialize(data.Message);
							MyPlayerID = data.Message.ReadByte();
							Players = data.Message.ReadPlayerList();
							MyPlayer = Players.Find(p => p.PlayerID == MyPlayerID);
							if (MyPlayer == null)
							{
								Players.Add(new PlayerClient(null, AmIHost, MyPlayerID));
								MyPlayer = Players[Players.Count - 1];
							}
							NameMenuManager.AllowContinue(true);
							break;
						case (byte)ServerRPCs.ShowGameStart:
							AllowStart = true;
							break;
						case (byte)ServerRPCs.GameStarted:
							Singleton<ElevatorScreen>.Instance.StartGame();
							break;
						default:
							break;
					}
					break;
				default:
					Console.WriteLine("Unknown/Unimplemented Packet Type:" + data.Message.Tag);
					break;
			}
			data.Message.Recycle();
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
			NameMenuManager.OnNameClicked += NameClicked;

		}
	}
}
