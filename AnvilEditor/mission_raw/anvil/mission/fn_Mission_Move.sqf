/*
	Author: Will Hart

	Description:
	  Creates a new mission, where the friendly team must move to the zone

	Parameter(s):
	  _this select 0: OBJECT, the objective being created
	  _this select 1: FUNCTION, the function to call when the EOS mission completes
	  _this select 2: FUNCTION, the function to call when the objective has been completed

	Example:
	  [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_Move;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_trg", "_act"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);
_trig_name = format ["%1_arrived", _obj_name];

if (friendlyTeam == EAST) then {
    _act = "EAST";
} else {
    if (friendlyTeam == WEST) then {
        _act = "WEST";
    } else {
        _act = "GUER";
    };
};

// spawn the occupation - callback passed should be a NOP
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

// create the trigger to detect at least 5 seconds of presence
_trg = createTrigger ["EmptyDetector", O_POS(_obj)];
_trg setTriggerArea [O_R(_obj), O_R(_obj), 0, false];
_trg setTriggerActivation [_act, "PRESENT", true];
_trg setTriggerTimeout [5, 5, 5, true];

// deactivate the trigger and perform the callback
waitUntil { sleep 5; triggerActivated _trg };
_trg setTriggerStatements ["false", "", ""];
_obj spawn _CB;