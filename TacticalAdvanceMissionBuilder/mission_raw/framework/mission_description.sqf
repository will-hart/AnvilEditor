/*
	Author: Will Hart

	Description:
	  Sets up the mission, defining the objectives and a few required variables.

	Parameter(s):
	  None

	Example:
	  _nul = [] execVM "framework\framework_init.sqf";
	
	Returns:
	  Nothing
*/



/*
 * Set up the objectives. The format of each line is:
 * [id, description, marker name, radius, infantry_strength, vehicle_strength, armour_strength, air_strength, troop_strength, new spawn point?, ammo spawn point marker, special weapons spawn marker,pre-requisites, mission type, reward description]
 */
/* START OBJECTIVE LIST */
objective_list = [];
publicVariable 'objective_list';
/* END OBJECTIVE LIST */

/* START MISSION DATA */
enemyTeam = EAST;
publicVariable "enemyTeam";
/* END MISSION DATA */