using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/SetStReflectionFallDelayTrigger")]
	public class SetStReflectionFallDelayTrigger : Trigger {
		public SetStReflectionFallDelayTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			SleepyHelperModule.CustomReflectionFallDelay = data.Int("delay");
		}

		public override void OnEnter(Player player) {
			SleepyHelperModule.CustomReflectionFallDelayEnabled = true;
			base.OnEnter(player);
		}

		public override void OnStay(Player player) {
			base.OnStay(player);
		}

		public override void OnLeave(Player player) {
			SleepyHelperModule.CustomReflectionFallDelayEnabled = false;
			base.OnLeave(player);
		}
	}
}
