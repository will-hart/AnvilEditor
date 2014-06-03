/*
	Author: Will Hart

	Description:
	  Shows all vehicles synchronised to a given respawn module

	Parameter(s):
	  _this: OBJECT, a module to use for respawning
	
	Example:
	  my_respawn_module call FW_fnc_startSpawnVehicle;

	Returns:
	Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

{
    _x hideObjectGlobal false;
    _x addEventHandler ["Killed", {_nul = [_this select 0] execVM "cookoff.sqf";}];
} forEach (synchronizedObjects _this);

