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

private ["_obj", "_unlock", "_oid", "_pre", "_preGroup", "_all"];

if (!isServer) exitWith {};

{
    _obj = _x;
    _oid = O_ID(_obj);
    
    if (_oid in completed_objectives or _oid in current_objectives) then {
        // already complete, ignore
    } else {
        // not complete yet, check prerequisites of the objective

        _pre = O_PREREQ(_x);
        _unlock = false;
        {
            scopeName "objectiveLoop";
            _preGroup = _x;
            
            if (typeName _preGroup == "SCALAR") then {
                if (_this == _preGroup) then { // handle scalar "or"
                    _unlock = true; 
                };
            };
            
            if (typeName _preGroup == "ARRAY") then { // handle "and" objective types, all elements in an array must be complete
                _all = true;
                {
                    scopeName "arrayLoop";
                    
                    if (!(_x in completed_objectives)) then { 
                        _all = false;
                        breakOut "arrayLoop";
                    };
                } forEach _preGroup;
                
                _unlock = _all;
            };

            // unlock if true
            if (_unlock) then {
                _obj call AFW_fnc_startObjective;
                
                current_objectives set [count current_objectives, _oid];
                incomplete_objectives = incomplete_objectives - [_oid];
                
                publicVariable "current_objectives";
                sleep 2;
                
                diag_log format ["Created objective %1", _oid];
                breakOut "objectiveLoop";
            };
        } forEach _pre;
    };
} forEach objective_list;

publicVariable "incomplete_objectives";