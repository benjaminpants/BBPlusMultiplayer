using System;
using System.Collections.Generic;
using System.Text;

namespace BaldiNetworking
{
	public enum TopRPCs : byte
	{
		ServerPacket = 3,
		GameStatePacket,
		ClientPacket,
		ObjectPacket,
		PlayerObjectPacket
	}


	public enum ServerRPCs : byte
	{
		WelcomeSendData,
		ChangeHost,
		GameStarted,
		PlayerJoined,
		PlayerLeft,
		ShowGameStart
	}

	public enum ClientRPCs : byte
	{
		SetName,
		EnterElevator,
		LevelLoaded,
		GameStart
	}

}
