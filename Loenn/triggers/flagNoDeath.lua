local trigger = {}

trigger.name = "SleepyHelper/FlagNoDeathTrigger"
trigger.triggerText = "Flag No Death"
trigger.placements = {
	name = "trigger",
	data = {
		flag = "flag_toggle_no_death",
		inverted = false
	}
}

return trigger
