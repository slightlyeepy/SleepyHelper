// This file uses code from SpeedrunTool -- see LICENSE.SpeedrunTool file.

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System.Linq;

namespace Celeste.Mod.SleepyHelper {
	[Tracked]
	public class Tooltip : Entity {
		private const int Padding = 25;

		private readonly string message;

		public Tooltip(string message) {
			this.message = message;
			Vector2 messageSize = ActiveFont.Measure(message);
			Position = new(Padding, Engine.Height - messageSize.Y - Padding / 2f);
			Tag = Tags.HUD | Tags.Global | Tags.FrozenUpdate | Tags.PauseUpdate | Tags.TransitionUpdate;
		}

		public override void Render() {
			base.Render();

			ActiveFont.DrawOutline(message, Position, Vector2.Zero, Vector2.One, Color.White, 2, Color.Black);
		}
	}
}
