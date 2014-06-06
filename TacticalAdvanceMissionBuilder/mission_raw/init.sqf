MISSION_ROOT = call {
    private "_arr";
    _arr = toArray str missionConfigFile;
    _arr resize (count _arr - 15);
    toString _arr
};
publicVariable 'MISSION_ROOT';

{
	_x addEventHandler ["Killed", {_nul = [_this select 0] execVM "cookoff.sqf";}];
	diag_log "Connected cookoff script for an existing vehicle";
} forEach vehicles;

// load up the briefing
_nul = [] execVM "briefing.sqf";
diag_log "Executed briefing";

// setup the required scripts
_nul = [] execVM "eos\OpenMe.sqf";
diag_log "Set up required scripts";

// start up the mission framework
sleep 5;
_nul = [] execVM "framework\framework_init.sqf";
diag_log "Started mission framework";