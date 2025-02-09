-- This file uses code from Maddie's Helping Hand:
--
-- The MIT License (MIT)
--
-- Copyright (c) 2019 maddie480
--
-- Permission is hereby granted, free of charge, to any person obtaining a copy
-- of this software and associated documentation files (the "Software"), to deal
-- in the Software without restriction, including without limitation the rights
-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
-- copies of the Software, and to permit persons to whom the Software is
-- furnished to do so, subject to the following conditions:
--
-- The above copyright notice and this permission notice shall be included in all
-- copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
-- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
-- SOFTWARE.

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
