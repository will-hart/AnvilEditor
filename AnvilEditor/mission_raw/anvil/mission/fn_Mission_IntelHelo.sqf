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

private ["_eosCB", "_CB", "_obj", "_obj_name", "_intel_var", "_intel"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);
_intel_var = format ["%1_intel", _obj_name];

// spawn the objective occupation
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

// create the intelligence
server setVariable [_intel_var, false];
_intel = "Land_Wreck_Heli_Attack_01_F" createVehicle O_POS(_obj);

[[_intel, "<t color='#11FF11'>Gather Intel</t>", {
    hint "Gathering intel";
    sleep random 30;
    hint "Intel gathered";
    deleteVehicle _THIS(0);
}, _intel], "AFW_fnc_addActionMP", nil, true] spawn BIS_fnc_MP;

// wait until the intel is gathered
waitUntil { sleep 5; !alive _intel };

// complete the mission
_obj spawn _CB;