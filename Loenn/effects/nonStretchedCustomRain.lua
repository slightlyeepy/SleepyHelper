return {
	name = "SleepyHelper/NonStretchedCustomRain",
	associatedMods = {
		"SleepyHelper",
		"ChroniaHelper"
	},
	defaultData = {
		only = "*", exclude = "", flag = "", notflag = "", tag = "",
		scrollx = 1.0, scrolly = 1.0, angle = 270.0, angleDiff = 2.86, speedMult = 1.0, scaleMult = 1.0, amount = 240, colors = "ffffff", alpha = 1.0
	},
	fieldOrder = {
		"only", "exclude", "flag", "notflag", "scrollx", "scrolly", "speedMult", "amount", "colors", "alpha"
	},
	canBackground = true,
	fieldInformation = {
		colors = {
			fieldType = "list",
			elementOptions = {
				fieldType = "color",
			},
		},
	},
}
