#include "defines.sqf"

if (!isServer) exitWith {};

private ["_id", "_task_name", "_str", "_likely", "_ammo_mkr"];

_id = O_ID(_this);
_task_name = O_EOS_NAME(_this);

// check if we have already handled completion
if (_id in completed_objectives) exitWith {};

// complete the task
[_task_name,"succeeded"] call SHK_Taskmaster_upd;

// update the marker
O_MARKER(_this) setMarkerType "Flag";
O_MARKER(_this) setMarkerColor "ColorGreen";

// deactivate and delete the old marker and EOS spawn area
[O_EOS_NAME(_this)] call EOS_Deactivate;
deleteMarker O_EOS_NAME(_this);


// enable any new objectives
_id call FW_fnc_spawnObjectives;

// update the completed and current objectives list
completed_objectives set [count completed_objectives, _id];
publicVariable "completed_objectives";
current_objectives = current_objectives - [_id];
publicVariable "current_objectives";

// set the objective completed flag
server setVariable [format ["objective_%1", _id], true, true];

// check if we have completed all objectives
if (count completed_objectives == count objective_list) then {
    hint "All objectives complete!";
    //endMission "END1";
};

// check if we are spawning counterattacks
if (("FW_RandomCounterAttacks" call BIS_fnc_getParamValue) == 1) then {
    _likely = "FW_CounterAttackLikelihood" call BIS_fnc_getParamValue;
    
    if (_likely > random 100) then {
        // we are being counter attacked!!
        _str = "FW_CounterAttackStrength" call BIS_fnc_getParamValue;
        _nul = [[_task_name],[_str,2],[1,1],[(floor random (_str - 1))],[(floor random (_str - 1)),2],[0,1,EAST],[(floor random 20),1,120,TRUE,FALSE]] call Bastion_Spawn;
    };
};

// check if we have a new spawn point here
if (O_SPAWN(_this)) then {
    // notify
    hint format ["A new spawn point is available at %1", O_DESCRIBE(_this)];
    
    // create a new respawn position
    [WEST, O_POS(_this)] call BIS_fnc_addRespawnPosition;
};

// check if we want to add a new ammo box here
_ammo_mkr = O_AMMO(_this);
if (_ammo_mkr != "") then {
    // place the ammobox
    _nul = [(getMarkerPos _ammo_mkr), 900] spawn FW_fnc_spawnAmmo;    
    _ammo_mkr setMarkerType "n_support";
    hint format ["Ammo is now available from %1", O_DESCRIBE(_this)];
};