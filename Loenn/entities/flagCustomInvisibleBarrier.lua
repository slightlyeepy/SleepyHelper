local celesteEnums = require("consts.celeste_enums")
local soundids = celesteEnums.tileset_sound_ids

return {
	name = "SleepyHelper/FlagCustomInvisibleBarrier",
	fillColor = {0.4, 0.4, 0.4, 0.8},
	borderColor = {0.4, 0.4, 0.4, 0.8},
	placements = {
		name = "entity",
		data = {
			width = 8,
			height = 8,
			soundIndex = 33,
			canClimb = false,
			flag = "flag_toggle_invisible_barrier",
			inverted = false
		}
	},
	fieldInformation = {
		soundIndex = {
			options = soundids
		}
	}
}
