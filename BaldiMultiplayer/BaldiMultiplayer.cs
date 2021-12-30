using System;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.NameMenu;
using Hazel.Udp;
using Hazel;
using System.Net;

namespace BaldiMultiplayer
{


	[BepInPlugin("mtm101.rulerp.baldiplus.bbmultiplayer", "Baldi's Basics Plus Multiplayer", "0.0.0.0")]
	public class BasePlugin : BaseUnityPlugin
	{

		public static Connection connection;


		void Connect(MenuObject ject)
		{
			IPAddress address = IPAddress.Loopback;


			IPEndPoint endPoint = new IPEndPoint(address, 25565);

			connection = new UdpClientConnection(endPoint);

			connection.Connect();


		}


		void Awake()
		{
			Harmony harmony = new Harmony("mtm101.rulerp.baldiplus.bbmultiplayer");
			harmony.PatchAll();
			NameMenuManager.AddPreStartPage("mp_connect_button", true);
			NameMenuManager.AddToPage("mp_connect_button",new MenuGeneric("connect_button", "Connect", Connect));

		}
	}
}
