using IPA.Config.Stores;
using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoteMode.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public static bool flagNoRedBlue = false;
        public static bool flagNoRed = false;

        public virtual bool noRed { get; set; } = false;
        public virtual bool noBlue { get; set; } = false;
        public virtual bool oneColorRed { get; set; } = false;
        public virtual bool oneColorBlue { get; set; } = false;
        public virtual bool noArrow { get; set; } = false;

        public event Action<PluginConfig> ConfigChangedEvent;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            ConfigChangedEvent?.Invoke(this);
            // Do stuff when the config is changed.
            /*if (!flagNoRedBlue && (noRed || noBlue))
            {
                if (oneColorRed || oneColorBlue)
                {
                    flagNoRedBlue = true;
                    noRed = false;
                    noBlue = false;
                }
                if (!flagNoRed && noRed)
                {
                    flagNoRed = true;
                    noBlue = false;
                }
                else if (flagNoRed && noBlue)
                {
                    flagNoRed = false;
                    noRed = false;
                }
            }
            else if (flagNoRedBlue && (oneColorRed || oneColorBlue))
            {
                if (noRed || noBlue)
                {
                    flagNoRedBlue = false;
                    oneColorRed = false;
                    oneColorBlue = false;
                }
            }*/
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
            var props = other.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == nameof(Instance))
                {
                    continue;
                }
                var currentProp = this.GetType().GetProperty(prop.Name);
                if (currentProp == null)
                {
                    continue;
                }
                Logger.log?.Debug($"NoteMode: {currentProp}");
            }
        }
    }
}
