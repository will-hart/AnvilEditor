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
 * [id, description, marker name, radius, infantry_strength, vehicle_strength, armour_strength, air_strength, troop_strength, new spawn point?, ammo spawn point marker, special weapons spawn marker,pre-requisites, mission type, reward description, randomise placement, loss prerequisities, generic data payload]
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

/* START EOS CONFIGURATION */
AFW_spawn_configuration = [[[
	["O_SoldierU_SL_F","O_soldierU_repair_F","O_soldierU_medic_F","O_sniper_F","O_Soldier_A_F","O_Soldier_AA_F","O_Soldier_AAA_F","O_Soldier_AAR_F","O_Soldier_AAT_F","O_Soldier_AR_F","O_Soldier_AT_F","O_soldier_exp_F","O_Soldier_F","O_engineer_F","O_engineer_U_F","O_medic_F","O_recon_exp_F","O_recon_F","O_recon_JTAC_F","O_recon_LAT_F","O_recon_M_F","O_recon_medic_F","O_recon_TL_F"],
	["O_APC_Tracked_02_AA_F","O_APC_Tracked_02_cannon_F","O_APC_Wheeled_02_rcws_F","O_MBT_02_arty_F","O_MBT_02_cannon_F"],
	["O_Truck_02_covered_F","O_Truck_02_transport_F","O_MRAP_02_F","O_MRAP_02_gmg_F","O_MRAP_02_hmg_F","O_Truck_02_medical_F"],
	["O_Heli_Attack_02_black_F","O_Heli_Attack_02_F"],
	["O_Heli_Light_02_F","O_Heli_Light_02_unarmed_F"],
	["O_UAV_01_F","O_UAV_02_CAS_F","O_UGV_01_rcws_F"],
	["O_Mortar_01_F","O_static_AT_F","O_static_AA_F"],
	["O_Boat_Armed_01_hmg_F","O_Boat_Transport_01_F"],
	["O_diver_exp_F","O_diver_F","O_diver_TL_F"],
	["O_crew_F"],
	["O_helicrew_F","O_helipilot_F"]
]]];
publicVariable "AFW_spawn_configuration";
/* END EOS CONFIGURATION */