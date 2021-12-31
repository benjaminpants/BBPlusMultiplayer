using Hazel;
using Hazel.Udp;
using System;

namespace BaldiNetworking
{
	public class GameData
	{

		public int Seed = 128;

		public void Serialize(ref MessageWriter writer)
		{
			writer.Write(Seed);
		}

		public static GameData Deserialize(MessageReader reader)
		{
			GameData data = new GameData();
			data.Seed = reader.ReadInt32();

			return data;
		}


	}
}
