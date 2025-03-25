using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/FlagCustomInvisibleBarrier")]
	public class FlagCustomInvisibleBarrier : Solid {
		private readonly string flag;
		private readonly bool inverted;

		public FlagCustomInvisibleBarrier(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true) {
			flag = data.Attr("flag");
			inverted = data.Bool("inverted");

			Visible = false;

			if (!data.Bool("canClimb", false))
				Add(new ClimbBlocker(true));

			SurfaceSoundIndex = data.Int("soundIndex", 33);

			AllowStaticMovers = false;
		}

		public override void Update() {
			base.Update();

			Collidable = (SceneAs<Level>().Session.GetFlag(flag) != inverted);
		}
	}
}
