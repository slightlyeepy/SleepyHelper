using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FreezePlayerIfInStDummyTrigger")]
	public class FreezePlayerIfInStDummyTrigger : Trigger {
		private bool preserveSpeed = false;
		private Vector2 preservedSpeed;
		private bool preservedSpeedSet = false;
		private bool preservedSpeedRestored = false;
		private bool wasInStDummy = false;

		public FreezePlayerIfInStDummyTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			preserveSpeed = data.Bool("preserveSpeed");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
				wasInStDummy = true;
			} else if (preserveSpeed) {
				preservedSpeed = player.Speed;
				preservedSpeedSet = true;
			}
		}

		public override void OnStay(Player player) {
			base.OnStay(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
				wasInStDummy = true;
			} else if (preserveSpeed) {
				if (preservedSpeedSet && !preservedSpeedRestored && wasInStDummy) {
					player.Speed = preservedSpeed;
					preservedSpeedRestored = true;
				} else if (!preservedSpeedSet) {
					preservedSpeed = player.Speed;
					preservedSpeedSet = true;
				}
			}
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);
		}
	}
}
