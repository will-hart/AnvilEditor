/*
	Author: Will Hart

	Description:
	  Loads in a given objective state (i.e. active and completed objectives). This allows
	  a mission to be restarted from a previously saved point. It does not save weapons, and 
	  it also doesn't sense check the objectives so they can potentially be completed out of 
	  order. This function should usually be restricted to admins.

	Parameter(s):
	  _this: ARRAY, a list of objective IDs which should be considered complete

    Example:
	  [0, 1, 2] call AFW_fnc_setObjectiveState;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (serverCommandAvailable "#kick") then {
	[_this, "AFW_fnc_doSetObjectiveState", false] spawn BIS_fnc_MP;
} else {
	diag_log format ["Player '%1' attempted to execute AFW_fnc_setObjectiveState but does not have permission", name player];
};