using UnityEngine;

namespace BaldiNetworking
{
	public class NetObjectBehavior : MonoBehaviour
	{
		public NetObject Object;

		public static implicit operator NetObject(NetObjectBehavior d) => d.Object; //allows you to just pass netobjectbehavior directly

	}
}
