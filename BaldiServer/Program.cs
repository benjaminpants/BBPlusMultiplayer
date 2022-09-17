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

		public static List<NetObject> Objects = new List<NetObject>();

		public static GameData Data = new GameData();

		public static void SendToAllPlayers(MessageWriter writer, byte except = 255)
		{
			foreach (PlayerClient player in Players)
			{
				if (player.NetState == PlayerNetState.Disconnected || player.Connection == null || player.PlayerID == except) continue;
				player.Connection.Send(writer);
			}
			writer.Recycle();
		}

		public static MessageWriter CreateNewPlayerPacket(PlayerClient cl)
        {
			MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ServerPacket, (byte)ServerRPCs.PlayerJoined);
			writer.Write(cl.AmHost);
			writer.Write(cl.PlayerID);
			writer.Write(cl.Username);
			writer.EndMessage();
			return writer;
		}

		public static MessageWriter CreateNewPlayerLeftPacket(PlayerClient cl)
		{
			MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ServerPacket, (byte)ServerRPCs.PlayerLeft);
			writer.Write(cl.PlayerID);
			writer.EndMessage();
			return writer;
		}

		public static PlayerClient Host { 
			get
            {
				return Players.Find(x => x.AmHost);
            }
			set
            {
				Host.AmHost = false;
				value.AmHost = true;

			}
		
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
			foreach (PlayerClient cl in Players.FindAll(p => p.NetState == PlayerNetState.Disconnected))
			{
				SendToAllPlayers(CreateNewPlayerLeftPacket(cl));
			}
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

		static void CheckForLevelLoad()
        {
			if (Players.Count == 0) return;
			int loadedplayers = 0;
			foreach (PlayerClient player in Players)
            {
				loadedplayers += (player.NetState == PlayerNetState.LevelGeneratedWaiting ? 1 : 0);
            }
			if (loadedplayers == Players.Count)
            {
				Console.WriteLine("All players have loaded the level, sending host StartGame packet..");
				MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ServerPacket, (byte)ServerRPCs.ShowGameStart);
				writer.EndMessage();
				Host.Connection.Send(writer);

			}
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
							if (client == null) break;
							if (client.Connection.EndPoint != data.Sender.EndPoint) break;
							List<string> CurrentPlayerUsernames = new List<string>();
							for (int i = 0; i < Players.Count; i++)
							{
								CurrentPlayerUsernames.Add(Players[i].Username);
							}
							client.Username = VerificationChecks.VerifyUsername(data.Message.ReadString(), CurrentPlayerUsernames);
							client.NetState = PlayerNetState.FullyLoaded;
							Console.WriteLine(client.Username + " has joined and gotten their username verified!");
							SendToAllPlayers(CreateNewPlayerPacket(client), ID);
							break;
						case ClientRPCs.EnterElevator:
							byte ID_b = data.Message.ReadByte();
							PlayerClient client_b = Players.Find(p => p.PlayerID == ID_b);
							if (client_b == null) break;
							if (client_b.Connection.EndPoint != data.Sender.EndPoint) break;
							client_b.NetState = PlayerNetState.Waiting;
							Console.WriteLine(client_b.Username + " has gotten in the elevator and is waiting.");
							break;
						case ClientRPCs.LevelLoaded:
							byte ID_c = data.Message.ReadByte();
							PlayerClient client_c = Players.Find(p => p.PlayerID == ID_c);
							if (client_c == null) break;
							if (client_c.Connection.EndPoint != data.Sender.EndPoint) break;
							client_c.NetState = PlayerNetState.LevelGeneratedWaiting;
							Console.WriteLine(client_c.Username + " has loaded into the level.");
							CheckForLevelLoad();
							break;
						case ClientRPCs.GameStart:
							byte ID_d = data.Message.ReadByte();
							PlayerClient client_d = Players.Find(p => p.PlayerID == ID_d);
							if (client_d == null) break;
							if (!client_d.AmHost) break;
							if (client_d.Connection.EndPoint != data.Sender.EndPoint) break;
							MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ServerPacket, (byte)ServerRPCs.GameStarted);
							writer.EndMessage();
							SendToAllPlayers(writer);
							break;
					}
					break;
				case TopRPCs.PlayerObjectPacket:
					PlayerObjectRPCs rpc_x = (PlayerObjectRPCs)second_tag;
					byte ID_x = data.Message.ReadByte();
					PlayerClient client_x = Players.Find(p => p.PlayerID == ID_x);
					if (client_x == null) break;
					if (client_x.Connection.EndPoint != data.Sender.EndPoint) break;
					switch (rpc_x)
                    {
						case PlayerObjectRPCs.UpdatePosition:
							MessageWriter writer = PacketStuff.StartPacket(SendOption.None, TopRPCs.PlayerObjectPacket, (byte)PlayerObjectRPCs.UpdatePosition);
							writer.Write(ID_x);
							writer.Write(data.Message.ReadVector3());
							writer.Write(data.Message.ReadQuaternion());
							writer.Write(data.Message.ReadVector3());
							writer.EndMessage();
							SendToAllPlayers(writer);
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
			Console.WriteLine("Connection recieved, attempting to send welcome packet!");
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
