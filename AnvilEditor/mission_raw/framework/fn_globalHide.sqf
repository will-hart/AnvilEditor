/*
	Author: Will Hart

	Description:
	  Runs on the server to hide an object globally

	Parameter(s):
	  _this: OBJECT, the object to hide globally

	Example:
	  [vehicle_one, "FW_fnc_globalHide"] spawn BIS_fnc_MP;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

hideObjectGlobal _this;