local trigger = {}

trigger.name = "SleepyHelper/ForceInputsTrigger"
trigger.triggerText = "Force Inputs"
trigger.placements = {
	name = "trigger",
	data = {
		inputs = "",
		showTooltip = false,
		hideTooltipInCutscenes = false
	}
}

return trigger
