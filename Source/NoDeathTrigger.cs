using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/NoDeathTrigger")]
	[Tracked]
	public class NoDeathTrigger : Trigger {
		public static bool HooksLoaded = false;

		private bool playerInside = false;

		public NoDeathTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public override void Awake(Scene scene) {
			base.Awake(scene);

			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Celeste.Player.Die += maybeDie;
			HooksLoaded = true;
		}

		public static void Unload() {
			On.Celeste.Player.Die -= maybeDie;
			HooksLoaded = false;
		}

		private static PlayerDeadBody maybeDie(On.Celeste.Player.orig_Die orig, Player player, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true) {
			NoDeathTrigger trigger = player.level.Tracker.GetEntities<NoDeathTrigger>().OfType<NoDeathTrigger>().FirstOrDefault(t => (t.playerInside));
			return (trigger != null) ? null : orig(player, direction, evenIfInvincible, registerDeathInStats);
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
