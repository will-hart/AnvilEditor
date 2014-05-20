#include "defines.sqf"

if (!isServer) exitWith {};

private ["_mkr", "_task_name", "_obj", "_obj_title", "_obj_description"];

// gather some information
_obj = _this;
_task_name = O_EOS_NAME(_obj);
_obj_title = format ["Capture %1", O_DESCRIBE(_obj)];
_obj_description = format ["%1.  %2", _obj_title, O_REWARDS(_obj)];

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
O_MARKER(_obj) setMarkerType "Flag1";
O_MARKER(_obj) setMarkerColor "ColorRed";

// set up the EOS zone
_str = "FW_EnemyStrength" call BIS_fnc_getParamValue;
_nul = [[_task_name],
        [O_INF(_obj) * _str, O_STR(_obj), 90],          //House Groups, Size of groups, Probability
        [O_INF(_obj) * _str, O_STR(_obj), 90],          //Patrol Groups, Size of groups, Probability
        [O_VEH(_obj) * _str, O_STR(_obj), 90],          //Light Vehicles, Size of Cargo, Probability
        [O_ARM(_obj) * _str, 70],                       //Armoured Vehicles, Probability
        [O_VEH(_obj) * _str, 50],                       //Static Vehicles, Probability
        [O_AIR(_obj) * _str, 0, 80],                    //Helicopters, Size of Cargo, Probability
        [0, 2, 1000, EAST, FALSE, FALSE, [_obj, FW_fnc_completeObjective]]] call EOS_Spawn;
                                                //Faction, Markertype, Distance, Side, HeightLimit, Debug

// add to the current player objectives
[format ["tsk_%1", _task_name], _obj_title, _obj_description, true, [_task_name, getMarkerPos _mkr]] call SHK_Taskmaster_add;