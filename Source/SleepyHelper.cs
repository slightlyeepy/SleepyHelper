using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.SleepyHelper;

public class SleepyHelper : EverestModule {
	public static SleepyHelper Instance { get; private set; }

	public static bool ChroniaHelperLoaded;

	public SleepyHelper() {
		Instance = this;
#if DEBUG
		// debug builds use verbose logging
		Logger.SetLogLevel(nameof(SleepyHelper), LogLevel.Verbose);
#else
		// release builds use info logging to reduce spam in log files
		Logger.SetLogLevel(nameof(SleepyHelper), LogLevel.Info);
#endif
	}

	public override void Load() {
		EverestModuleMetadata chroniaHelper = new () {
			Name = "ChroniaHelper",
			Version = new Version(1, 27, 0)
		};

		ChroniaHelperLoaded = Everest.Loader.DependencyLoaded(chroniaHelper);

		if (ChroniaHelperLoaded) {
			Everest.Events.Level.OnLoadBackdrop += levelOnLoadBackdrop;
		} else {
			Logger.Log(LogLevel.Info, "SleepyHelper", "ChroniaHelper 1.27.0+ not found -- Non-Stretched Custom Rain will not work.");
		}

		NoDeathTrigger.Load();
		SetStReflectionFallDelayTrigger.Load();
		StClimbDoesntKillSpeedTrigger.Load();

		FlagNoDeathTrigger.Load();
		FlagToggleColorGradeFadeTrigger.Load();
	}

	public override void Unload() {
		if (ChroniaHelperLoaded) {
			Everest.Events.Level.OnLoadBackdrop -= levelOnLoadBackdrop;
		}

		NoDeathTrigger.Unload();
		SetStReflectionFallDelayTrigger.Unload();
		StClimbDoesntKillSpeedTrigger.Unload();

		FlagNoDeathTrigger.Unload();
		FlagToggleColorGradeFadeTrigger.Unload();
	}

	private Backdrop levelOnLoadBackdrop(MapData map, BinaryPacker.Element child, BinaryPacker.Element above) {
		if (child.Name.Equals("SleepyHelper/NonStretchedCustomRain", StringComparison.OrdinalIgnoreCase)) {
			return new NonStretchedCustomRain(new Vector2(child.AttrFloat("scrollx", 0f), child.AttrFloat("scrolly", 0f)), child.AttrFloat("angle", 270f), child.AttrFloat("angleDiff", 3f), child.AttrFloat("speedMult", 1f), child.AttrFloat("scaleMult", 1f), Extensions.AttrInt(child, "amount", 240), child.Attr("colors", "161933"), child.AttrFloat("alpha", 0f));
		}
		return null;
	}
}
