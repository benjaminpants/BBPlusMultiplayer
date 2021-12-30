using System;
using Hazel;
using UnityEngine;

namespace BaldiNetworking
{
	static class NetExtensions
	{

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
