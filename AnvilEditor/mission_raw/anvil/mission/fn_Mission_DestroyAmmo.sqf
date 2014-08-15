/*
    Author: Will Hart

    Description:
      Creates a new mission, where an HVT must be eliminated

    Parameter(s):
      _this select 0: OBJECT, the objective being created
      _this select 1: FUNCTION, the function to call when the EOS mission completes
      _this select 2: FUNCTION, the function to call when the objective has been completed

    Example:
      [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_Assassinate;

    Returns:
      Nothing
*/

if (!isServer) exitWith { false };

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_veh", "_group", "_items"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

_items = [[], [], [["DemoCharge_Remote_Mag", 5], ["ClaymoreDirectionalMine_Remove_Mag", 5]];

// spawn the occupation - callback passed should be a NOP
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

// spawn the officer and set them to patrol
_ammo = [O_POS(_obj), 10, _items] spawn AFW_fnc_populateAmmobox;

// mission success when the officer dies
waitUntil { sleep 8; !alive _ammo};
_obj spawn _CB;