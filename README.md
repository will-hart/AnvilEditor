# Anvil Mission Framework (Version 6.5.2)

*By |TG| Will*

## About

The Anvil Mission Framework is a system for rapidly generating missions of any size for ArmA 3. It provides entirely 
script based mission generation using a series of map markers to determine where missions should be placed, and 
therefore doesn't rely on any mods to function. Additionally the "Anvil Editor" is provided which is a graphical 
interface for developing progressive missions using the Anvil Mission Framework. It is easy to build complicated,
multi-objective missions suitable for dedicated servers in under an hour.

## Licenses

The Framework is licensed under the [ArmA Public License, Share Alike](https://www.bistudio.com/community/licenses/arma-public-license-share-alike),
which basically means you are free to use it in your missions and to edit, update, improve, pull apart
or otherwise play around with the scripts on the proviso that you keep the original credit and make the 
improvements available upon request to the main source code repository so that everybody benefits.

The Anvil Editor is provided under the highly permissive MIT license. Pull requests, suggestions and
feedback are more than welcome.

## Features

Current features of the framework include:

- Create progressive missions, where completing objectives unlocks new objectives
- Set and/or objective prerequisite conditions - e.g. RTB once all ammo caches have been destroyed
- Captured objectives can unlock ammo boxes, respawn points or special weaponry
- Enemy forces are dynamically spawned and cached using the Enemy Occupation System to reduce server load
- Save and load the objective state using scripts so you can complete the mission over multiple sittings
- Different mission types including:
    - Capture the area
    - Destroy tower, ammo, AA, crashed helicopters
    - Find intelligence documents or gather intel from a crashed chopper
    - Assassinate an HVT
    - Move to or through a specific location
- Mission parameters for:
    - Optional random enemy patrols in the AO
    - Optional random enemy counter-attacks when objectives are captured
    - Size of enemy forces to increase the difficulty or support a larger player count
- One click script inclusion (including description.ext or init files)
- Create your own one click scripts for your favourite scripts
- Supported maps include Altis (low res image), Stratis, Chernarus, Takistan and Zargabad
- Framework auto-updater

## Roadmap

The Editor already has a lot of features which make it great for rapidly creating missions. New features are generally added when there is a need for them
so there isn't a specific road map. The Github repository issues allow users to suggest features so this is the closes thing to a roadmap. Anvil Editor is now in
"beta" and has been used to generate a large number of missions for the [Tactical Gamer](www.tacticalgamer.com) servers including one mission with almost 200 objectives.

## A note on version numbers

Version numbers indicate both the framework version and the editor version. For instance version 6.5 indicates editor
version 6 and framework version 5. New framework versions (where there are no features that break backwards compatibility)
can be downloaded and installed through the Editor.

The software is in Beta as of version 6.5. Every effort has been made to ensure backwards compatibility with missions made in older versions
of the editor and to the best of my knowledge there have been no such breaking issues to date.

## Changelogs 

All version information and change logs are available from [http://www.anvilproject.com](http://www.anvilproject.com/downloads)

## Contributing 

All contributions and suggestions are welcome, although currently as I'm a one man band I can make no guarantees. Get 
in touch over on the [BI forums](http://forums.bistudio.com/showthread.php?180268-Release-Anvil-Mission-Editor-and-Framework)
or raise an issue on [Github](https://github.com/will-hart/AnvilEditor), or tweet to @wlhart.

## Credits & Acknowledgements (Core items)

The following resources are a key part of the Editor and Framework

*FRAMEWORK*

- Bangabob's Enemy Occupation System (EOS)

*EDITOR*

- Xceed WPF Extended Toolkit Community Edition (Microsoft Public License)
- [Newtonsoft.JSON](https://github.com/JamesNK/Newtonsoft.Json) (MIT License)
- [Sprache](https://github.com/sprache/Sprache) (MIT License)

## Credits & Acknowledgements (Bundled Scripts)

The following scripts are bundled with the framework as optional addons

- Tonic for TAW_VD view distance script (bundled script)
- Aerson's Group Manager
- TacticalGamer.com Name Scrips (tacticalgamer.com)
- =BTC= Logistics
- TacticalGamer.com Task Force Radio autodetect script
- Generic Vehicle Service 
- others?