/*
	Author: Will Hart

	Description:
	  Spawns, despawns and manages random patrols moving throughout the AO. The number of patrols
	  (if any) can be controlled through mission paramters. Updates the patrol state every 180 seconds

	Parameter(s):
	  None

    Example:
	  _nul = [] spawn AFW_fnc_manageRandomPatrols;
	  	
	Returns:
	  Nothing
*/

#include "defines.sqf"

if (!isServer) exitWith {};

private ["_patrol_count", "_patrols", "_new_patrols", "_destinations", "_sources", "_grp", "_pos", "_patrol", "_markers", "_mkr_name", "_ldr", "_mkr", "_num_patrols", "_debug", "_wp", "_faction"];

// loop for the whole game, but update infrequently
_patrol_count = "AFW_NumberRandomPatrols" call BIS_fnc_getParamValue;
_patrols = [];
_markers = [];
_debug = false; // change to false for production

waitUntil { sleep 2; (count objective_list) > 0 };

// determine which units to spawn
if (enemyTeam == WEST) then {
	_faction = 1;
} else {
	if (enemyTeam == EAST) then {
		_faction = 0;
	} else {
		_faction = 2;
	};
};

while {true} do {
    diag_log "Updating random patrols";

    // start by checking if we have any patrols that can be removed
    _new_patrols = [];
    {
        if (({alive _x} count units _x) == 0) then {
            deleteGroup _x;
            diag_log "Removing destroyed patrol group";
        } else {
            _new_patrols set [count _new_patrols, _x];
        };
    } forEach _patrols;
    _patrols = _new_patrols;
    
	if (_debug) then {
		// update all the markers
		{
			deleteMarker _x;
		} forEach _markers;
		_markers = [];
		
		// create a marker for each group
		_i = 0;
		{
			_mkr_name = format ["patrol_%1", _i];
			APPEND(_markers, _mkr_name);
			_ldr = leader _x;
			
			if (typeOf _ldr != typeOf objNull) then {
				_mkr = createMarker [_mkr_name, getPos leader _x];
				_mkr setMarkerShape "ELLIPSE";
				_mkr setMarkerColor "ColorBlack";
				_mkr setMarkerSize [50, 50];
				_mkr setMarkerAlpha 1.0;
			};
			
			_i = _i + 1;
		} forEach _patrols;
	};
		
    // check if we need to spawn a new patrol
    _num_patrols = count _patrols;
    diag_log format ["There are %1 patrols active on the map", _num_patrols];
    
    if (_num_patrols < _patrol_count and (count incomplete_objectives) > 0 ) then {
        diag_log format ["Adding an additional patrol on %1 side", enemyTeam];
        
        // find all the incomplete or current objectives and select a random destination
		_sources = current_objectives + incomplete_objectives;
		_destinations = current_objectives + completed_objectives;
		
		if (count _sources == 0) then {
			diag_log "Unable to spawn any patrols - no source objectives available";
		} else { 
			if (count _destinations == 0) then {
				diag_log "Unable to spawn any patrols - no destination objectives available";
			} else {
				_src = objective_list select (_sources call BIS_fnc_selectRandom);
				_dst = objective_list select (_destinations call BIS_fnc_selectRandom);
				_pos = [O_POS(_src), random 360, 50, true, 1] call SHK_pos;
			
				// get a random destination point
				if (isNil "_dst") then {
					diag_log "Ignoring attempt to patrol to empty destination";
				} else {
					diag_log format ["Creating patrol from objective %1 to objective %2", _src, _dst];
					
					if (_debug) then { 
						// create a marker at the source
						_mkr_name = format ["patrol_%1", _num_patrols];
						_mkr = createMarker [_mkr_name, _pos];
						APPEND(_markers, _mkr_name);
					
						_mkr setMarkerShape "ELLIPSE";
						_mkr setMarkerColor "ColorBlack";
						_mkr setMarkerSize [20, 20];
						_mkr setMarkerAlpha 1.0;
						diag_log " -> created source marker";
					};
					
					// spawn a group and set it on the patrol route. When it gets to the end it will cycle
					_patrol = [_pos, [floor random 3, 3], _faction, enemyTeam] call EOS_fnc_spawngroup;
					diag_log " -> created patrol";
					
					//diag_log " -> set patrol waypoints";
					_wp = _patrol addWaypoint [O_POS(_dst), 0];
					_wp setWaypointStatements ["true", "[group this] spawn AFW_fnc_setPatrolDestination;"];
					
					// add the patrol to the list of patrols
					APPEND(_patrols, _patrol);
				};
			};
		};
    };
    
    diag_log "Done spawning random patrols";
    
	if (_debug) then {
		for [{_i=0}, {_i<20}, {_i=_i+1}] do
		{
			sleep 15;
			
			_i = 0;
			{
				_mkr_name = format ["patrol_%1", _i];
				_ldr = leader _x;
				_mkr_name setMarkerPos (getPos _ldr);
				_i = _i + 1;
			} forEach _patrols;
			
		};
	} else {
		sleep 300;
	};
};