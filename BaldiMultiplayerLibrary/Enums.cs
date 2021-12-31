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
		ObjectPacket
	}


	public enum ServerRPCs : byte
	{
		WelcomeSendData,
		ChangeHost,
		GameStarted
	}

	public enum PlayerRPCs : byte
	{
		SetName,
		EnterElevator,
		GameStart
	}
}
