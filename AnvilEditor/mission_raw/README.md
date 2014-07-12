# Anvil Mission Framework (Version 3)

*By |TG| Will*

## About

The Anvil Mission Framework and Editor is a system for rapidly generating missions of any size for ArmA 3, working in conjunction
with the ArmA 3 official 2D editor. The Framework provides a series of objective types which can be linked together so that completing
one objective unlocks another. Enemy occupation levels of objectives are highly configurable and managed using the Enemy Occupation 
System from bangabob.

This version also coincides with the release of the first Showcase mission "Night Assault" which is the start of a three mission 
"campaign" which will demonstrate the power of the Anvil Framework and Editor. The showcase missions are available from the 
Anvil Project homepage - http://www.anvilproject.com/downloads

## The Framework

Current features of the framework include:

- Create progressive missions, where completing objectives unlocks new objectives
- Captured objectives can unlock ammo boxes, respawn points or special weaponry
- Enemy forces are dynamically spawned and cached using the Enemy Occupation System to reduce server load
- Ambient occupation zones can be set up and configured
- Save and load the objective state using scripts so you can complete the mission over multiple sittings
- Different mission types including:
    - Capture the area
    - Destroy communications tower
	- Destroy AA
    - Assassinate an HVT
	- Gather intel
- Mission parameters for:
    - Optional random enemy patrols in the AO
    - Optional random enemy counter-attacks when objectives are captured
    - Size of enemy forces
- A series of one-click installable scripts to make the game world more realistic, including:
    - Scripts used by TacticalGamer including Aerson's group manager, TAW VD and the TG name scripts
    - =BTC= logistics including lift, fast ropes and loading

## The Editor

The editor is a Windows desktop application which allows users to rapidly generate the required markers and 
scripts to create an Anvil Framework based mission. It integrates with the ArmA 3 editor so that the two can
mostly be used side by side. The editor has the following functionality:

- Installable maps of Altis, Stratis, Chernarus, Takistan and Zargabad which can be zoomed and panned
- Ability to place objectives and modify all the relevant properties
- Link objectives together through "shift+clicking"
- Load and save mission information such as enemy and friendly sides
- Set victory / defeat triggers on each objective or for when all objectives are completed
- Export a nearly playable mission (just add units in the ArmA 3 official 2D editor)

## Roadmap

The roadmap lays out what features are being considered for future version of the framework `[FW]` and editor `[ED]`:

- [FW] Selectable cleanup script
- [FW] Allow loading mission state from dialog
- [FW] Scripted generation of vehicle respawns to save making them in the editor
- [FW] Think of some other mission types that might be fun!
- [FW] Vehicle respawn reward type
- [ED] Simple briefing editor
- [ED] Templating system for easy adding of groups, modules and so on
- [ED] Set up patrol routes on the maps
- [ED] Allow BIS modules to be attached to ambient zones
- [ED] Set start time in mission properties
- [ED] Configurable ammo boxes
- [ED] Auto update for the editor, not just the framework

## A note on version numbers

The first public release of the framework and editor will be version 4.3.  The first number will always refer to the framework version number, whilst the second number will always refer to the editor version number. 
