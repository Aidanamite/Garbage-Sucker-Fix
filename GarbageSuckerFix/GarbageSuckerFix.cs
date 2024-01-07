using UnityEngine;
using HarmonyLib;
using HMLLibrary;

public class GarbageSuckerFix : Mod
{
    Harmony harmony;
    public void Start()
    {
        harmony = new Harmony("com.aidanamite.GarbageSuckerFix");
        harmony.PatchAll();
        Log("Garbage suckers have been patched!");
    }
    public void OnModUnload()
    {
        harmony.UnpatchAll(harmony.Id);
        Log("Garbage suckers have been unpatched!");
    }
}

[HarmonyPatch(typeof(BlockQuad), "AcceptsBlock")]
public class Patch_Accept
{
	static void Postfix(ref BlockQuad __instance, ref Item_Base blockItem, ref bool __result, Vector3 surfaceNormal)
	{
		if (blockItem.UniqueName == "placeable_garbagesucker")
		{
			var pivot = SingletonGeneric<GameManager>.Singleton.lockedPivot;
			if (surfaceNormal != pivot.up)
            {
				__result = false;
				return;
            }
			var p = pivot.InverseTransformPoint(__instance.transform.position);
			foreach (var block in BlockCreator.GetPlacedBlocks())
				if (block is Block_Foundation_ItemNet && p == block.transform.localPosition)
				{
					__result = true;
					return;
				}
			__result = false;
		}
	}
}