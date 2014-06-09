/*
	Author: Will Hart

	Description:
	  Loads in a given objective state (i.e. active and completed objectives). This allows
	  a mission to be restarted from a previously saved point. It does not save weapons, and 
	  it also doesn't sense check the objectives so they can potentially be completed out of 
	  order. This function should usually be restricted to admins and MUST be called using `spawn`

	Parameter(s):
	  _this: ARRAY, a list of objective IDs which should be considered complete

    Example:
	  [0, 1, 2] spawn FW_fnc_doSetObjectiveState;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

//if (!isServer) exitWith {};

private ["_obj", "_mkr_name"];

diag_log "--------------------------------------------";
diag_log "    Loading mission states via script:";
diag_log "--------------------------------------------";
diag_log "Passed missions: ";
diag_log _this;

{
	if (!(_x in completed_objectives)) then {
		_obj = objective_list select _x;
		_mkr_name = format ["obj_%1", O_ID(_obj)];
		diag_log format [" - Processing objective %1", O_ID(_obj)];
		
		// start the objective
		if (!(_x in current_objectives)) then {
			_obj spawn FW_fnc_startObjective;
			sleep 2;
			current_objectives set [count current_objectives, _x];
			publicVariable "current_objectives";
			diag_log "      Forcibly started mission";
		};
		
		// finish the objective!
		_obj spawn FW_fnc_completeObjective;
		waitUntil {sleep 0.5; _x in completed_objectives;};
		diag_log "      Mission completed";
		[[_mkr_name]] call EOS_Deactivate;
		diag_log "      EOS deactivated";
	};
    
} forEach _this;

diag_log "--------------------------------------------";