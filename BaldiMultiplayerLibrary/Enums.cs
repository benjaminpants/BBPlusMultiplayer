using System;
using System.Collections.Generic;
using System.Text;

namespace BaldiNetworking
{
	public enum TopRPCs : byte
	{
		ServerPacket = 3,
		GameStatePacket,
		ObjectPacket
	}


	public enum ServerRPCs : byte
	{
		WelcomeSendData,
		ChangeHost,
		GameStarted
	}
}
