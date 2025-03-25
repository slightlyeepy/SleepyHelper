-- This file uses code from Maddie's Helping Hand -- see LICENSE.MaddiesHelpingHand file.

local celesteEnums = require("consts.celeste_enums")
local colorgrades = celesteEnums.color_grades

return {
	name = "SleepyHelper/FlagToggleColorGradeFadeTrigger",
	category = "visual",
	triggerText = "Flag Toggle Color Grade Fade",
	placements = {
		name = "trigger",
		data = {
			colorGradeA = "none",
			colorGradeB = "none",
			direction = "LeftToRight",
			flag = "flag_toggle_color_grade_fade",
			inverted = false,
			evenDuringReflectionFall = false
		}
	},
	fieldInformation = {
		colorGradeA = {
			options = colorgrades
		},
		colorGradeB = {
			options = colorgrades
		},
		direction = {
			options = { "LeftToRight", "TopToBottom" },
			editable = false
		}
	}
}
