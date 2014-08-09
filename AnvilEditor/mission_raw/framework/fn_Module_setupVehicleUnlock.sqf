/*
    Author: Will Hart

    Description:
      Configures an unlockable vehicle respawn, by hiding all the vheicles synchronised to the 
      given trigger.

    Parameter(s):
      _this: ARRAY, the list of trigger objects to configure

    Example:
      [fw_module_1, fw_module_2] spawn FW_fnc_Module_setupVehicleUnlock;

    Returns:
      Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_triggers"];

// gather some information
_triggers = _this;

{
    {
        [synchronizedObjects _x, true] spawn FW_fnc_globalHide;
    }
} forEach _triggers;