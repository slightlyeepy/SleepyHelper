using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/StClimbDoesntKillSpeedTrigger")]
	[Tracked]
	public class StClimbDoesntKillSpeedTrigger : Trigger {
		private readonly bool useRetained;

		private bool playerInside = false;

		public StClimbDoesntKillSpeedTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			useRetained = data.Bool("useRetained");
		}

		public static void Load() {
			On.Celeste.Player.ClimbBegin += climbBegin;
		}

		public static void Unload() {
			On.Celeste.Player.ClimbBegin -= climbBegin;
		}

		private static void climbBegin(On.Celeste.Player.orig_ClimbBegin orig, Player player) {
			StClimbDoesntKillSpeedTrigger trigger = player.level.Tracker.GetEntities<StClimbDoesntKillSpeedTrigger>().OfType<StClimbDoesntKillSpeedTrigger>().FirstOrDefault(t => (t.playerInside));
			if (trigger != null) {
				float speedX;
				if (trigger.useRetained && /* player.wallSpeedRetentionTimer > 0f && */ player.Speed.X == 0) {
					speedX = player.wallSpeedRetained;
				} else {
					speedX = player.Speed.X;
				}
				orig(player);
				player.Speed.X = speedX;
			} else {
				orig(player);
			}
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			playerInside = true;
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			playerInside = false;
		}
	}
}
