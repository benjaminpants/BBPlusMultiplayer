using Hazel;
using UnityEngine;
using System.Collections.Generic;

namespace BaldiNetworking
{

	public enum PlayerRPCs : byte //maybe get guilty RPC isn't necessary but oh well
	{
		UpdatePosition,
		UseItem,
		GetGuilty
	}


	public class PlayerNetObject : NetObject
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public PlayerManager mymanager;

		//public List<MovementModifier> am;


		public void SendUseItemPacket(Connection tosend, byte slot)
		{
			MessageWriter writer = this.CreateMessageWriter(32,true, (byte)PlayerRPCs.UseItem);
			writer.Write(slot);
			writer.EndMessage();
			tosend.Send(writer);
			writer.Recycle();
		}


		public override void Serialize(MessageWriter writer)
		{
			base.Serialize(writer);
			writer.Write(Position);
			writer.Write(Rotation);
			//maybe dont write activity modifiers because of how heavily act modifiers rely on being spawned by certain scripts
			//writer.Write(am.Count);
			//for (int i = 0; i < am.Count; i++)
			//{
			//	writer.Write(am[i]);
			//}
		}

		public override void Deserialize(MessageReader reader)
		{
			base.Deserialize(reader);
			Position = reader.ReadVector3();
			Rotation = reader.ReadQuaternion();
			//am = reader.read
		}

	}
}
