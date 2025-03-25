using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

using Celeste.Mod.MaxHelpingHand.Entities;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/TurnOnFlagTouchSwitchesTrigger")]
	public class TurnOnFlagTouchSwitchesTrigger : Trigger {
		private readonly string flagToTurnOn;

		public TurnOnFlagTouchSwitchesTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			flagToTurnOn = data.Attr("flagToTurnOn");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			foreach (FlagTouchSwitch touchSwitch in player.level.Tracker.GetEntities<FlagTouchSwitch>())
				if (touchSwitch.Flag == flagToTurnOn)
					touchSwitch.TurnOn();
		}
	}
}
