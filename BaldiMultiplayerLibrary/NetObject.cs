using System;
using Hazel;
using UnityEngine;

namespace BaldiNetworking
{
	public class NetObject
	{

		public int ObjectID;
		public PlayerClient Owner;

		public virtual void Serialize(MessageWriter writer, bool InitialSend)
		{
			writer.Write(ObjectID);
			if (InitialSend)
            {
				writer.Write(Owner.PlayerID);
            }
		}



		public MessageWriter CreateMessageWriter(int buffersize, bool reliable, byte messageflag)
		{
			MessageWriter writer = new MessageWriter(buffersize + 32 + 8);
			if (reliable)
			{
				writer.Clear(SendOption.Reliable);
			}
			writer.StartMessage((byte)TopRPCs.ObjectPacket);
			writer.Write(ObjectID);
			writer.Write(messageflag);
			return writer;
		}

		public virtual void Deserialize(MessageReader reader)
		{
			//ignore object ID as that should have already been read by now.
		}
	}
}
