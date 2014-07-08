/*
	Author: Will Hart

	Description:
	  Initialises a mission using the Tactical Advance framework and defines the list of objectives that should be created

	Parameter(s):
	  None

	Example:
	  _nul = [] execVM "framework\framework_init.sqf";
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_handle"];

_handle = [] execVM "framework\mission_description.sqf";
waitUntil {scriptDone _handle};
diag_log "Loaded mission description";

completed_objectives = [];
publicVariable 'completed_objectives';

current_objectives = [];
publicVariable 'current_objectives';

// record all objectives as incomplete
incomplete_objectives = [];
{
    incomplete_objectives set [count incomplete_objectives, O_ID(_x)];
    server setVariable [O_OBJ_NAME(_x), false, true];
} foreach objective_list; 
publicVariable 'incomplete_objectives';

all_objectives_complete = false;
publicVariable 'all_objectives_complete';

// set up random patrols if enabled in the GUI
if ("FW_NumberRandomPatrols" call BIS_fnc_getParamValue > 0) then {
    _nul = [] spawn FW_fnc_manageRandomPatrols;
};

// set up mission types - indexed by mission type, elements are 
// [Mission setup function, EOS Callback function, General callback]
mission_types = [
    [FW_fnc_Mission_Capture,       FW_fnc_completeObjective,                FW_fnc_NOP],
    [FW_fnc_Mission_Intel,                       FW_fnc_NOP,  FW_fnc_completeObjective],
    [FW_fnc_Mission_Assassinate,                 FW_fnc_NOP,  FW_fnc_completeObjective],
    [FW_fnc_Mission_Destroy,                     FW_fnc_NOP,  FW_fnc_completeObjective],
    [FW_fnc_Mission_DestroyAA,                   FW_fnc_NOP,  FW_fnc_completeObjective]
];
publicVariable "mission_types";

support_weapons = [
    ['B_UAV_01_backpack_F'],
    ['B_Kitbag_rgr_Exp'],
    ['B_Mortar_01_support_F', 'B_Mortar_01_weapon_F'],
    ['B_Kitbag_rgr_Exp'],
    ['B_HMG_01_support_F','B_HMG_01_weapon_F']
];
publicVariable "support_weapons";

// set up missions
_nul = FW_NONE spawn FW_fnc_spawnObjectives;

// draw respawn positions?
[true] call BIS_fnc_drawRespawnPositions;
