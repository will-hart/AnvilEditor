/*
	Author: Will Hart

	Description:
	  Creates a new mission, where intelligence must be recovered by the player.

	Parameter(s):
	  _this select 0: OBJECT, the objective being created
	  _this select 1: FUNCTION, the function to call when the EOS mission completes
	  _this select 2: FUNCTION, the function to call when the objective has been completed

	Example:
	  [objective, FW_fnc_NOP, FW_fnc_NOP] call FW_fnc_Mission_Intel;
	
	Returns:
	  Nothing
*/

if (!isServer) exitWith { false };

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_intel_var", "_intel"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);
_intel_var = format ["%1_intel", _obj_name];

// spawn the objective occupation
[_obj, _eosCB] spawn FW_fnc_doEosSpawn;

// create the intelligence
server setVariable [_intel_var, false];
_intel = "Land_Suitcase_F" createVehicle O_POS(_obj);

[[_intel, "<t color='#11FF11'>Gather Intel</t>", {
	hint "Gathering intel";
	sleep random 10;
	hint "Intel gathered";
	
	deleteVehicle _THIS(0);
}, _intel_var], "FW_fnc_addActionMP", nil, false] spawn BIS_fnc_MP;

// wait until the intel is gathered
waitUntil { sleep 5; !alive _intel };

// complete the mission
_obj spawn _CB;