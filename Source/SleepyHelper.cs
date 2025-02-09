namespace Celeste.Mod.SleepyHelper;

public class SleepyHelper : EverestModule {
	public static SleepyHelper Instance { get; private set; }

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
		FlagToggleColorGradeFadeTrigger.Load();
		SetStReflectionFallDelayTrigger.Load();
	}

	public override void Unload() {
		FlagToggleColorGradeFadeTrigger.Unload();
		SetStReflectionFallDelayTrigger.Unload();
	}
}
