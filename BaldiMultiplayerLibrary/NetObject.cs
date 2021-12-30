using System;
using Hazel;
using UnityEngine;

namespace BaldiNetworking
{
	public class NetObject : MonoBehaviour
	{

		public int ObjectID;

		public virtual void Serialize(MessageWriter writer)
		{
			writer.Write(ObjectID);
		}

		public virtual void Deserialize(MessageReader reader)
		{
			//ignore object ID as that should be handled by whatever code is doing the thing
		}
	}
}
