/*
    Author: Will Hart

    Description:
      Creates a new mission, where intelligence must be recovered by the player.

    Parameter(s):
      _this select 0: OBJECT, the objective being created
      _this select 1: FUNCTION, the function to call when the EOS mission completes
      _this select 2: FUNCTION, the function to call when the objective has been completed

    Example:
      [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_IntelHeli;

    Returns:
      Nothing
*/

if (!isServer) exitWith { false };

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_intel_var", "_intel", "_pos"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);
_intel_var = format ["%1_intel", _obj_name];

// spawn the objective occupation
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

if (O_RANDOMISE(_obj)) then {
    _pos = _obj call AFW_fnc_getRandomSpawnPosition;
} else {
    _pos = (O_POS(_obj));
};
_pos = _pos findEmptyPosition [0, 30, "Land_Wreck_Heli_Attack_01_F"];

// create the intelligence
server setVariable [_intel_var, false];
_intel = "Land_Wreck_Heli_Attack_01_F" createVehicle _pos;
_intel setVariable ["complete", false, true];

[[_intel, "<t color='#11FF11'>Gather Intel</t>", {
    hint "Gathering intel";
    sleep random 30;
    hint "Intel gathered";
    _THIS(0) setVariable ["complete", true, true];
}, _intel], "AFW_fnc_addActionMP", nil, true] spawn BIS_fnc_MP;

// wait until the intel is gathered
waitUntil { sleep 5; _intel getVariable "complete" };

// complete the mission
_obj spawn _CB;