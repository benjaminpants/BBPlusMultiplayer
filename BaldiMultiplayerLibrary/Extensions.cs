using System;
using Hazel;
using UnityEngine;
using System.Collections.Generic;

namespace BaldiNetworking
{
	public static class NetExtensions
	{


		public static void Write(this MessageWriter writer, PlayerClient player)
		{
			writer.Write(player.AmHost);
			writer.Write(player.PlayerID);
			writer.Write(player.Username == "Loading..." ? "" : player.Username);
		}


		public static void Write(this MessageWriter writer, List<PlayerClient> list)
		{
			writer.Write((byte)list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				writer.Write(list[i].AmHost);
				writer.Write(list[i].PlayerID);
			}
		}

		public static List<PlayerClient> ReadPlayerList(this MessageReader reader)
		{
			List<PlayerClient> update = new List<PlayerClient>();
			int length = (int)reader.ReadByte();
			for (int i = 0; i < length; i++)
			{
				update.Add(new PlayerClient(null, reader.ReadBoolean(), reader.ReadByte()));
			}
			return update;
		}

		public static void Write(this MessageWriter writer, Vector3 vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
			writer.Write(vec.z);
		}

		public static Vector3 ReadVector3(this MessageReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this MessageWriter writer, Quaternion quar)
		{
			writer.Write(quar.x);
			writer.Write(quar.y);
			writer.Write(quar.z);
			writer.Write(quar.w);
		}

		public static Quaternion ReadQuaternion(this MessageReader reader)
		{
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static void Write(this MessageWriter writer, MovementModifier mod)
		{
			bool WriteAddend = mod.movementAddend != Vector3.zero;

			writer.Write(WriteAddend);
			if (WriteAddend)
			{
				writer.Write(mod.movementAddend);
			}
			writer.Write(mod.movementMultiplier);
		}


		public static void Write(this MessageWriter writer, ActivityModifier actmod)
		{
			writer.Write(actmod.moveMods.Count);
			for (int i = 0; i < actmod.moveMods.Count; i++)
			{
				writer.Write(actmod.moveMods[i]);
			}
		}

		public static MovementModifier ReadMovementMod(this MessageReader reader)
		{
			if (reader.ReadBoolean())
			{
				return new MovementModifier(reader.ReadVector3(), reader.ReadSingle());
			}
			else
			{
				return new MovementModifier(Vector3.zero, reader.ReadSingle());
			}
		}


	}
}
