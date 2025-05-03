using UnityEngine;
using UnityModManagerNet;

namespace ModTagEx;

internal class UITools
{
    internal static bool CheckboxToggle(bool toggleState)
    {
        var text = toggleState ? "✔" : "✖";
        GUIStyle style = new(GUI.skin.button);
        style.normal.textColor = Color.red;
        style.hover.textColor = Color.red;
        style.hover.background = style.normal.background;
        style.onHover.background = style.normal.background;
        style.onHover.textColor = Color.green;
        style.onNormal.textColor = Color.green;
        style.onNormal.background = style.normal.background;
        float width = 40 * UnityModManager.Params.UIScale;
        return GUILayout.Toggle(toggleState, text, style, GUILayout.Width(width));
    }

    internal static bool ToggleButton(bool toggleState)
    {
        var text = toggleState ? "Enabled" : "Disabled";
        GUIStyle style = new(GUI.skin.button);
        style.normal.textColor = Color.gray;
        style.hover.textColor = Color.gray;
        style.hover.background = style.normal.background;
        style.onHover.background = style.normal.background;
        style.onHover.textColor = Color.green;
        style.onNormal.textColor = Color.green;
        style.onNormal.background = style.normal.background;
        float width = 70;
        return GUILayout.Toggle(toggleState, text, style, GUILayout.Width(width));
    }
}