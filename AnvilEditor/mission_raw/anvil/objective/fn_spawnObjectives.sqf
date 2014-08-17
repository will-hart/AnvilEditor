/*
	Author: Will Hart

	Description:
	  Starts all objectives which require the given mission ID as a prerequisite. Is automatically
	  called when the give mission ID has just been completed.

	Parameter(s):
	  _this: INTEGER, the Id of the mission that was just completed

	Example:
	  _id call AFW_fnc_spawnObjectives;
	  3 call AFW_fnc_spawnObjectives;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

{
    if (_this in O_PREREQ(_x)) then {
        if (!(O_ID(_x) in completed_objectives) && !(O_ID(_x) in current_objectives)) then {
            _x call AFW_fnc_startObjective;
            
            current_objectives set [count current_objectives, O_ID(_x)];
            incomplete_objectives = incomplete_objectives - [O_ID(_x)];
            
            publicVariable "current_objectives";
            sleep 2;
            
            diag_log format ["Created objective %1", O_ID(_x)];
        };
    };
} forEach objective_list;

publicVariable "incomplete_objectives";
