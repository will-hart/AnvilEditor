/*
	Author: Will Hart

	Description:
	  Creates an empty ammo crate and returns a handle to the caller. Returns the ammobox variable 
	  but also sets the variable "AFW_last_user_ammobox".

	Parameter(s):
	  _this select 0: ARRAY, the position to place the ammobox near to (a safe nearby position will be found)
	  _this select 1: INT, the radius within which the ammobox should be created

    Example:
	  ammo = [position player, 10] call AFW_fnc_createAmmobox;
      ammo = [position player, 10, "Box_East_AmmoOrd_F"] call AFW_fnc_createAmmobox;
	
	Returns:
	  A handle to the ammobox so you can fill it up with goodies
*/

private ["_pos", "_safePos", "_radius", "_crate", "_ammoboxType"];

_pos = _this select 0;
_radius = _this select 1;
_ammoboxType = if (count _this > 2) then { _this select 2 } else { "I_SupplyCrate_F" };

// find a safe position and drop the ammobox
_safePos = _pos findEmptyPosition [0, _radius, _ammoboxType];
_crate = _ammoboxType createVehicle _safePos;

// clear the ammobox
clearMagazineCargoGlobal _crate;
clearWeaponCargoGlobal   _crate;
clearItemCargoGlobal     _crate;
clearBackpackCargoGlobal _crate;

// save the handle
AFW_last_user_ammobox = _crate;
publicVariable "AFW_last_user_ammobox";

// return the crate handle
_crate