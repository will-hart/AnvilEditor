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

_handle = [] execVM "anvil\mission_description.sqf";
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
} forEach objective_list; 
publicVariable 'incomplete_objectives';

all_objectives_complete = false;
publicVariable 'all_objectives_complete';

// randomly assign mission order if required
if (!isNil "afw_random_objective_order") then {
    if (afw_random_objective_order) then {
        private ["_tmp", "_last", "_lastObj", "_curr", "_currObj", "_i"];
        _tmp = [];

        // copy a list of all objectives (doing it this way as no guarantee
        // that objective ID == array element ID
        _i = 0;
        {
            APPEND(_tmp, _i);
            _i = _i + 1;
        } forEach objective_list;

        // randomly set prerequisites
        _last = 0;
        while {count _tmp > 0} do {
            // select the next random objective
            _curr = _tmp select floor random count _tmp;
            _currObj = EL(objective_list, _curr);

            // set the prerequisite
            if (!(_last == 0)) then {
                _currObj set [12, [O_ID(_lastObj)]];
            };

            // remove the current item from the list and save the previous items
            _last = _curr;
            _lastObj = _currObj;
            _tmp = _tmp - [_curr];
        };
    };
};
publicVariable "objective_list";

// set up random patrols if enabled in the GUI
if ("AFW_NumberRandomPatrols" call BIS_fnc_getParamValue > 0) then {
    _nul = [] spawn AFW_fnc_manageRandomPatrols;
};

// set up mission types - indexed by mission type, elements are 
// [Mission setup function, EOS Callback function, General callback]
mission_types = [
    [AFW_fnc_Mission_Capture,       AFW_fnc_completeObjective,                AFW_fnc_NOP],
    [AFW_fnc_Mission_Intel,                       AFW_fnc_NOP,  AFW_fnc_completeObjective],
    [AFW_fnc_Mission_Assassinate,                 AFW_fnc_NOP,  AFW_fnc_completeObjective],
    [AFW_fnc_Mission_Destroy,                     AFW_fnc_NOP,  AFW_fnc_completeObjective],
    [AFW_fnc_Mission_DestroyAA,                   AFW_fnc_NOP,  AFW_fnc_completeObjective]
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
_nul = AFW_NONE spawn AFW_fnc_spawnObjectives;

// draw respawn positions?
[true] call BIS_fnc_drawRespawnPositions;
