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

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

// spawn the objective
[_obj, _eosCB] spawn FW_fnc_doEosSpawn;

// auto-complete
server setVariable [_obj_name, TRUE, TRUE];

waitUntil { server getVariable _obj_name };

_obj spawn _CB;