# sleepy helper

my first celeste mod !!

it was made to be used in my own map, but feel free to use it in yours or do anything you want with it

## list of things this helper adds

- **effects:**
- Non-Stretched Custom Rain: (ChroniaHelper 1.27.0+ required) [Chronia Helper](https://gamebanana.com/mods/507580)'s `Custom Rain`, except the size of the droplets isn't affected by `Speed Mult`, but instead a new attribute called `Scale Mult`.
- **entities:**
- Any Entity Can Trigger Touch Switches Controller: while in a room with this controller, every entity can trigger [Maddie's Helping Hand](https://gamebanana.com/mods/53687) `Flag Touch Switch`es.
- Flag Custom Invisible Barrier: similar to [Frost Helper](https://gamebanana.com/mods/53647)'s `Custom Invisible Barrier`, but flag-toggled.
- Force Grab Controller: while in a room with this controller, the grab input is force-pressed, similarly to invert grab mode with the button released.
- **triggers:**
- Disable Dialogue Confirm Trigger: while inside this trigger, pressing confirm to fast-forward dialogue is disabled.
- Force Inputs Trigger: takes a TAS-like string (e.g `R,U,X,!J` to force an up-right dash and disable jumping) and force presses/releases those buttons until you leave the trigger.
- Freeze Player If In StDummy Trigger: while inside this trigger, if you are in StDummy (for example, in a cutscene), you'll be frozen (0 horizontal & vertical speed) until you leave StDummy.
- Freeze Player Trigger: freezes the player in place for a set amount of time.
- Hard Crash Trigger: makes the game crash. not an everest crash, actually just crashing & closing the game.
- No Death Trigger: while inside this trigger, `Player.Die()` is disabled completely.
- Set StReflectionFall Delay Trigger: while inside this trigger, if you enter StReflectionFall, the amount of time before you start falling is changed from 2 seconds to a configurable delay in frames.
- Set TimeRate Trigger: allows you to set the value of `Engine.TimeRate`.
- Show Tooltip While In Area Trigger: displays some text at the bottom-left corner of the screen while you're inside this trigger. Will be more configurable later.
- StClimb Doesn't Kill Speed Trigger: while inside this trigger, if you enter StClimb, your horizontal speed won't be reset to 0. has an option to use retained speed if normal horizontal speed is 0.
- Teleport On Dash Trigger: teleports you to a set location if you dash while inside. does not cancel the dash.
- Turn On Flag Touch Switches Trigger: turns on every [Maddie's Helping Hand](https://gamebanana.com/mods/53687) `Flag Touch Switch` in the room with a specified `Flag` value.
- **flag-toggled trigger variants:**
- Flag No Death Trigger: a flag-toggled variant of the No Death Trigger.
- Flag Toggle Color Grade Fade Trigger: a flag-toggled variant of the Color Grade Fade Trigger from [Maddie's Helping Hand](https://github.com/maddie480/MaddieHelpingHand). 
