using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FreezePlayerIfInStDummyTrigger")]
	public class FreezePlayerIfInStDummyTrigger : Trigger {
		public FreezePlayerIfInStDummyTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
			}
		}

		public override void OnStay(Player player) {
			base.OnStay(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
			}
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);
		}
	}
}
