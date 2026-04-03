using UnityModManagerNet;

namespace ModTagEx;

public class ModTagExSettings : UnityModManager.ModSettings
{
    public bool LevelUpClass = true;
    public bool LevelUpArchetype = true;
    public bool LevelUpRace = true;
    public bool LevelUpFeature = true;
    public bool UIFeature = true;
    public bool Ability = true;
    public bool ActivatableAbility = true;
    public bool Buff = true;
    public bool Item = true;
    public string DatabaseCommit = "";
    public long LastUpdateTS;
    public bool AutoUpdateDB = false;

    public new void Save(UnityModManager.ModEntry modEntry) => Save(this, modEntry);
}
