using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FreezePlayerIfInStDummyTrigger")]
	public class FreezePlayerIfInStDummyTrigger : Trigger {
		private bool PreserveSpeed = false;
		private Vector2 PreservedSpeed;
		private bool PreservedSpeedSet = false;
		private bool PreservedSpeedRestored = false;
		private bool WasInStDummy = false;

		public FreezePlayerIfInStDummyTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			PreserveSpeed = data.Bool("preserveSpeed");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
				WasInStDummy = true;
			} else if (PreserveSpeed) {
				PreservedSpeed = player.Speed;
				PreservedSpeedSet = true;
			}
		}

		public override void OnStay(Player player) {
			base.OnStay(player);

			if (player.StateMachine.State == Player.StDummy) {
				player.DummyGravity = false;
				player.Speed = Vector2.Zero;
				WasInStDummy = true;
			} else if (PreserveSpeed) {
				if (PreservedSpeedSet && !PreservedSpeedRestored && WasInStDummy) {
					player.Speed = PreservedSpeed;
					PreservedSpeedRestored = true;
				} else if (!PreservedSpeedSet) {
					PreservedSpeed = player.Speed;
					PreservedSpeedSet = true;
				}
			}
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);
		}
	}
}
