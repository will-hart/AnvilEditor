/*
	Author: Will Hart

	Description:
	  Spawns a special weapon rewar at the given marker. Available special weapons are defined in the `support_weapons`
	  variable set in the `framework_init.sqf`

	Parameter(s):
	  _this: STRING, the name of the marker where the special weapon spawn should be created

	Example:
	  "special_marker_0" call FW_fnc_spawnSpecialWeapon;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (isDedicated) exitWith {
	false
};

private ['_weapons', '_marker', '_crate', '_pos', '_safePos'];
_pos = getMarkerPos _this;

// set up the marker
_this setMarkerType "n_support";

// find a safe position
_safePos = _pos findEmptyPosition [0,10,"I_SupplyCrate_F"];

// update the marker to a safe position
if (count _safePos > 0) then {
	_pos = _safePos;
	_this setMarkerPos _pos;
};

// select a reward
_weapons = support_weapons select floor random count support_weapons;

// create a crate
_crate = "B_supplyCrate_F" createVehicleLocal _pos;
_crate setVariable ["BTC_cannot_lift",1,true];
_crate setVariable ["BTC_cannot_drag",1,true];
_crate setVariable ["BTC_cannot_load",1,true];

// remove existing
clearMagazineCargo _crate;
clearWeaponCargo   _crate;
clearItemCargo     _crate;
clearBackpackCargo _crate;

// fill the crate
{
    _crate addBackpackCargo [_x, 2];
} forEach _weapons;

diag_log format ["Spawned special weapon %1 at safe pos %2 for %3", _weapons, _pos, getMarkerPos _this]; 