#include "defines.sqf"

if (!isServer) exitWith {};

_this hideObjectGlobal true;
_this addEventHandler ["Killed", {_nul = [_this select 0] execVM "cookoff.sqf";}];

