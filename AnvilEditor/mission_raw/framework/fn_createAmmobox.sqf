/*
	Author: Will Hart

	Description:
	  Creates an empty ammo crate and returns a handle to the caller

	Parameter(s):
	  _this select 0: ARRAY, the position to place the ammobox near to (a safe nearby position will be found)
	  _this select 1: INT, the radius within which the ammobox should be created

    Example:
	  ammo = [position player, 10] call FW_fnc_createAmmobox;
	
	Returns:
	  A handle to the ammobox so you can fill it up with goodies
*/

private ["_pos", "_safePos", "_radius", "_crate"];

_pos = _this select 0;
_radius = _this select 1;

// find a safe position
_safePos = _pos findEmptyPosition [1, _radius,"I_SupplyCrate_F"];
_crate = "I_supplyCrate_F" createVehicle _safePos;

// return the crate handle
_crate