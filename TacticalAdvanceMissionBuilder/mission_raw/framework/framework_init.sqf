#include "defines.sqf"

if (!isServer) exitWith {};

/*
 * Set up the objectives. The format of each line is:
 * [id, description, radius, marker name, infantry_strength, vehicle_strength, armour_strength, air_strength, troop_strength, new spawn point?, ammo spawn point marker, special weapons spawn marker,pre-requisites, reward description]
 */
$$$OBJECTIVELIST$$$

completed_objectives = [];
publicVariable 'completed_objectives';

current_objectives = [];
publicVariable 'current_objectives';

incomplete_objectives = [];
{
    incomplete_objectives set [count incomplete_objectives, O_ID(_x)];
} foreach objective_list; 
publicVariable 'incomplete_objectives';

// set all the completed objective flags to false
for "_i" from 0 to (count completed_objectives) do
{
    server setVariable [format ["objective_%1", _i], false, true];
};

// set up missions
_nul = FW_NONE spawn FW_fnc_spawnObjectives;

// set up random patrols if enabled in the GUI
if ("FW_NumberRandomPatrols" call BIS_fnc_getParamValue > 0) then {
    _nul = [] spawn FW_fnc_manageRandomPatrols;
};

// TODO set up side missions
