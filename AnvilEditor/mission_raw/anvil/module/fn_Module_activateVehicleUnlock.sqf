/*
    Author: Will Hart

    Description:
      ACtivates an vehicle unlockable spawn by showing alll synchronised vehicles

    Parameter(s):
      _this: OBJECT, the trigger to activate

    Example (in activate line of the trigger):
      this spawn AFW_fnc_Module_activateVehicleUnlock;

    Returns:
      Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_trigger"];

// gather some information
_trigger = _this;
[synchronizedObjects _trigger, false] spawn AFW_fnc_globalHide;