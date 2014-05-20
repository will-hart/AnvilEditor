#include "defines.sqf"

if (!isServer) exitWith {};

private ["_obj", "_mkr_name"];

diag_log "Loading mission states via script";

{
    _obj = objective_list select _x;
    _mkr_name = format ["obj_%1", O_ID(_obj)];
    diag_log format ["Forcing completed state for objective %1", O_ID(_obj)];
    
    // start the objective
    _obj call FW_fn_startObjective;
    diag_log format " - Mission started";
    
    // finish the objective!
    [_mkr_name] call EOS_Deactivate;
    diag_log " - EOS deactivated";
    _obj call FW_fn_completeObjective;
    diag_log " - Mission completed";
    
} foreach _this;