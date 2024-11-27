using HarmonyLib;
using NoteMode.Configuration;
using System.Reflection;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BasicBeatmapObjectManager), "ProcessNoteData")]
    public class BasicBeatmapObjectManagerProcessNoteData
    {
        static bool Prefix(ref NoteData noteData)
        {
            if (PluginConfig.Instance.noRed && (noteData.colorType == ColorType.ColorA))
            {
                return false;
            }
            else if (PluginConfig.Instance.noBlue && (noteData.colorType == ColorType.ColorB))
            {
                return false;
            }

            if (PluginConfig.Instance.noNotesBomb)
            {
                return false;
            }
            return true;
        }
    }

    /**
     * Wall
     */
    [HarmonyPatch(typeof(BasicBeatmapObjectManager), "ProcessObstacleData")]
    public class BasicBeatmapOjbectManagerProcessObstacleData
    {
        static void Prefix(ref ObstacleData obstacleData, ref BeatmapObjectSpawnMovementData.ObstacleSpawnData obstacleSpawnData)
        {
            if (PluginConfig.Instance.noNotesBomb)
            {
                
            }
        }
    }

    /**
    * Arc or Chain Notes
    */
    [HarmonyPatch(typeof(BasicBeatmapObjectManager), "ProcessSliderData")]
    public class BasicBeatmapOjbectManagerProcessSliderData
    {
        static bool Prefix(ref SliderData sliderData, ref BeatmapObjectSpawnMovementData.SliderSpawnData sliderSpawnData)
        {

            if (sliderData.sliderType == SliderData.Type.Burst)
            {
                //return false;
            }

            if ((sliderData.colorType == ColorType.ColorA) && PluginConfig.Instance.noRed)
            {
                return false;
            }
            else if ((sliderData.colorType == ColorType.ColorB) && PluginConfig.Instance.noBlue)
            {
                return false;
            }

            if (PluginConfig.Instance.noNotesBomb)
            {
                return false;
            }
            return true;
        }
    }
}