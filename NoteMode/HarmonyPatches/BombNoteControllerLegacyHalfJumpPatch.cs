using HarmonyLib;
using NoteMode.Configuration;

namespace NoteMode.HarmonyPatches
{
    /// <summary>
    /// バニラ <see cref="BombNoteController"/> はジャンプ半分通過で <c>canBeCut=false</c> にする。
    /// レガシーボム ON 時はその無効化をスキップする。
    /// </summary>
    [HarmonyPatch(typeof(BombNoteController), "HandleDidPassHalfJump")]
    internal static class BombNoteControllerSkipHalfJumpDisableCut
    {
        private static bool Prefix()
        {
            if (PluginConfig.Instance == null || !PluginConfig.Instance.legacyBombBladeHitbox)
            {
                return true;
            }

            return false;
        }
    }
}
