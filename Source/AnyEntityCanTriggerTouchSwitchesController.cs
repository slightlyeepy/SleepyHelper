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
	public class FlagTouchSwitchGenericCollider : Component {
		public Action<Entity, FlagTouchSwitch> OnCollide;
		public FlagTouchSwitch TouchSwitch;
		public Collider Collider;

		public FlagTouchSwitchGenericCollider(Action<Entity, FlagTouchSwitch> onCollide, FlagTouchSwitch touchSwitch, Collider collider = null) : base(active: false, visible: false) {
			OnCollide = onCollide;
			TouchSwitch = touchSwitch;
			Collider = null;
		}

		public void Check(Entity entity) {
			if (OnCollide != null) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;
				if (entity.CollideCheck(base.Entity))
					OnCollide(entity, TouchSwitch);
				base.Entity.Collider = collider;
			}
		}
	}

	[CustomEntity("SleepyHelper/AnyEntityCanTriggerTouchSwitchesController")]
	[Tracked]
	public class AnyEntityCanTriggerTouchSwitchesController : Entity {
		public static bool HooksLoaded = false;

		private static Hook setUpCollisionHook;

		public AnyEntityCanTriggerTouchSwitchesController(EntityData data, Vector2 offset) : base(data.Position + offset) {
			// load hooks in ctor instead of Awake() because, uhh, idk it doesnt work if i load them in Awake()
			if (!HooksLoaded)
				Load();
		}

		public static void Load() {
			On.Monocle.Entity.Update += entityUpdate;

			setUpCollisionHook = new Hook(typeof(FlagTouchSwitch).GetMethod("setUpCollision", BindingFlags.NonPublic | BindingFlags.Instance), typeof(AnyEntityCanTriggerTouchSwitchesController).GetMethod("setUpCollision", BindingFlags.NonPublic | BindingFlags.Static));

			HooksLoaded = true;
		}

		public static void Unload() {
			On.Monocle.Entity.Update -= entityUpdate;

			setUpCollisionHook?.Dispose();
			setUpCollisionHook = null;

			HooksLoaded = false;
		}

		private static void entityUpdate(On.Monocle.Entity.orig_Update orig, Entity self) {
			orig(self);
			foreach (FlagTouchSwitchGenericCollider component in self.Scene.Tracker.GetComponents<FlagTouchSwitchGenericCollider>()) {
				AnyEntityCanTriggerTouchSwitchesController controller = getController();
				if (controller != null)
					component.Check(self);
			}
		}

		private static void setUpCollision(Action<FlagTouchSwitch, EntityData> orig, FlagTouchSwitch self, EntityData data) {
			self.Add(new FlagTouchSwitchGenericCollider(onEntity, self, new Hitbox(24f, 24f, -12f, -12f)));
			orig(self, data);
		}

		private static void onEntity(Entity entity, FlagTouchSwitch self) {
			self.TurnOn();
		}

		private static AnyEntityCanTriggerTouchSwitchesController getController() {
			// return null if the type isn't tracked yet. This can happen when the mod is being loaded during runtime
			if (Engine.Scene == null || !Engine.Scene.Tracker.Entities.ContainsKey(typeof(AnyEntityCanTriggerTouchSwitchesController)))
				return null;

			return Engine.Scene.Tracker.GetEntity<AnyEntityCanTriggerTouchSwitchesController>();
		}
	}
}
