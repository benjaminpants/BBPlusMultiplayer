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
	[HarmonyPatch(typeof(CoreGameManager))]
	[HarmonyPatch("SpawnPlayers")]
	class SetPlayerCountPatchf
	{
		static bool Prefix(CoreGameManager __instance, ref int ___setPlayers, ref bool ___restoreItemsOnSpawn, ref HudManager ___hudPref, ref PlayerManager[] ___players, ref GameCamera[] ___cameras, ref HudManager[] ___huds, ref EnvironmentController ec)
		{
			___setPlayers = BasePlugin.Players.Count;
			Console.WriteLine("Set player count to:" + Singleton<CoreGameManager>.Instance.setPlayers);
			for (int i = 0; i < __instance.setPlayers; i++)
			{
				BasePlugin.Players[i].AssignedPlayerObject = (byte)i;
				if (___players[i] == null)
				{
					//removed support for authentic mode b___ecause like. come on.
					___huds[i] = UnityEngine.Object.Instantiate<HudManager>(___hudPref);
					___huds[i].hudNum = i;
					___players[i] = UnityEngine.Object.Instantiate<PlayerManager>(__instance.playerPref);
					___players[i].playerNumber = i;
					___cameras[i] = UnityEngine.Object.Instantiate<GameCamera>(__instance.cameraPref);
					___cameras[i].canvasCam.transform.parent = null;
					UnityEngine.Object.DontDestroyOnLoad(___cameras[i].canvasCam);
					___cameras[i].UpdateTargets(___players[i].transform, 30);
					___cameras[i].offestPos = Vector3.zero;
					___cameras[i].camNum = i;
					if (i == 0)
					{
						___huds[i].Canvas().worldCamera = ___cameras[i].canvasCam;
						Singleton<GlobalCam>.Instance.ChangeType(CameraRenderType.Overlay);
						___cameras[i].camCom.GetUniversalAdditionalCameraData().cameraStack.Add(Singleton<GlobalCam>.Instance.RenderTexCam);
					}
				}
				else
				{
					PlayerManager playerManager = ___players[i];
					if (ec.Players[i] == null)
					{
						___players[i] = UnityEngine.Object.Instantiate<PlayerManager>(__instance.playerPref);
						___players[i].playerNumber = i;
						___players[i].itm.items = playerManager.itm.items;
						___cameras[i].UpdateTargets(___players[i].transform, 30);
						___cameras[i].offestPos = Vector3.zero;
						___cameras[i].controllable = true;
						___cameras[i].matchTargetRotation = true;
						___cameras[i].cameraModifiers = new List<CameraModifier>(___players[i].camMods);
						playerManager.gameObject.SetActive(false);
						playerManager.gameObject.SetActive(false);
					}
					else
					{
						___players[i] = ec.Players[i];
						___players[i].itm.items = playerManager.itm.items;
						___cameras[i].UpdateTargets(___players[i].transform, 30);
						___cameras[i].offestPos = Vector3.zero;
						___cameras[i].controllable = true;
						___cameras[i].matchTargetRotation = true;
						___cameras[i].cameraModifiers = new List<CameraModifier>(___players[i].camMods);
						playerManager.gameObject.SetActive(false);
						___players[i].gameObject.SetActive(true);
					}
					if (playerManager.ec == null)
					{
						UnityEngine.Object.Destroy(playerManager.gameObject);
					}
				}
				if (ec.map != null)
				{
					if (ec.map.cams.Count <= i)
					{
						ec.map.targets.Add(___players[i].transform);
						___cameras[i].mapCam = UnityEngine.Object.Instantiate<Camera>(___cameras[i].mapCamPre, ec.map.transform);
						ec.map.cams.Add(___cameras[i].mapCam);
						___cameras[i].mapCam.transform.rotation = Quaternion.identity;
						___cameras[i].camCom.GetUniversalAdditionalCameraData().cameraStack.Insert(1, ___cameras[i].mapCam);
					}
					else
					{
						___cameras[i].mapCam = ec.map.cams[i];
					}
				}
			}
			for (int i = 0; i < __instance.setPlayers; i++)
			{
				___players[i].transform.position = ec.spawnPoint;
				___players[i].transform.rotation = ec.spawnRotation;
				___players[i].plm.height = ec.spawnPoint.y;
				if (i != 0)
                {
					___cameras[i].camCom.enabled = false;
					___cameras[i].canvasCam.enabled = false;
					___huds[i].Hide(true);
				}
			}
			ec.AssignPlayers();

			if (___restoreItemsOnSpawn)
			{
				__instance.RestorePlayers();
				___restoreItemsOnSpawn = false;
			}
			Singleton<GlobalCam>.Instance.SetListener(false);
			//remove extra listeners
			for (int i = 0; i < VA_AudioListener.Instances.Count; i++)
            {
				GameObject.Destroy(VA_AudioListener.Instances[i]);
			}
			VA_AudioListener.Instances.RemoveRange(1, VA_AudioListener.Instances.Count - 1);
			return false;
		}
	}//Console.WriteLine(Singleton<CoreGameManager>.Instance.GetPlayer(1) == null);
}