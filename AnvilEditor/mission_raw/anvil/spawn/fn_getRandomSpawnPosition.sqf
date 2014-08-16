/*
	Author: Will Hart, based on Shuko's shkPos

	Description:
	  Selects a random position within the objective

	Parameter(s):
	  _this select 0: OBJECITVE, the objective to return a position for

    Example:
	  _pos = [_obj] call AFW_fnc_getRandomSpawnPosition;
	
	Returns:
	  A position at which to spawn the objective
*/

private ["_obj", "_pos", "_centre", "_rad"];

_obj = _THIS(0);
_centre = O_POS(_obj);
_rad = O_R(_obj);

_pos = [_centre, random 360, _rad, false, 1] call SHK_pos;
_pos