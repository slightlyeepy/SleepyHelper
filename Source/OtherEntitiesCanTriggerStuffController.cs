using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Celeste.Mod.MaxHelpingHand.Entities;

namespace Celeste.Mod.SleepyHelper {
	[Tracked(false)]
	public class TriggerGenericCollider : Component {
		public Collider Collider;

		public TriggerGenericCollider(Collider collider = null) : base(active: false, visible: false) {
			Collider = null;
		}

		public void Check(Entity entity, HashSet<string> whitelist) {
			if (whitelist.Contains(entity.GetType().FullName) || whitelist.Contains(entity.GetType().Name)) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;

				Trigger trigger = base.Entity as Trigger;
				Player player = Engine.Scene.Tracker.GetEntity<Player>();
				if (entity.CollideCheck(trigger)) {
					if (!trigger.Triggered) {
						trigger.Triggered = true;
						trigger.OnEnter(player);
					}
					trigger.OnStay(player);
				} else if (trigger.Triggered) {
					trigger.Triggered = false;
					trigger.OnLeave(player);
				}

				base.Entity.Collider = collider;
			}
		}
	}

	[Tracked(false)]
	public class TouchSwitchGenericCollider : Component {
		public Collider Collider;

		public TouchSwitchGenericCollider(Collider collider = null) : base(active: false, visible: false) {
			Collider = null;
		}

		public void Check(Entity entity, HashSet<string> whitelist) {
			if (whitelist.Contains(entity.GetType().FullName) || whitelist.Contains(entity.GetType().Name)) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;
				if (entity.CollideCheck(base.Entity))
					(base.Entity as TouchSwitch).TurnOn();
				base.Entity.Collider = collider;
			}
		}
	}

	[Tracked(false)]
	public class FlagTouchSwitchGenericCollider : Component {
		public Collider Collider;

		public FlagTouchSwitchGenericCollider(Collider collider = null) : base(active: false, visible: false) {
			Collider = null;
		}

		public void Check(Entity entity, HashSet<string> whitelist) {
			if (whitelist.Contains(entity.GetType().FullName) || whitelist.Contains(entity.GetType().Name)) {
				Collider collider = base.Entity.Collider;
				if (Collider != null)
					base.Entity.Collider = Collider;
				if (entity.CollideCheck(base.Entity))
					(base.Entity as FlagTouchSwitch).TurnOn();
				base.Entity.Collider = collider;
			}
		}
	}

	[CustomEntity("SleepyHelper/OtherEntitiesCanTriggerStuffController")]
	[Tracked]
	public class OtherEntitiesCanTriggerStuffController : Entity {
		private readonly HashSet<string> whitelist;

		public static bool HooksLoaded = false;

		private static Hook touchSwitchCtorHook;
		private static Hook flagTouchSwitchSetUpCollisionHook;

		public OtherEntitiesCanTriggerStuffController(EntityData data, Vector2 offset) : base(data.Position + offset) {
			// load hooks in ctor instead of Awake() because, uhh, idk it doesnt work if i load them in Awake()
			if (!HooksLoaded)
				Load();

			whitelist = new HashSet<string>(data.Attr("whitelist").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()));
		}

		public static void Load() {
			On.Monocle.Entity.Update += entityUpdate;
			On.Celeste.Trigger.ctor += triggerCtor;
			// for SOME reason "'TouchSwitch' does not contain a definition for 'ctor'" so hook it manually
			// On.Celeste.TouchSwitch.ctor += touchSwitchCtor;

			touchSwitchCtorHook = new Hook(typeof(TouchSwitch).GetConstructor(new [] { typeof(Vector2) }), typeof(OtherEntitiesCanTriggerStuffController).GetMethod("touchSwitchCtor", BindingFlags.NonPublic | BindingFlags.Static));
			flagTouchSwitchSetUpCollisionHook = new Hook(typeof(FlagTouchSwitch).GetMethod("setUpCollision", BindingFlags.NonPublic | BindingFlags.Instance), typeof(OtherEntitiesCanTriggerStuffController).GetMethod("flagTouchSwitchSetUpCollision", BindingFlags.NonPublic | BindingFlags.Static));

			HooksLoaded = true;
		}

		public static void Unload() {
			On.Monocle.Entity.Update -= entityUpdate;
			On.Celeste.Trigger.ctor -= triggerCtor;
			// On.Celeste.TouchSwitch.ctor -= touchSwitchCtor;

			touchSwitchCtorHook?.Dispose();
			touchSwitchCtorHook = null;
			flagTouchSwitchSetUpCollisionHook?.Dispose();
			flagTouchSwitchSetUpCollisionHook = null;

			HooksLoaded = false;
		}

		private static void entityUpdate(On.Monocle.Entity.orig_Update orig, Entity self) {
			orig(self);

			OtherEntitiesCanTriggerStuffController controller = getController();
			if (controller != null) {
				foreach (TriggerGenericCollider component in self.Scene.Tracker.GetComponents<TriggerGenericCollider>())
					component.Check(self, controller.whitelist);
				foreach (TouchSwitchGenericCollider component in self.Scene.Tracker.GetComponents<TouchSwitchGenericCollider>())
					component.Check(self, controller.whitelist);
				foreach (FlagTouchSwitchGenericCollider component in self.Scene.Tracker.GetComponents<FlagTouchSwitchGenericCollider>())
					component.Check(self, controller.whitelist);
			}
		}

		private static void triggerCtor(On.Celeste.Trigger.orig_ctor orig, Trigger self, EntityData data, Vector2 offset) {
			orig(self, data, offset);
			self.Add(new TriggerGenericCollider(new Hitbox(24f, 24f, -12f, -12f)));
		}

		private static void touchSwitchCtor(Action<TouchSwitch, Vector2> orig, TouchSwitch self, Vector2 position) {
			orig(self, position);
			self.Add(new TouchSwitchGenericCollider(new Hitbox(24f, 24f, -12f, -12f)));
		}

		private static void flagTouchSwitchSetUpCollision(Action<FlagTouchSwitch, EntityData> orig, FlagTouchSwitch self, EntityData data) {
			orig(self, data);
			self.Add(new FlagTouchSwitchGenericCollider(new Hitbox(24f, 24f, -12f, -12f)));
		}

		private static OtherEntitiesCanTriggerStuffController getController() {
			// return null if the type isn't tracked yet. this can happen when the mod is being loaded during runtime
			if (Engine.Scene == null || !Engine.Scene.Tracker.Entities.ContainsKey(typeof(OtherEntitiesCanTriggerStuffController)))
				return null;

			return Engine.Scene.Tracker.GetEntity<OtherEntitiesCanTriggerStuffController>();
		}
	}
}
