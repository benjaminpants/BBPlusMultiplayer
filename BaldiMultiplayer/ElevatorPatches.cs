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
using System.Collections;

namespace BaldiMultiplayer
{

    [HarmonyPatch(typeof(ElevatorScreen))]
    [HarmonyPatch("Update")]
    class PreventElevatorStart
    {
        static bool Prefix(ElevatorScreen __instance, ref bool ___busy, ref bool ___readyToStart, ref GameObject ___playButton, ref GameObject ___saveButton, ref List<IEnumerator> ___queuedEnumerators)
        {
            ___saveButton.SetActive(false);
            if (___queuedEnumerators.Count > 0 && !___busy)
            {
                __instance.StartCoroutine(___queuedEnumerators[0]);
                ___busy = true;
                ___queuedEnumerators.RemoveAt(0);
            }
            if (Singleton<CoreGameManager>.Instance.readyToStart && !___playButton.activeSelf && !___busy && !___readyToStart && BasePlugin.MyPlayer.NetState != PlayerNetState.LevelGeneratedWaiting)
            {
                MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ClientPacket, (byte)ClientRPCs.LevelLoaded);
                writer.Write(BasePlugin.MyPlayerID);
                BasePlugin.MyPlayer.NetState = PlayerNetState.LevelGeneratedWaiting;
                writer.EndMessage();
                BasePlugin.connection.Send(writer);
            }
            if (BasePlugin.AllowStart)
            {
                BasePlugin.AllowStart = false;
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(ElevatorScreen))]
    [HarmonyPatch("ButtonPressed")]
    class HookWhenGameStart
    {
        static void Prefix()
        {
            MessageWriter writer = PacketStuff.StartPacket(SendOption.Reliable, TopRPCs.ClientPacket, (byte)ClientRPCs.GameStart);
            writer.Write(BasePlugin.MyPlayerID);
            BasePlugin.MyPlayer.NetState = PlayerNetState.Ingame;
            writer.EndMessage();
            BasePlugin.connection.Send(writer);
        }
    }
}
