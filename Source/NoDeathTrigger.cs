using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/NoDeathTrigger")]
	[Tracked]
	public class NoDeathTrigger : Trigger {
		private bool playerInside = false;

		public NoDeathTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public static void Load() {
			On.Celeste.Player.Die += maybeDie;
		}

		public static void Unload() {
			On.Celeste.Player.Die -= maybeDie;
		}

		private static PlayerDeadBody maybeDie(On.Celeste.Player.orig_Die orig, Player player, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true) {
			NoDeathTrigger trigger = player.level.Tracker.GetEntities<NoDeathTrigger>().OfType<NoDeathTrigger>().FirstOrDefault(t => (t.playerInside));
			if (trigger != null) {
				return null;
			}
			return orig(player, direction, evenIfInvincible, registerDeathInStats);
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			playerInside = true;
		}

		public override void OnStay(Player player) {
			base.OnStay(player);
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			playerInside = false;
		}
	}
}
