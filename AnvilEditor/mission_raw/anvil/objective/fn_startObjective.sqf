/*
	Author: Will Hart

	Description:
	  Commences an objective, creating the marker, setting up the enemy spawn and creating the user task

	Parameter(s):
	  _this: ARRAY, the objective (from objective_list) being started

	Example:
	  (objective_list select 0) call FW_fnc_startObjective;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_mkr", "_task_name", "_desc", "_miss_type", "_obj", "_obj_title", "_obj_description", "_fns"];

// gather some information
_obj = _this;
_task_name = O_EOS_NAME(_obj);
_desc = O_DESCRIBE(_obj);
_miss_type = O_MISSIONTYPE_DESC(_obj);

_obj_title = format ["%1 %2", _miss_type, _desc];
_obj_description = format ["%1.  %2 (#%3)", _obj_title, O_REWARDS(_obj), O_ID(_obj)];

// create and format the marker
_mkr = createMarker [_task_name, O_POS(_obj)];
_mkr setMarkerSize O_SIZE(_obj);

// DEBUG:
_mkr setMarkerShape "ELLIPSE";
_mkr setMarkerColor "ColorRed";
_mkr setMarkerAlpha 0.5;
_mkr setMarkerText _obj_title;

// Set up the existing marker
O_MARKER(_obj) setMarkerText O_DESCRIBE(_obj);
O_MARKER(_obj) setMarkerType "mil_flag";
O_MARKER(_obj) setMarkerColor "ColorRed";
 
// start the mission based on the specified type
// first item is setup, second item is EOS callback, third item is general callback
_fns = EL(mission_types, O_MISSIONTYPE(_obj));
[_obj, EL(_fns, 1), EL(_fns, 2)] spawn EL(_fns, 0);

// add to the current player objectives
_null = [WEST, O_TASK_NAME(_obj), [_obj_description, _obj_title, _miss_type], getMarkerPos O_MARKER(_obj)] spawn BIS_fnc_taskCreate;