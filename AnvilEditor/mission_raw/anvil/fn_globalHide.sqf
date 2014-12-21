/*
    Author: Will Hart

    Description:
      Runs on the server to hide an object globally

    Parameter(s):
      _this select 0: ARRAY, the objects to hide globally
      _this select 1: BOOL, true to hide, false to show
      _this select 2: INT, [optional] an objective ID which must be incomplete for this to execute

    Example:
      [[[vehicle_one], true], "AFW_fnc_globalHide"] spawn BIS_fnc_MP;

    Returns:
      Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_vehs", "_hide", "_id", "_execute"];

_vehs = _THIS(0);
_hide = _THIS(1);
_id = if (count _this == 2) then { -1 } else { _THIS(2) };
_execute = true;

// don't execute if the objective is given and is complete
if (_id > -1) then {
    waitUntil { !isNil "completed_objectives" }; 
    if (_id in completed_objectives) then { _execute = false; };
};

// always show the vehicle and enable simulation, but only hide it if the objective is incomplete
if (_execute || !_hide) then { 
    {
        if (isMultiplayer) then
        {
            // support dedicated servers
            _x hideObjectGlobal _hide;
            _x enableSimulationGlobal !_hide;
            _x allowDamage !_hide;
        } 
        else 
        {
            // support single player
            _x hideObject _hide;
            _x enableSimulation !_hide;
            _x allowDamage !_hide;
        };
    } forEach _vehs;
};