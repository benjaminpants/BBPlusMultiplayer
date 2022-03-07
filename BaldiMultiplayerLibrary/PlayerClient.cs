using Hazel;
using UnityEngine;
using System;

namespace BaldiNetworking
{

	public enum PlayerNetState
	{
		Disconnected,
		Connected,
		FullyLoaded,
		Waiting,
		Ingame
	}

	public class PlayerClient
	{
		public Connection Connection;
		public byte PlayerID = 0;
		public bool AmHost = false;
		public string Username = "Loading...";
		public PlayerNetState NetState = PlayerNetState.Disconnected;

		public PlayerClient()
		{
		}

		public void DisconnectPlayer(object sender, DisconnectedEventArgs e)
		{
			Console.WriteLine(Username + " has disconnected: " + e.Reason);
			NetState = PlayerNetState.Disconnected;
		}

		public PlayerClient(Connection connection, bool host, byte id)
		{
			Connection = connection;
			NetState = PlayerNetState.Connected;
			PlayerID = id;
			AmHost = host;
		}

		public void Disconnect()
		{
			Connection = null;
			NetState = PlayerNetState.Disconnected;
		}

	}
}
