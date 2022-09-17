using System;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.NameMenu;
using Hazel.Udp;
using Hazel;
using System.Net;
using BaldiNetworking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


namespace BaldiMultiplayer
{
	[HarmonyPatch(typeof(ElevatorScreen))]
	[HarmonyPatch("Initialize")]
	class LoadgamePatch
	{
		static void Postfix()
		{
			MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ClientPacket, (byte)ClientRPCs.EnterElevator);
			writer.Write(BasePlugin.MyPlayerID);
			writer.EndMessage();
			BasePlugin.connection.Send(writer);
		}
	}
}
