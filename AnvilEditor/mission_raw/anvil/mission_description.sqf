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

#include "defines.sqf"

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

/* START AMMO BOX CONFIGURATION */
AFW_ammobox_weapons = [["launch_NLAW_F", 2], ["launch_B_Titan_F", 2], ["launch_B_Titan_short_F", 2]];
publicVariable "AFW_ammobox_weapons";

AFW_ammobox_magazines = [
	["30Rnd_65x39_caseless_mag", 20], ["30Rnd_65x39_caseless_mag_Tracer", 20], ["9Rnd_45ACP_Mag", 10],
	["1Rnd_HE_Grenade_shell", 20], ["200Rnd_65x39_cased_Box", 10], ["200Rnd_65x39_cased_Box_Tracer", 10],
	["100Rnd_65x39_caseless_mag", 10], ["100Rnd_65x39_caseless_mag_Tracer", 10], ["SmokeShell", 10],
	["SmokeShellRed", 10], ["SmokeShellGreen", 10], ["HandGrenade", 10], ["Titan_AT", 10], 
	["Titan_AA", 10], ["NLAW_F", 10], ["DemoCharge_Remote_Mag", 5]
];
publicVariable "AFW_ammobox_magazines";

AFW_ammobox_items = [
	["muzzle_snds_H", 5], ["muzzle_snds_H_MG", 5], ["muzzle_snds_L", 5], 
	["acc_flashlight", 2], ["acc_pointer_IR", 2], ["optic_Arco", 2],
	["optic_Hamr", 2], ["optic_Holosight", 2], ["optic_MRCO", 2], ["FirstAidKit", 10], 
	["Binocular", 5], ["Rangefinder", 5], ["NVGoggles", 3], ["ItemGPS", 5]
];
publicVariable "AFW_ammobox_items";

AFW_ammobox_backpacks = [["B_AssaultPack_khk", 5]];
publicVariable "AFW_ammobox_backpacks";
/* END AMMO BOX CONFIGURATION */