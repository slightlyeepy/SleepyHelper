// This file uses code from Maddie's Helping Hand -- see LICENSE.MaddiesHelpingHand file.

using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.SleepyHelper {
	[CustomEntity("SleepyHelper/ForceInputsTrigger")]
	[Tracked]
	public class ForceInputsTrigger : Trigger {
		private static Hook buttonCheckHook;
		private static Hook buttonPressedHook;
		private static Hook buttonReleasedHook;
		private static Hook grabCheckHook;
		private static Hook dashPressedHook;
		private static Hook crouchDashPressedHook;

		private readonly string inputstring;
		private readonly string[] inputs;
		private readonly bool showTooltip;
		private readonly bool hideTooltipInCutscenes;
		private Tooltip tooltip;

		private bool playerInside = false;
		private bool playerJustEntered = false;
		private bool playerLeft = false;

		public ForceInputsTrigger(EntityData data, Vector2 offset) : base(data, offset) {
			inputstring = data.Attr("inputs");
			showTooltip = data.Bool("showTooltip");
			hideTooltipInCutscenes = data.Bool("hideTooltipInCutscenes");

			inputs = inputstring.ToLower().Split(',', StringSplitOptions.RemoveEmptyEntries);
			validateInputs();
		}

		private void validateInputs() {
			List<char> seen = new List<char>();
			bool seenH = false;
			bool seenV = false;
			int i;
			foreach (string input in inputs) {
				i = (input[0] == '!') ? 1 : 0;
				if (seen.Contains(input[i])) {
					throw new Exception("Force Inputs Trigger: invalid inputs - can't have same input twice, or same input force pressed and force released!");
				}
				seen.Add(input[i]);
				switch (input[i]) {
				case 'l':
				case 'r':
					if (seenH && input[0] != '!') {
						throw new Exception("Force Inputs Trigger: invalid inputs - can't force both left & right!");
					}
					seenH = true;
					break;
				case 'u':
				case 'd':
					if (seenV && input[0] != '!') {
						throw new Exception("Force Inputs Trigger: invalid inputs - can't force both up & down!");
					}
					seenV = true;
					break;
				case 'j':
				case 'x':
				case 'z':
				case 'g':
					break;
				default:
					throw new Exception($"Force Inputs Trigger: invalid inputs - unknown input '{input[i]}'!");
				}
			}
		}

		public static void Load() {
			On.Celeste.Player.Update += playerUpdate;

			buttonCheckHook = new Hook(typeof(VirtualButton).GetMethod("get_Check"), typeof(ForceInputsTrigger).GetMethod("onButtonCheck", BindingFlags.NonPublic | BindingFlags.Static));
			buttonPressedHook = new Hook(typeof(VirtualButton).GetMethod("get_Pressed"), typeof(ForceInputsTrigger).GetMethod("onButtonPressed", BindingFlags.NonPublic | BindingFlags.Static));
			buttonReleasedHook = new Hook(typeof(VirtualButton).GetMethod("get_Released"), typeof(ForceInputsTrigger).GetMethod("onButtonReleased", BindingFlags.NonPublic | BindingFlags.Static));
			grabCheckHook = new Hook(typeof(Input).GetMethod("get_GrabCheck"), typeof(ForceInputsTrigger).GetMethod("modGrabResult", BindingFlags.NonPublic | BindingFlags.Static));
			dashPressedHook = new Hook(typeof(Input).GetMethod("get_DashPressed"), typeof(ForceInputsTrigger).GetMethod("modDashResult", BindingFlags.NonPublic | BindingFlags.Static));
			crouchDashPressedHook = new Hook(typeof(Input).GetMethod("get_CrouchDashPressed"), typeof(ForceInputsTrigger).GetMethod("modCrouchDashResult", BindingFlags.NonPublic | BindingFlags.Static));
		}

		public static void Unload() {
			On.Celeste.Player.Update -= playerUpdate;

			buttonCheckHook?.Dispose();
			buttonPressedHook?.Dispose();
			buttonReleasedHook?.Dispose();
			grabCheckHook?.Dispose();
			dashPressedHook?.Dispose();
			crouchDashPressedHook?.Dispose();

			buttonCheckHook = null;
			buttonPressedHook = null;
			buttonReleasedHook = null;
			grabCheckHook = null;
			dashPressedHook = null;
			crouchDashPressedHook = null;
		}

		private static void playerUpdate(On.Celeste.Player.orig_Update orig, Player self) {
			ForceInputsTrigger trigger = self.level.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			if (trigger != null) {
				if (trigger.showTooltip && trigger.hideTooltipInCutscenes && trigger.tooltip != null) {
					trigger.tooltip.ShouldRender = (self.StateMachine.State != Player.StDummy);
				}

				Vector2 oldAim = Input.Aim;
				int oldMoveX = Input.MoveX.Value;
				int oldMoveY = Input.MoveY.Value;
				int oldGliderMoveY = Input.GliderMoveY.Value;

				Vector2 newAim = Input.Aim;
				int newMoveX = Input.MoveX.Value;
				int newMoveY = Input.MoveY.Value;
				int newGliderMoveY = Input.GliderMoveY.Value;

				int i;
				foreach (string input in trigger.inputs) {
					i = (input[0] == '!') ? 1 : 0;
					switch (input[i]) {
					case 'l':
						if (input[0] != '!') {
							// force press left
							newAim.X = -1.0f;
							newMoveX = -1;
						} else {
							// force release left
							newAim.X = Math.Max(0, newAim.X);
							newMoveX = Math.Max(0, newMoveX);
						}
						break;
					case 'r':
						if (input[0] != '!') {
							// force press right
							newAim.X = 1.0f;
							newMoveX = 1;
						} else {
							// force release right
							newAim.X = Math.Min(0, newAim.X);
							newMoveX = Math.Min(0, newMoveX);
						}
						break;
					case 'u':
						if (input[0] != '!') {
							// force press up
							newAim.Y = -1.0f;
							newMoveY = -1;
							newGliderMoveY = -1;

						} else {
							// force release up
							newAim.Y = Math.Max(0, newAim.Y);
							newMoveY = Math.Max(0, newMoveY);
							newGliderMoveY = Math.Max(0, newGliderMoveY);
						}
						break;
					case 'd':
						if (input[0] != '!') {
							// force press down
							newAim.Y = 1.0f;
							newMoveY = 1;
							newGliderMoveY = 1;
						} else {
							// force release down
							newAim.Y = Math.Min(0, newAim.Y);
							newMoveY = Math.Min(0, newMoveY);
							newGliderMoveY = Math.Min(0, newGliderMoveY);
						}
						break;
					}
				}

				new DynData<VirtualJoystick>(Input.Aim)["Value"] = newAim;
				Input.MoveX.Value = newMoveX;
				Input.MoveY.Value = newMoveY;
				Input.GliderMoveY.Value = newGliderMoveY;

				orig(self);

				new DynData<VirtualJoystick>(Input.Aim)["Value"] = oldAim;
				Input.MoveX.Value = oldMoveX;
				Input.MoveY.Value = oldMoveY;
				Input.GliderMoveY.Value = oldGliderMoveY;
			} else {
				orig(self);
			}
		}

		private static bool? checkButtonOverride(string[] inputs, VirtualButton button) {
			int i;
			foreach (string input in inputs) {
				i = (input[0] == '!') ? 1 : 0;
				switch (input[i]) {
				case 'j':
					if (button == Input.Jump) {
						return (input[0] != '!');
					}
					break;
				case 'x':
					if (button == Input.Dash) {
						return (input[0] != '!');
					}
					break;
				case 'z':
					if (button == Input.CrouchDash) {
						return (input[0] != '!');
					}
					break;
				case 'g':
					if (button == Input.Grab) {
						return (input[0] != '!');
					}
					break;
				}
			}
			return null;
		}

		private static bool? checkInputOverride(string[] inputs, char input) {
			if (inputs.Contains(input.ToString())) {
				return true;
			} else if (inputs.Contains("!" + input)) {
				return false;
			}
			return null;
		}

		private static bool onButtonCheck(Func<VirtualButton, bool> orig, VirtualButton self) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			if (trigger != null) {
				bool? overrideInput = checkButtonOverride(trigger.inputs, self);
				return overrideInput ?? orig(self);
			}

			return orig(self);
		}

		private static bool onButtonPressed(Func<VirtualButton, bool> orig, VirtualButton self) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			if (trigger != null) {
				bool? overrideInput = checkButtonOverride(trigger.inputs, self);
				if (overrideInput == false) {
					return false;
				} else if (overrideInput == true && trigger.playerJustEntered) {
					return true;
				}
			}

			return orig(self);
		}

		private static bool onButtonReleased(Func<VirtualButton, bool> orig, VirtualButton self) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerLeft);
			if (trigger != null) {
				trigger.playerLeft = false;
				bool? overrideInput = checkButtonOverride(trigger.inputs, self);
				return overrideInput ?? orig(self);
			}

			return orig(self);
		}

		private static bool modGrabResult(Func<bool> orig) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			return (trigger != null) ? (checkInputOverride(trigger.inputs, 'g') ?? orig()) : orig();
		}

		private static bool modDashResult(Func<bool> orig) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			return (trigger != null) ? (checkInputOverride(trigger.inputs, 'x') ?? orig()) : orig();
		}

		private static bool modCrouchDashResult(Func<bool> orig) {
			ForceInputsTrigger trigger = Engine.Scene.Tracker.GetEntities<ForceInputsTrigger>().OfType<ForceInputsTrigger>().FirstOrDefault(t => t.playerInside);
			return (trigger != null) ? (checkInputOverride(trigger.inputs, 'z') ?? orig()) : orig();
		}

		public override void OnEnter(Player player) {
			base.OnEnter(player);

			playerInside = true;
			playerJustEntered = true;

			if (showTooltip) {
				tooltip = new Tooltip($"force inputs: {inputstring}");
				player.level.Add(tooltip);
			}
		}

		public override void OnStay(Player player) {
			base.OnStay(player);

			playerJustEntered = false;
		}

		public override void OnLeave(Player player) {
			base.OnLeave(player);

			playerInside = false;
			playerLeft = true;
			if (showTooltip) {
				tooltip.RemoveSelf();
				tooltip = null;
			}
		}
	}
}
