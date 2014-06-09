# Anvil Mission Framework (Version 2.2)

*By |TG| Will*

## About

The Anvil Mission Framework is a system for rapidly generating missions of any size for ArmA 3. It provides entirely 
script based mission generation using a series of map markers to determine where missions should be placed, and 
therefore doesn't rely on any mods to function. Additionally the "Anvil Editor" is provided which is a graphical 
interface for developing progressive missions using the Anvil Mission Framework. 

The source code for the framework will be released under a permissive ArmA license whilst the source code for the 
editor will be released under the MIT license.

Two demo missions known as *Tactical Advance* and *Spearhead* will be provided when the framework is released to 
show the power of the system. The *Tactical Advance* mission covers the entire surface of Altis with about 150 missions, 
whilst the Spearhead mission demonstrates a smaller mission with a specific (and still secret!) theme.

## Features

Current features of the framework include:

- Create progressive missions, where completing objectives unlocks new objectives
- Captured objectives can unlock ammo boxes, respawn points or special weaponry
- Enemy forces are dynamically spawned and cached using the Enemy Occupation System to reduce server load
- Save and load the objective state using scripts so you can complete the mission over multiple sittings
- Different mission types including:
    - Capture the area
    - Destroy communications tower
    - Assassinate an HVT
- Mission parameters for:
    - Optional random enemy patrols in the AO
    - Optional random enemy counter-attacks when objectives are captured
    - Size of enemy forces
- A series of scripts to make the game world more realistic, including:
    - Scripts used by TacticalGamer including Aerson's group manager, TAW VD and the TG name scripts
    - =BTC= logistics including lift, fast ropes and loading
    - Custom scripted vehicle ammo cookoff effects

## The Editor

The editor is a C# application which allows users to rapidly generate the required markers and scripts to create an Anvil Framework based mission. 
The editor has the following functionality:

- A high resolution map of Altis which can be zoomed and panned
- Place objectives and modify all the relevant properties
- Link objectives together through "shift+clicking"
- Load and save mission information
- Generate a complete mission
- Access the mission data for copying / pasting into an existing mission

## Roadmap

The roadmap lays out what features are planned for future version of the framework:

- [FW] Refactor vehicle spawn code 
- [FW] Cleanup script required?
- [FW] Restrict mission state loading / saving to server admins
- [FW] Allow loading mission state from dialog
- [FW] Support for headless client in spawning and managing AI
- [FW] Scripted generation of vehicle respawns to save making them in the editor
- [FW] Unlockable Mobile HQ for respawn in the field
- [FW] Create a "capture the intel" mission type
- [FW] Think of some other mission types that might be fun!

- [ED] Place framework modules from the editor
- [ED] Mission templates (i.e. place an infantry squad down in the mission SQM)
- [ED] Briefing manager

## A note on version numbers

The first public release of the framework and editor will be version 1.2.  The first number will always refer to the framework version number, whilst the second number will always refer to the editor version number. 

## Changelogs 

All version information and change logs are available from http://www.anvilproject.com

## Credits & Acknowledgements (Core items)

The following resources are a key part of the Editor and Framework

*FRAMEWORK*

- Bangabob's Enemy Occupation System (EOS)

*EDITOR*

- Xceed WPF Extended Toolkit Community Edition (Microsoft Public License)
- Newtonsoft.JSON (MIT License)

## Credits & Acknowledgements (Bundled Scripts)

The following scripts are bundled with the framework as optional addons

- Tonic for TAW_VD view distance script (bundled script)
- Aerson's Group Manager
- TG Name Scrips (tacticalgamer.com)
- =BTC= Logistics
