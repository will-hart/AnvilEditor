﻿/*
	Author: Will Hart

	Description:
	  Creates an ammo crate and populates it with the given objects. Supplied objects may be the 
	  string class name, or an array containing [class name, quantity]. See the example. Provide an
	  empty array for no items.

	Parameter(s):
	  _this select 0: ARRAY, the position to place the ammobox near to (a safe nearby position will be found)
	  _this select 1: INT, the radius within which the ammobox should be created
	  _this select 2: ARRAY, the items to add
	  _this select 3: ARRAY, the weapons to add
	  _this select 4: ARRAY, the magazines to add

    Example:
	  ammobox = [position player, 10, ['Medikit", ["FirstAidKit", 10]], [], ["30Rnd_65x39_case_mag"]] call FW_fnc_createAmmobox;
	
	Returns:
	  A handle to the ammobox so you can fill it up with goodies
*/

private ["_pos", "_safePos", "_radius", "_handle", "_items", "_weapons", "_magazines"];

_pos = _this select 0;
_radius = _this select 1;
_weapons = _this select 2;
_magazines = _this select 3;
_items = _this select 4;

// find a safe position and drop the ammobox
_handle = [_pos, _radius] spawn FW_fnc_createAmmobox;
waitUntil { scriptDone _handle };

{
	if (typeName _x == "STRING") then {
		_x = [_x, 1];
	};
	FW_last_user_ammobox addItemCargoGlobal _x;
} forEach _items;

{
	if (typeName _x == "STRING") then {
		_x = [_x, 1];
	};
	FW_last_user_ammobox addWeaponCargoGlobal _x;
} forEach _weapons;

{
	if (typeName _x == "STRING") then {
		_x = [_x, 1];
	};
	FW_last_user_ammobox addMagazineCargoGlobal _x;
} forEach _weapons;

FW_last_user_ammobox