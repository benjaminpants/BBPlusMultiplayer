using Hazel;
using System;
using UnityEngine;
using HarmonyLib;

namespace BaldiMultiplayerBepInEx
{
	[HarmonyPatch(typeof(CoreGameManager))]
	[HarmonyPatch("Seed")]
	class HjackSeed
	{
		static bool Prefix(ref int __result)
		{
			__result = 128;
			return false;
		}
	}
}
