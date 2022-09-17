using System;
using System.Collections.Generic;
using System.Net;
using Hazel;
using Hazel.Udp;
using BaldiNetworking;

namespace BaldiNetworking
{
	public class PacketStuff
	{
		public static MessageWriter StartPacket(SendOption option, TopRPCs rpctype, byte data)
		{
			MessageWriter writer = MessageWriter.Get(option);//new MessageWriter(32);
			writer.Clear(option);
			writer.StartMessage((byte)rpctype);
			writer.Write(data);

			return writer;
		}

		public static MessageWriter StartPacketHeaderless(SendOption option, TopRPCs rpctype)
		{
			MessageWriter writer = MessageWriter.Get(option);//new MessageWriter(32);
			writer.Clear(option);
			writer.StartMessage((byte)rpctype);

			return writer;
		}
	}
}
