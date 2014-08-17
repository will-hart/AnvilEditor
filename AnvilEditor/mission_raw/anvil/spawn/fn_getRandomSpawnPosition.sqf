/*
	Author: Will Hart

	Description:
	  Finds a random spawn position suitable for the given object within the objective radius

	Parameter(s):
	  _this select 0: ARRAY, The objective to find a spawn point for
	  _this select 1: STRING, The type of object to find a spawn point for

    Example:
	  pos = [_obj, "B_Officer_F"] call AFW_fnc_getRandomSpawnPosition;
	
	Returns:
	  A position to spawn the object at or the initial position if none can be found
*/

private ["_obj", "_safePos", "_radius", "_initial", "_temp", "_object", "_count"];

#include "defines.sqf"

_obj = _this select 0;
_object = _this select 1;

_radius = O_R(_obj);
_initial = O_POS(_obj);
_safePos = [];
_count = 0;

if (O_RANDOMISE(_obj)) then {

    while {format ["%1", _safePos] == "[]" and _count < 10} do {
        // find a random safe position
        _temp = [_initial, random 360, _radius, false, 1] call SHK_pos;
        _safePos = _temp findEmptyPosition [0, _radius, _object];
        _count = _count + 1;
    };

    if (format ["%1", _safePos] == "[]") then {
        _safePos = _initial;
    };
} else {
    _safePos = _initial;
};

// return the crate handle
_safePos