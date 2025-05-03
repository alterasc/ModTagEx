using HarmonyLib;
using Kingmaker.UI.MVVM._VM.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Owlcat.Runtime.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModTagEx;

[HarmonyPatch]
internal static class Patches
{
    private static string GreyText(string text) => $"<color=grey>{text}</color>";

    private const string UNKNOWN_MOD_TAG = "Unknown mod";

    // level up class selection
    [HarmonyPatch(typeof(TooltipTemplateClassBuildInformation), nameof(TooltipTemplateClassBuildInformation.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void LevelUpClass(TooltipTemplateClassBuildInformation __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.LevelUpClass)
        {
            return;
        }
        if (__instance.ClassInfo?.Class == null)
        {
            return;
        }
        var guid = __instance.ClassInfo.Class.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid, true);
    }

    // level up archetype selection
    [HarmonyPatch(typeof(TooltipTemplateFirstLevelClass), nameof(TooltipTemplateFirstLevelClass.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void LevelUpArchetype(TooltipTemplateFirstLevelClass __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.LevelUpArchetype)
        {
            return;
        }
        if (__instance.ClassInfo.Class == null)
        {
            return;
        }

        var guid = __instance.ClassInfo.Archetype != null ? __instance.ClassInfo.Archetype.AssetGuid.m_Guid : __instance.ClassInfo.Class.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid, true);
    }

    //level up race selection
    [HarmonyPatch(typeof(TooltipTemplateLevelUpRace), nameof(TooltipTemplateLevelUpRace.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void LevelUpRace(TooltipTemplateLevelUpRace __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.LevelUpRace)
        {
            return;
        }
        if (__instance.m_Race == null)
        {
            return;
        }
        var guid = __instance.m_Race.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid, true);
    }

    // level up feature selection
    [HarmonyPatch(typeof(TooltipTemplateLevelUpFeature), nameof(TooltipTemplateLevelUpFeature.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void LevelUpFeature(TooltipTemplateLevelUpFeature __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.LevelUpFeature)
        {
            return;
        }
        if (__instance.FeatureInfo?.BlueprintFeature == null)
        {
            return;
        }
        var guid = __instance.FeatureInfo.BlueprintFeature.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    // feature
    [HarmonyPatch(typeof(TooltipTemplateUIFeature), nameof(TooltipTemplateUIFeature.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void Feature(TooltipTemplateUIFeature __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.UIFeature)
        {
            return;
        }
        if (__instance.m_UIFeature?.Feature == null)
        {
            return;
        }
        var guid = __instance.m_UIFeature.Feature.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    // ability
    [HarmonyPatch(typeof(TooltipTemplateAbility), nameof(TooltipTemplateAbility.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void Ability(TooltipTemplateAbility __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.Ability)
        {
            return;
        }
        if (__instance.BlueprintAbility == null)
        {
            return;
        }
        var guid = __instance.BlueprintAbility.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    // activatable ability
    [HarmonyPatch(typeof(TooltipTemplateActivatableAbility), nameof(TooltipTemplateActivatableAbility.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void ActivatableAbility(TooltipTemplateActivatableAbility __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.ActivatableAbility)
        {
            return;
        }
        if (__instance.BlueprintActivatableAbility == null)
        {
            return;
        }
        var guid = __instance.BlueprintActivatableAbility.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    // buff
    [HarmonyPatch(typeof(TooltipTemplateBuff), nameof(TooltipTemplateBuff.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void Buff(TooltipTemplateBuff __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.Buff)
        {
            return;
        }
        if (__instance.Buff?.Blueprint == null)
        {
            return;
        }
        var guid = __instance.Buff.Blueprint.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    // item
    [HarmonyPatch(typeof(TooltipTemplateItem), nameof(TooltipTemplateItem.GetBody))]
    [HarmonyPostfix]
    [HarmonyAfter("0ToyBox0")]
    public static void Item(TooltipTemplateItem __instance, ref IEnumerable<ITooltipBrick> __result)
    {
        if (!Main.Settings.Item)
        {
            return;
        }
        if (__instance.m_BlueprintItem == null && __instance.m_Item?.Blueprint == null)
        {
            return;
        }
        var guid = __instance.m_BlueprintItem != null ? __instance.m_BlueprintItem.AssetGuid.m_Guid : __instance.m_Item.Blueprint.AssetGuid.m_Guid;
        InjectGuidInformation(ref __result, guid);
    }

    private static void InjectGuidInformation(ref IEnumerable<ITooltipBrick> __result, Guid guid, bool centered = false)
    {
        var style = centered
            ? TooltipTextType.Small | TooltipTextType.Italic | TooltipTextType.Centered
            : TooltipTextType.Small | TooltipTextType.Italic;
        if (Main.GuidModDict.TryGetValue(guid, out var modName))
        {
            var list = __result.ToList();
            list.Insert(0, new TooltipBrickText(GreyText(modName), style));
            __result = list;
        }
        else if (!Main.BaseGameGuids.Contains(guid))
        {
            var list = __result.ToList();
            list.Insert(0, new TooltipBrickText(GreyText(UNKNOWN_MOD_TAG), style));
            __result = list;
        }
    }
}
