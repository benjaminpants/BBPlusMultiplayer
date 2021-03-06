using System;
using System.Collections.Generic;
using System.Net;
using Hazel;
using Hazel.Udp;
using BaldiNetworking;

namespace BaldiServer
{
	class Program
	{
		static ConnectionListener listener;

		public static List<PlayerClient> Players = new List<PlayerClient>();

		public static GameData Data = new GameData();


		public static void SendToAllPlayers(MessageWriter writer)
		{
			foreach (PlayerClient player in Players)
			{
				if (player.NetState == PlayerNetState.Disconnected || player.Connection == null) continue;
				player.Connection.Send(writer);
			}
			writer.Recycle();
		}




		static int ClearDisconnectedPlayers()
		{
			int amount_cleared = Players.Count;
			Players.RemoveAll(p => p.NetState == PlayerNetState.Disconnected);
			amount_cleared -= Players.Count;
			if (Players.Find(p => p.AmHost) == null)
			{
				if (Players[0] != null)
				{
					Players[0].AmHost = true;
				}
			}
			return amount_cleared;
		}

		public static void DisconnectPlayer(object sender, DisconnectedEventArgs e)
		{
			ClearDisconnectedPlayers();
		}

		static PlayerClient CreatePlayer(Connection connect, bool amhost)
		{
			PlayerClient client = new PlayerClient(connect,amhost,(byte)(Players.Count + 1));
			Players.Add(client);
			connect.Disconnected += client.DisconnectPlayer;
			connect.Disconnected += DisconnectPlayer;
			return client;
		}


		static void Main(string[] args)
		{
			IPEndPoint endpoint = new IPEndPoint(0, 25565);
			listener = new UdpConnectionListener(endpoint);
			listener.NewConnection += NewConnectionHandler;
			listener.Start();
			Console.WriteLine("The server has loaded");
			CommandLoop();
		}

		static void DataRecieved(DataReceivedEventArgs data)
		{
			data.Message.ReadByte();
			data.Message.ReadByte();
			TopRPCs packet_type = (TopRPCs)data.Message.ReadByte();
			byte second_tag = data.Message.ReadByte();
			switch (packet_type)
			{
				case TopRPCs.ServerPacket:
					Console.WriteLine("Why did I get sent a server packet? Odd. Disregarding.");
					break;
				case TopRPCs.ClientPacket:
					ClientRPCs rpc = (ClientRPCs)second_tag;
					switch (rpc)
					{
						case ClientRPCs.SetName:
							byte ID = data.Message.ReadByte();
							PlayerClient client = Players.Find(p => p.PlayerID == ID);
							if (client == null) return;
							if (client.Connection.EndPoint != data.Sender.EndPoint) return;
							List<string> CurrentPlayerUsernames = new List<string>();
							for (int i = 0; i < Players.Count; i++)
							{
								CurrentPlayerUsernames.Add(Players[i].Username);
							}
							client.Username = VerificationChecks.VerifyUsername(data.Message.ReadString(), CurrentPlayerUsernames);
							client.NetState = PlayerNetState.FullyLoaded;
							Console.WriteLine(client.Username + " has joined and gotten their username verified!");
							break;
						case ClientRPCs.EnterElevator:
							byte ID_b = data.Message.ReadByte();
							PlayerClient client_b = Players.Find(p => p.PlayerID == ID_b);
							if (client_b == null) return;
							if (client_b.Connection.EndPoint != data.Sender.EndPoint) return;
							client_b.NetState = PlayerNetState.Waiting;
							Console.WriteLine(client_b.Username + " has gotten in the elevator and is waiting.");
							break;
					}
					break;
			}
			data.Message.Recycle();
		}

		static void CommandLoop()
		{
			string thingy = Console.ReadLine();
			CommandLoop();
		}


		static void NewConnectionHandler(NewConnectionEventArgs args)
		{
			Connection connect = args.Connection;
			Console.WriteLine("Connection recieved, attempting to send data packet!");
			connect.DataReceived += DataRecieved;
			MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable,TopRPCs.ServerPacket,(byte)ServerRPCs.WelcomeSendData);
			writer.Write(Players.Count == 0); //Should this player be the host?
			

			PlayerClient pl = CreatePlayer(connect, Players.Count == 0);
			Data.Serialize(ref writer);
			writer.Write(pl.PlayerID);
			writer.Write(Players);
			writer.EndMessage();
			connect.Send(writer);
			writer.Recycle();
		}


	}
}
