/*
	Author: Will Hart

	Description:
	  Creates a new mission, where an object must be destroyed.

	Parameter(s):
	  _this select 0: OBJECT, the objective being created
	  _this select 1: FUNCTION, the function to call when the EOS mission completes
	  _this select 2: FUNCTION, the function to call when the objective has been completed

	Example:
	  [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_Destroy;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_veh"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

// spawn the occupation - callback passed should be a NOP
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

_pos = O_POS(_obj);
if (O_RANDOMISE(_obj)) then {
    _pos = [_obj] call AFW_fnc_getRandomSpawnPosition;
};
_safePos = _pos findEmptyPosition [0, 30, "Land_TTowerSmall_1_F"];

// spawn the radio tower on the marker
_veh = "Land_TTowerSmall_1_F" createVehicle _pos;

waitUntil { sleep 5; !(alive _veh)};
_obj spawn _CB;