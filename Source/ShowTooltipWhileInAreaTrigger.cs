using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/ShowTooltipWhileInAreaTrigger")]
	public class ShowTooltipWhileInAreaTrigger : Trigger {
		private readonly string tooltipText;

		private Tooltip tooltip;

		public ShowTooltipWhileInAreaTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			tooltipText = data.Attr("tooltipText");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			tooltip = new Tooltip(tooltipText);
			player.level.Add(tooltip);
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			tooltip.RemoveSelf();
			tooltip = null;
		}
	}
}
