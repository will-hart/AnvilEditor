/*
	Author: Will Hart

	Description:
	  Set a new spawn destination for the given patrol group

	Parameter(s):
	  _this select 0 - the group to create a new waypoint for

    Example:
	  _nul = [_group] spawn FW_fnc_setPatrolDestination;
	  	
	Returns:
	  Nothing
*/

if (!isServer) exitWith {};

private ["_group", "_wp", "_dst"];

_group = _this;
_dst = (current_objectives call BIS_fnc_selectRandom);


diag_log format ["Updating patrol with new waypoint: %1", _dst];
_wp = _group addWaypoint [O_POS(_dst), 0];
_wp setWaypointStatements ["true", "[group this] spawn FW_fnc_setPatrolDestination;"];