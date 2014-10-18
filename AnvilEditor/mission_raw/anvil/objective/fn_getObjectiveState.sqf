/*
	Author: Will Hart

	Description:
	  Returns a list of the currently completed objectives. Can be used in conjunction with
	  AFW_fnc_setObjectiveState to restart the mission at a specific point in time.

	Parameter(s):
	  None

    Example:
	  objectives = [] call AFW_fnc_getObjectiveState;
	  ([] call AFW_fnc_getObjectiveState) call AFW_fnc_setObjectiveState;
	  	
	Returns:
	  A list of completed objectives equivalent to the public variable `completed_objectives`.
*/

#include "defines.sqf"

completed_objectives