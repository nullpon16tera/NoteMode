using IPA.Loader;
using System.Linq;
using Zenject;

namespace NoteMode.Utilities
{
    public class BeatmapUtil : IInitializable
    {
        public GameplayCoreSceneSetupData Currentmap { get; private set; }
        public bool IsNoodle { get; private set; }
        public bool IsChroma { get; private set; }

        [Inject]
        public BeatmapUtil(GameplayCoreSceneSetupData level)
        {
            this.Currentmap = level;
        }

        public static bool IsNoodleMap(GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            if (PluginManager.EnabledPlugins.Any(x => x.Name == "NoodleExtensions"))
            {
                var isIsNoodleMap = SongCore.Collections.RetrieveDifficultyData(gameplayCoreSceneSetupData.beatmapLevel, gameplayCoreSceneSetupData.beatmapKey)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Noodle Extensions") == true;
                return isIsNoodleMap;
            }
            else
            {
                return false;
            }
        }

        public static bool IsChromaMap(GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            if (PluginManager.EnabledPlugins.Any(x => x.Name == "Chroma"))
            {
                var isIsNoodleMap = SongCore.Collections.RetrieveDifficultyData(gameplayCoreSceneSetupData.beatmapLevel, gameplayCoreSceneSetupData.beatmapKey)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Chroma") == true;
                isIsNoodleMap = isIsNoodleMap || SongCore.Collections.RetrieveDifficultyData(gameplayCoreSceneSetupData.beatmapLevel, gameplayCoreSceneSetupData.beatmapKey)?
                    .additionalDifficultyData?
                    ._suggestions?.Any(x => x == "Chroma") == true;
                return isIsNoodleMap;
            }
            else
            {
                return false;
            }
        }

        public void Initialize()
        {
            this.IsNoodle = IsNoodleMap(this.Currentmap);
            this.IsChroma = IsChromaMap(this.Currentmap);
        }
    }
}
