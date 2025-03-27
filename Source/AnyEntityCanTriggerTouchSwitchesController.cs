using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;

using Celeste.Mod.MaxHelpingHand.Entities;

namespace Celeste.Mod.SleepyHelper {
	[Tracked(false)]
	public class TouchSwitchGenericCollider : Component {
		public Action<Entity, TouchSwitch> OnCollide;
		public TouchSwitch TouchSwitch;
		public Collider Collider;

		public TouchSwitchGenericCollider(Action<Entity, TouchSwitch> onCollide, TouchSwitch touchSwitch, Collider collider = null) : base(active: false, visible: false) {
			OnCollide = onCollide;
			TouchSwitch = touchSwitch;
			Collider = null;
		}

		public void Check(Entity entity) {
			if (OnCollide != null && entity is not SolidTiles && entity is not BackgroundTiles && entity is not Trigger) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;
				if (entity.CollideCheck(base.Entity))
					OnCollide(entity, TouchSwitch);
				base.Entity.Collider = collider;
			}
		}
	}

	[Tracked(false)]
	public class FlagTouchSwitchGenericCollider : Component {
		public Action<Entity, FlagTouchSwitch> OnCollide;
		public FlagTouchSwitch FlagTouchSwitch;
		public Collider Collider;

		public FlagTouchSwitchGenericCollider(Action<Entity, FlagTouchSwitch> onCollide, FlagTouchSwitch flagTouchSwitch, Collider collider = null) : base(active: false, visible: false) {
			OnCollide = onCollide;
			FlagTouchSwitch = flagTouchSwitch;
			Collider = null;
		}

		public void Check(Entity entity) {
			if (OnCollide != null && entity is not SolidTiles && entity is not BackgroundTiles && entity is not Trigger) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;
				if (entity.CollideCheck(base.Entity))
					OnCollide(entity, FlagTouchSwitch);
				base.Entity.Collider = collider;
			}
		}
	}

	[CustomEntity("SleepyHelper/AnyEntityCanTriggerTouchSwitchesController")]
	[Tracked]
	public class AnyEntityCanTriggerTouchSwitchesController : Entity {
		public static bool HooksLoaded = false;

		private static Hook touchSwitchCtorHook;
		private static Hook flagTouchSwitchSetUpCollisionHook;

		public AnyEntityCanTriggerTouchSwitchesController(EntityData data, Vector2 offset) : base(data.Position + offset) {
			// load hooks in ctor instead of Awake() because, uhh, idk it doesnt work if i load them in Awake()
			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Monocle.Entity.Update += entityUpdate;
			// for SOME reason "'TouchSwitch' does not contain a definition for 'ctor'" so hook it manually
			// On.Celeste.TouchSwitch.ctor += touchSwitchCtor;

			touchSwitchCtorHook = new Hook(typeof(TouchSwitch).GetConstructor(new [] { typeof(Vector2) }), typeof(AnyEntityCanTriggerTouchSwitchesController).GetMethod("touchSwitchCtor", BindingFlags.NonPublic | BindingFlags.Static));
			flagTouchSwitchSetUpCollisionHook = new Hook(typeof(FlagTouchSwitch).GetMethod("setUpCollision", BindingFlags.NonPublic | BindingFlags.Instance), typeof(AnyEntityCanTriggerTouchSwitchesController).GetMethod("flagTouchSwitchSetUpCollision", BindingFlags.NonPublic | BindingFlags.Static));

			HooksLoaded = true;
		}

		public static void Unload() {
			On.Monocle.Entity.Update -= entityUpdate;
			// On.Celeste.TouchSwitch.ctor -= touchSwitchCtor;

			touchSwitchCtorHook?.Dispose();
			touchSwitchCtorHook = null;
			flagTouchSwitchSetUpCollisionHook?.Dispose();
			flagTouchSwitchSetUpCollisionHook = null;

			HooksLoaded = false;
		}

		private static void entityUpdate(On.Monocle.Entity.orig_Update orig, Entity self) {
			orig(self);

			AnyEntityCanTriggerTouchSwitchesController controller = getController();
			if (controller != null) {
				foreach (TouchSwitchGenericCollider component in self.Scene.Tracker.GetComponents<TouchSwitchGenericCollider>())
					component.Check(self);
				foreach (FlagTouchSwitchGenericCollider component in self.Scene.Tracker.GetComponents<FlagTouchSwitchGenericCollider>())
					component.Check(self);
			}
		}

		private static void touchSwitchCtor(Action<TouchSwitch, Vector2> orig, TouchSwitch self, Vector2 position) {
			orig(self, position);
			self.Add(new TouchSwitchGenericCollider(onTouchSwitch, self, new Hitbox(24f, 24f, -12f, -12f)));
		}

		private static void flagTouchSwitchSetUpCollision(Action<FlagTouchSwitch, EntityData> orig, FlagTouchSwitch self, EntityData data) {
			orig(self, data);
			self.Add(new FlagTouchSwitchGenericCollider(onFlagTouchSwitch, self, new Hitbox(24f, 24f, -12f, -12f)));
		}

		private static void onTouchSwitch(Entity entity, TouchSwitch self) {
			self.TurnOn();
		}

		private static void onFlagTouchSwitch(Entity entity, FlagTouchSwitch self) {
			self.TurnOn();
		}

		private static AnyEntityCanTriggerTouchSwitchesController getController() {
			// return null if the type isn't tracked yet. this can happen when the mod is being loaded during runtime
			if (Engine.Scene == null || !Engine.Scene.Tracker.Entities.ContainsKey(typeof(AnyEntityCanTriggerTouchSwitchesController)))
				return null;

			return Engine.Scene.Tracker.GetEntity<AnyEntityCanTriggerTouchSwitchesController>();
		}
	}
}
