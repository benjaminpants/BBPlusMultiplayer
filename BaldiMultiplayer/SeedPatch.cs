using Hazel;
using System;
using UnityEngine;
using HarmonyLib;

namespace BaldiMultiplayer
{
	[HarmonyPatch(typeof(CoreGameManager))]
	[HarmonyPatch("Seed")]
	class HjackSeed
	{
		static bool Prefix(ref int __result)
		{
			__result = BasePlugin.GameDat.Seed;
			return false;
		}
	}
}
