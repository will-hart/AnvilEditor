if (!isServer) exitWith {};

diag_log "ANVIL ZEUS - MANAGE ALL ENEMY SPAWNS INIT";

// init
{zeusCurator addCuratorEditableObjects [[_x], true]} forEach vehicles;
{zeusCurator addCuratorEditableObjects [[_x], true]} forEach (allMissionObjects "Man");

// loop for EOS spawns
while {true} do {
	sleep 30;

	{
		if (side _x == enemyTeam) then {
			zeusCurator addCuratorEditableObjects [[_x], true];
		};
	} forEach allUnits;
};