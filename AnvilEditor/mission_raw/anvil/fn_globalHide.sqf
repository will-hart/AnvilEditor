/*
    Author: Will Hart

    Description:
      Runs on the server to hide an object globally

    Parameter(s):
      _this select 0: ARRAY, the objects to hide globally
      _this select 1: BOOL, true to hide, false to show

    Example:
      [[[vehicle_one], true], "AFW_fnc_globalHide"] spawn BIS_fnc_MP;

    Returns:
      Nothing
*/

#include "defines.sqf"

private ["_vehs", "_hide"];

_vehs = _THIS(0);
_hide = _THIS(1);

{
    if (isMultiplayer) then
    {
        _x hideObjectGlobal _hide;
    } 
    else 
    {
        _x hideObject _hide;
    };
} forEach _vehs;