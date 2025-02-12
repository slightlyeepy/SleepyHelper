-- This file uses code from Maddie's Helping Hand -- see LICENSE.MaddiesHelpingHand file.

local trigger = {}

local colorGrades = {
	"none", "oldsite", "panicattack", "templevoid", "reflection", "credits", "cold", "hot", "feelingdown", "golden"
}

trigger.name = "SleepyHelper/FlagToggleColorGradeFadeTrigger"
trigger.category = "visual"
trigger.triggerText = "Flag Toggle Color Grade Fade"
trigger.placements = {
	name = "trigger",
	data = {
		colorGradeA = "none",
		colorGradeB = "none",
		direction = "LeftToRight",
		flag = "flag_toggle_color_grade_fade",
		inverted = false,
		evenDuringReflectionFall = false
	}
}

trigger.fieldInformation = {
	colorGradeA = {
		options = colorGrades
	},
	colorGradeB = {
		options = colorGrades
	},
	direction = {
		options = { "LeftToRight", "TopToBottom" },
		editable = false
	}
}

return trigger
