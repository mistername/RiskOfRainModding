
# now included in r2api
do not use this

# Sound
	- Loads custom .bnk files from the plugin folder if they are named .sound instead.
	- Loads custom .bnk files passed to `AssetPlus.SoundBanks.Add()`.
	- The bank files should be from wwise 2018.1

# Language
	- Loads any *.language file located inside `\BepInEx\plugins\` or passed to `AssetPlus.Languages.Add()`
		- They should have the same structure as the ingame language files, look at angry text mod as example
	- If the loaded file has the same value as files inside `Risk of Rain 2_Data\Language\*` it overrides their value.
		- Currently last loaded file has priority.

# font
	- Loads TMP_TMPFont from assetbundles that have extension .font or passed to `AssetPlus.Fonts.Add()`
		- Uses the first font loaded into it, .font are loaded first


## Installation
Drop `AssetPlus.dll` into `\BepInEx\plugins\`

## Changelog
	- v0.1.0
		- release
	- v0.1.1
		- Added support for bank unloading
	- v0.1.2
		- text
			- Added support for single tokens from code, generic or per language
		- sound
			- Added bank loading during runtime (automatically gets done on an Add after the normal point)
		- fonts
			- nothing
		- xml
			- Added the documentation to the zip

#todo