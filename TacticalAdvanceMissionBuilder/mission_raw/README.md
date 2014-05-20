# Tactical Advance

*By |TG| Will*

This mission is a full island mission where two NATO squads make an amphibious landing and try to take over Altis 
one objective at a time. The mission was made by |TG| Will for the TacticalGamer.com community. The code for
this mission is public domain, you are free to modify and use it as you wish, however please acknowledge this original 
mission (this one) in your documentation should you do so.

The mission features include:

- A series of progressive objectives covering 1/4 of Altis
- Capturing an objective opens up new objectives
- Capturing some objectives unlocks new respawn locations, ammo boxes, vehicles, etc
- Random enemy patrols in the AO (can be switched off)
- Random enemy counter-attacks when objectives are captured (can be switched off)
- Lots of mission parameters to control difficulty
- Standard TG scripts including group manager, TAW VD and TG name scripts
- =BTC= logistics including lift, fast ropes and loading
- Enemy forces are dynamically spawned and cached using the Enemy Occupation Script to reduce server load
- Custom ammo cookoff effect on all spawned vehicles
- Optional Zeus master / moderator to spice things up (the mission is designed to be playable without Zeus, so this is just an added extra)

On the Roadmap:

- Expand objectives to cover the whole island
- Optional side missions which give access to special weapons
- Additional fortifications etc around the island
- Unlockable Mobile HQ for respawn in the field
- Aircraft, armour and tank unlocks
- Load mission objective state from file or debug console to recover progress after a server restart. This will involve an array of completed objective IDs - e.g. `[0, 1, 2] call FW_fn_loadObjectiveState`

TODO:

- Make `isServer` check into a function and add support for headless client (including parameter)
- Revive?