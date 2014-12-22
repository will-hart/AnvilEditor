/*
    Author: Will Hart

    Description:
      Commences an objective, creating the marker, setting up the enemy spawn and creating the user task.
      If the objective is already in the completed list then EOS won't be spawned

    Parameter(s):
      _this: ARRAY, the objective (from objective_list) being started

      or 

      _obj: ARRAY, contains objective details
      _spawnEOS: BOOL, set flag to FALSE if EOS shoud NOT be spawned

    Example:
      (objective_list select 0) call AFW_fnc_startObjective; // standard with EOS spawn
      [(objective_list select 0), false] call AFW_fnc_startObjective; // without involving EOS

    Returns:
      Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_mkr", "_task_name", "_desc", "_miss_type", "_obj", "_obj_title", "_obj_description", "_fns"];

// gather some information - allow for missions to be completed without involving EOS by pasing a second parameter "false"
_obj = if (count _this == 2) then { _THIS(0) } else { _this };
_spawnEOS = if (count _this == 2) then { _THIS(1) } else { true };
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

// add to the current player objectives
_show_notification = !(O_ID(_obj) in completed_objectives);
_null = [WEST, O_TASK_NAME(_obj), [_obj_description, _obj_title, _miss_type], getMarkerPos O_MARKER(_obj), false, 1, _show_notification ] spawn BIS_fnc_taskCreate;

// check if we are spawning EOS and the objective is not complete
if (_spawnEOS) then {
  // start the mission based on the specified type
  // first item is setup, second item is EOS callback, third item is general callback
  _fns = EL(mission_types, O_MISSIONTYPE(_obj));
  [_obj, EL(_fns, 1), EL(_fns, 2)] spawn EL(_fns, 0);
} else {
  diag_log "   _spawnEOS set to false - EOS not being initialised";
};