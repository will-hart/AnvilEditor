#include "defines.sqf"

if (!isServer) exitWith {};

{
    if (_this in O_PREREQ(_x)) then {
        if (!(O_ID(_x) in completed_objectives) && !(O_ID(_x) in current_objectives)) then {
            _x call FW_fnc_startObjective;
            
            current_objectives set [count current_objectives, O_ID(_x)];
            incomplete_objectives = incomplete_objectives - [O_ID(_x)];
            diag_log format ["Created objective %1", O_ID(_x)];
        };
    };
} forEach objective_list;

publicVariable "current_objectives";
publicVariable "incomplete_objectives";
