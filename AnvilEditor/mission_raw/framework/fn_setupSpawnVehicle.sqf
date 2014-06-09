/*
	Author: Will Hart

	Description:
	  Hides a vehicle attached to a respawn module and adds the cookoff event handler

	Parameter(s):
	  _this: OBJECT, the vehicle to set up

	Example:
	  In the vehicle's init: 
	    _nul = this call FW_fnc_setupSpawnVehicle;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

_this hideObjectGlobal true;

