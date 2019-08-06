# SoundPlus
	- Loads custom .bnk files from the plugin folder if they are named .sound instead.
	- Loads custom .bnk files passed to SoundPlus.SoundBanks.add(byte[]).
	- The bank files should be from wwise 2018.1

## Installation
Drop `SoundPlus.dll` into `\BepInEx\plugins\`

## Changelog
	- v0.1.0
		- release
	- v0.1.1
		- Makes updates to .sound files actually update (thanks to violet chaolan for helping)
	- v0.2.0
		no longer requires moving all stuff to sound and now supports an `byte[]` passed by other dlls

## ToDo
	- Bank Unloading