using HarmonyLib;
using Kingmaker.BundlesLoading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace ModTagEx;

#if DEBUG
[EnableReloading]
#endif
public static class Main
{
    internal static Harmony HarmonyInstance;
    internal static UnityModManager.ModEntry.ModLogger log;
    public static ModTagExSettings Settings;

    public static Dictionary<Guid, string> GuidModDict = [];
    public static HashSet<Guid> BaseGameGuids = [];

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
        Settings = UnityModManager.ModSettings.Load<ModTagExSettings>(modEntry);
        log = modEntry.Logger;
#if DEBUG
        modEntry.OnUnload = OnUnload;
#endif
        modEntry.OnGUI = OnGUI;
        modEntry.OnSaveGUI = OnSaveGUI;
        Updater.Update(modEntry);
        LoadModBlueprintGuids(modEntry);
        LoadBaseGameGuids();
        HarmonyInstance = new Harmony(modEntry.Info.Id);
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        return true;
    }

    private static void LoadModBlueprintGuids(UnityModManager.ModEntry modEntry)
    {
        var blueprintDbPath = Path.Combine(modEntry.Path, "all_blueprints.txt");

        if (File.Exists(blueprintDbPath))
        {
            var sw = Stopwatch.StartNew();
            foreach (var line in File.ReadLines(blueprintDbPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.Length < 36) continue; // skip empty lines

                var p1 = line.Substring(0, 32);
                var p2 = line.Substring(35);

                if (Guid.TryParse(p1, out Guid guid))
                {
                    GuidModDict[guid] = p2;
                }
            }
            log.Log($"Took {sw.Elapsed} to load guid db of size {GuidModDict.Count}");
        }
        else
        {
            log.Log("all_blueprints.txt file not found. All modded content will be marked as 'Unknown mod'");
        }

    }

    private static void LoadBaseGameGuids()
    {
        var sw = Stopwatch.StartNew();
        string path = BundlesLoadService.BundlesPath("blueprints-pack.bbp");
        var m_PackFile = new FileStream(path, FileMode.Open, FileAccess.Read);
        byte[] array = new byte[16];
        using BinaryReader binaryReader = new(m_PackFile, Encoding.UTF8, true);
        int num = binaryReader.ReadInt32();
        for (int i = 0; i < num; i++)
        {
            binaryReader.Read(array, 0, 16);
            Guid key = new(array);
            BaseGameGuids.Add(key);
            _ = binaryReader.ReadUInt32();
        }
        log.Log($"Took {sw.Elapsed} to load base game guids of size {BaseGameGuids.Count}");
    }

    static void OnGUI(UnityModManager.ModEntry modEntry)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tag modded content:");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.LevelUpClass = UITools.CheckboxToggle(Settings.LevelUpClass);
        GUILayout.Label("Class in level up interface");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.LevelUpArchetype = UITools.CheckboxToggle(Settings.LevelUpArchetype);
        GUILayout.Label("Archetype in level up interface");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.LevelUpRace = UITools.CheckboxToggle(Settings.LevelUpRace);
        GUILayout.Label("Race in level up interface");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.LevelUpFeature = UITools.CheckboxToggle(Settings.LevelUpFeature);
        GUILayout.Label("Feature in level up interface");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.UIFeature = UITools.CheckboxToggle(Settings.UIFeature);
        GUILayout.Label("Feature in normal UI");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.Ability = UITools.CheckboxToggle(Settings.Ability);
        GUILayout.Label("Ability and spell");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.ActivatableAbility = UITools.CheckboxToggle(Settings.ActivatableAbility);
        GUILayout.Label("Activatable Ability");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.Buff = UITools.CheckboxToggle(Settings.Buff);
        GUILayout.Label("Buff");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.Item = UITools.CheckboxToggle(Settings.Item);
        GUILayout.Label("Item");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("---------------------------");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Settings.AutoUpdateDB = UITools.CheckboxToggle(Settings.AutoUpdateDB);
        GUILayout.Label("Automatically download & update blueprint database on startup");
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            Settings.Save(modEntry);
        }
    }
    static void OnSaveGUI(UnityModManager.ModEntry modEntry)
    {
        Settings.Save(modEntry);
    }

#if DEBUG
    public static bool OnUnload(UnityModManager.ModEntry modEntry)
    {
        HarmonyInstance.UnpatchAll(modEntry.Info.Id);
        return true;
    }
#endif
}
