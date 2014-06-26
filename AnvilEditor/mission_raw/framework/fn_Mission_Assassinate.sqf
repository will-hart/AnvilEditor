/*
	Author: Will Hart

	Description:
	  Creates a new mission, where an HVT must be eliminated

	Parameter(s):
	  _this select 0: OBJECT, the objective being created
	  _this select 1: FUNCTION, the function to call when the EOS mission completes
	  _this select 2: FUNCTION, the function to call when the objective has been completed

	Example:
	  [objective, FW_fnc_NOP, FW_fnc_NOP] call FW_fnc_Mission_Assassinate;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_veh", "_group", "_vehType"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

// spawn the occupation - callback passed should be a NOP
[_obj, _eosCB] spawn FW_fnc_doEosSpawn;

if (enemyTeam == EAST) then {
	_vehType = "O_Officer_F";
} else {
	if (enemyTeam == INDEPENDENT) then {
		_vehType = "I_Officer_F";
	} else {
		_vehType = "B_Officer_F";
	};
};

// spawn the officer and set them to patrol
_group = createGroup enemyTeam;
_veh = _group createUnit [_vehType, O_POS(_obj), [], 0, "FORM"];
[_group, O_POS(_obj), O_R(_obj)] call bis_fnc_taskPatrol;

// mission success when the officer dies
waitUntil {!alive _veh};
_obj spawn _CB;