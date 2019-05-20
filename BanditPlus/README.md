# Banditplus

  - Makes the bandit selectable in the character menu
	- Stats of abilities are configurable in `\BepInEx\config\BanditPlus.cfg`
  - Changes the smokescreen to the version with an actual animation
  - Changes Bandits special to the secondary of MUL-T with different stats.
  - Gives bandit skill names and descriptions.
	- Description changes based on stats in config.
  - Gives lights out a timer after hit to still get regen on kill.
	- Configurable icon (Save a png named `lightsout.png` at the dll location)
	- Option for displaying of time left

## Installation
Drop BanditMod.dll into `\BepInEx\plugins\`

## Changelog
	- v0.1.1
		- Increased radius of smokescreen.
		- Sped up startanimation.
	- v0.2.0
		- Added names and description to every skill.
	- v0.2.1
		- Improved automatic shooting speed and added config to change it
	- v0.3.0
		- Added configuration.
		- Included BanditTimer mod.
		- Moved bandit to a different spot for better functionality
	- v0.3.1
		- Fixed game breaking buggs
	- v0.3.2
		- Fixed SmokeScreen in multiplayer (config is temporarily not usable for this skill)
	- v0.3.3
		- Made the timer client based
	- v0.3.4
		- Fixed error on kill flying enemies.
	- v0.3.5
		- Changed how timer worked behind the scene, now acts nicely with other mods that add buffs (not that there is any yet)
	- v4.0.0
		- Added a softdependency to https://thunderstore.io/package/mistername/BuffDisplayAPI/
	- v4.0.1
		- fixed config file location for renamed config folder
		
## ToDo
	- Give bandit an unique special skill.
	- More fine tuning.
	- Give bandit an description.
	- Give bandit an character display.