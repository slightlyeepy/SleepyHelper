using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FlagNoDeathTrigger")]
	[Tracked]
	public class FlagNoDeathTrigger : Trigger {
		private readonly string flag;
		private readonly bool inverted;

		private bool playerInside = false;

		public FlagNoDeathTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			flag = data.Attr("flag");
			inverted = data.Bool("inverted");
		}

		public static void Load() {
			On.Celeste.Player.Die += maybeDie;
		}

		public static void Unload() {
			On.Celeste.Player.Die -= maybeDie;
		}

		private static PlayerDeadBody maybeDie(On.Celeste.Player.orig_Die orig, Player player, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true) {
			FlagNoDeathTrigger trigger = player.level.Tracker.GetEntities<FlagNoDeathTrigger>().OfType<FlagNoDeathTrigger>().FirstOrDefault(t => t.playerInside && player.level.Session.GetFlag(t.flag) != t.inverted);
			if (trigger != null) {
				return null;
			}
			return orig(player, direction, evenIfInvincible, registerDeathInStats);
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
