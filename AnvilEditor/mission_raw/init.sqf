
// load up the briefing
_nul = [] execVM "briefing.sqf";
diag_log "Executed briefing";

// setup the required scripts
/* START ADDITIONAL SCRIPTS */
/* END ADDITIONAL SCRIPTS */

_nul = [] execVM "eos\OpenMe.sqf";
diag_log "Set up required scripts";

// start up the mission framework
sleep 5;
_nul = [] execVM "anvil\framework_init.sqf";
diag_log "Started mission framework";