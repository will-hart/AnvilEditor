/*
	Author: Will Hart

	Description:
	  Creates a new mission, where an HVT must be eliminated

	Parameter(s):
	  _this select 0: OBJECT, the objective being created
	  _this select 1: FUNCTION, the function to call when the EOS mission completes
	  _this select 2: FUNCTION, the function to call when the objective has been completed

	Example:
	  [objective, AFW_fnc_NOP, AFW_fnc_NOP] call AFW_fnc_Mission_DestroyHelo;
	
	Returns:
	  Nothing
*/

if (!isServer) exitWith { false };

#include "defines.sqf"

private ["_eosCB", "_CB", "_obj", "_obj_name", "_veh", "_group", "_vehType"];

_obj = _THIS(0);
_eosCB = _THIS(1);
_CB = _THIS(2);
_obj_name = O_OBJ_NAME(_obj);

if (! (O_ID(_obj) in completed_objectives)) then {

	if (friendlyTeam == EAST) then {
	    _vehType = "O_APC_Tracked_02_AA_F";
	} else {
	    if (friendlyTeam == INDEPENDENT) then {
	        _vehType = "O_Heli_Attack_02_F";
	    } else {
	        _vehType = "B_Heli_Transport_01_camo_F";
	    };
	};

	// spawn the objective occupation
	[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

	// spawn the occupation - callback passed should be a NOP
	_pos = [_obj, _vehType] call AFW_fnc_getRandomSpawnPosition;
	[_obj, _eosCB] spawn AFW_fnc_doEosSpawn;

	// spawn the officer and set them to patrol
	_veh = _vehType createVehicle _pos;
	_veh setDamage 0.7;
	_veh setFuel 0;
	_veh setFuelCargo 0;

	// mission success when the officer dies
	waitUntil { sleep 5; !alive _veh };
};

_obj spawn _CB;