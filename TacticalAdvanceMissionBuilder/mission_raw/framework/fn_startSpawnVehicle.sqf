#include "defines.sqf"

if (!isServer) exitWith {};

{
    _x hideObjectGlobal false;
} forEach (synchronizedObjects _this);

