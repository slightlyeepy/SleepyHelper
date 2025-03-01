using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/SetTimeRateTrigger")]
	public class SetTimeRateTrigger : Trigger {
		private readonly float timeRate;

		public SetTimeRateTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			timeRate = data.Float("timeRate");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			Engine.TimeRate = timeRate;
		}
	}
}
