using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FreezePlayerTrigger")]
	public class FreezePlayerTrigger : Trigger {
		private readonly float freezeTime;

		private float timer;
		private int oldState;
		private Vector2 oldSpeed;

		public FreezePlayerTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			freezeTime = data.Float("freezeTime");
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);
			timer = freezeTime;
			oldState = player.StateMachine.State;
			oldSpeed = player.Speed;
			player.StateMachine.State = 17;
			player.Speed = Vector2.Zero;
		}

		public override void OnStay(Player player) {
			base.OnStay(player);
			if (timer >= 0f) {
				timer -= Engine.DeltaTime;
			} else {
				player.StateMachine.State = oldState;
				player.Speed = oldSpeed;
			}
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);
			player.StateMachine.State = oldState;
			player.Speed = oldSpeed;
		}
	}
}
