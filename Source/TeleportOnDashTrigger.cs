using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/TeleportOnDashTrigger")]
	public class TeleportOnDashTrigger : Trigger {
		private readonly Vector2 targetPos;

		private readonly float freezeDuration;
		private readonly string flagOnTeleport;

		public TeleportOnDashTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			if (data.Nodes.Length != 1)
				throw new IndexOutOfRangeException("SleepyHelper/TeleportOnDashTrigger: must have exactly 1 node!");
			targetPos = data.Nodes[0] + offset;

			freezeDuration = data.Float("freezeDuration");
			flagOnTeleport = data.Attr("flagOnTeleport");
		}

		public override void OnStay(Player player) {
			base.OnStay(player);

			if (Input.Dash.Pressed) {
				if (!String.IsNullOrWhiteSpace(flagOnTeleport))
					player.level.Session.SetFlag(flagOnTeleport, true);
				if (freezeDuration > 0f)
					Celeste.Freeze(freezeDuration);
				player.Position = targetPos;
			}
		}
	}
}
