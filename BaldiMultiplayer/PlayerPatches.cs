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
using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace BaldiMultiplayer
{
	[HarmonyPatch(typeof(PlayerManager))]
	[HarmonyPatch("Update")]
	class DisableMapPatch
	{
		static bool Prefix(PlayerManager __instance, ref float ___guiltTime, ref float ___playerTimeScale)
        {
			if (___guiltTime > 0f)
			{
				___guiltTime -= Time.deltaTime * ___playerTimeScale;
				return false;
			}
			if (__instance.disobeying)
			{
				__instance.disobeying = false;
				___guiltTime = 0f;
			}
			return false;
        }
	}

	[HarmonyPatch(typeof(PlayerMovement))]
	[HarmonyPatch("Update")]
	class MovementNetworkPatch
	{
		public static float SendUpdateTimer = 0.3f;
		public static Vector3 LastPushedPosition = Vector3.zero;
		static bool Prefix(PlayerMovement __instance)
		{
			if (__instance.pm.playerNumber == 0)
            {
				SendUpdateTimer -= Time.deltaTime;
				if (SendUpdateTimer <= 0f)
                {
					if (Vector3.Distance(__instance.gameObject.transform.position,LastPushedPosition) >= 0f)
					{
						MessageWriter writer = PacketStuff.StartPacket(SendOption.None, TopRPCs.PlayerObjectPacket, (byte)PlayerObjectRPCs.UpdatePosition);
						writer.Write(BasePlugin.MyPlayerID);
						writer.Write(__instance.gameObject.transform.position);
						writer.Write(__instance.gameObject.transform.rotation);
						writer.Write(__instance.cc.velocity);
						LastPushedPosition = __instance.gameObject.transform.position;
						writer.EndMessage();
						BasePlugin.connection.Send(writer);
					}
					SendUpdateTimer = 0.3f;

				}
				return true;
            }
			else
            {
				return false;
            }
		}
	}
}