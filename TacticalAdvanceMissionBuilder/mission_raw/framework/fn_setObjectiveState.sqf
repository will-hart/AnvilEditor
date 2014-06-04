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
	  [0, 1, 2] call FW_fnc_setObjectiveState;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_obj", "_mkr_name"];

diag_log "Loading mission states via script";

{
	if (!(_x in completed_objectives)) then {
		_obj = objective_list select _x;
		_mkr_name = format ["obj_%1", O_ID(_obj)];
		diag_log format ["Forcing completed state for objective %1", O_ID(_obj)];
		
		// start the objective
		_obj call FW_fn_startObjective;
		diag_log " - Mission started";
		
		// finish the objective!
		[_mkr_name] call EOS_Deactivate;
		diag_log " - EOS deactivated";
		_obj call FW_fn_completeObjective;
		diag_log " - Mission completed";
	};
    
} foreach _this;