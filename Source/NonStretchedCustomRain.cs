using ChroniaHelper.Effects;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SleepyHelper {
	public class NonStretchedCustomRain : ChroniaHelper.Effects.CustomRain {
		public NonStretchedCustomRain(Vector2 scroll, float angle, float angleDiff, float speedMult, float scaleMult, int count, string colors, float alpha) : base(scroll, angle, angleDiff, speedMult, count, colors, alpha) {
			float rand;
			Vector2 scale;

			for (int i = 0; i < count; i++) {
				// scuffed workaround but whatever lmfao
				rand = Calc.Random.Range(200f, 600f);
				scale = Calc.AngleToVector(particles[i].Rotation, rand * scaleMult);
				particles[i].Speed = Calc.AngleToVector(particles[i].Rotation, rand * speedMult);
				particles[i].Scale.X = 4f + (scale.Length() - 200f) / 33.33333f;
			}
		}
	}
}
