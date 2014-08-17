/*
    Author: Will Hart

    Description:
      Creates a new mission, where an ammobox must be eliminated

    Parameter(s):
      _this select 0: OBJECT, the objective being created
      _this select 1: FUNCTION, the function to call when the EOS mission completes
      _this select 2: FUNCTION, the function to call when the objective has been completed

    Example:
      [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_DestroyAmmo;

    Returns:
      Nothing
*/

if (!isServer) exitWith { false };

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_veh", "_group"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

// spawn the occupation - callback passed should be a NOP
[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

// spawn the officer and set them to patrol
_pos = [_obj, "Box_East_AmmoOrd_F"] call AFW_fnc_getRandomSpawnPosition;
_ammo = [_pos, 10, [], [], [["DemoCharge_Remote_Mag", 5], ["ClaymoreDirectionalMine_Remote_Mag", 5]], "Box_East_AmmoOrd_F"] call AFW_fnc_populateAmmobox;

sleep 5;
waitUntil { sleep 5; alive _ammo };

// mission success when the officer dies
waitUntil { sleep 8; !alive _ammo};
_obj spawn _CB;