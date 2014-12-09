/*
	Author: Will Hart

	Description:
	  Spawns an ammo box reward at the given marker. The ammobox refreshes every 10 minutes

	Parameter(s):
	  _this select 0: STRING, the name of the marker where the ammo box should be created

	Example:
	  "ammo_marker_0" call AFW_fnc_spawnAmmo;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

// execute only on clients
if (isDedicated) exitWith { 
	false
};

// Creates and refreshes a custom ammo crate
private ["_pos", "_crate", "_safePos"];
_pos = getMarkerPos _this;

// set up the marker
_this setMarkerType "n_support";

// find a safe position
_safePos = _pos findEmptyPosition [0,10,"I_SupplyCrate_F"];

// update the marker to a safe position
if (count _safePos > 0) then {
	_pos = _safePos;
	_this setMarkerPos _pos;
};

// create and place an ammobox
_crate = "I_supplyCrate_F" createVehicleLocal _pos;
_crate setVariable ["BTC_cannot_lift", 1, true];
_crate setVariable ["BTC_cannot_drag", 1, true];
_crate setVariable ["BTC_cannot_load", 1, true];

diag_log format ["Spawned ammo crate at safe pos %1 for %2", _pos, getMarkerPos _this]; 

while {alive _crate} do {

    // remove existing
    clearMagazineCargo _crate;
    clearWeaponCargo   _crate;
    clearItemCargo     _crate;
    clearBackpackCargo _crate;
    
    // add new magazines
    _crate addMagazineCargo ["30Rnd_65x39_caseless_mag", 20];
    _crate addMagazineCargo ["30Rnd_65x39_caseless_mag_Tracer", 20];
    _crate addMagazineCargo ["9Rnd_45ACP_Mag", 10];
    _crate addMagazineCargo ["1Rnd_HE_Grenade_shell", 20];
    _crate addMagazineCargo ["200Rnd_65x39_cased_Box", 10];
    _crate addMagazineCargo ["200Rnd_65x39_cased_Box_Tracer", 10];
    _crate addMagazineCargo ["100Rnd_65x39_caseless_mag", 10];
    _crate addMagazineCargo ["100Rnd_65x39_caseless_mag_Tracer", 10];
    
    // add some grenades
    _crate addMagazineCargo ["SmokeShell", 10];
    _crate addMagazineCargo ["SmokeShellRed", 10];
    _crate addMagazineCargo ["SmokeShellGreen", 10];
    _crate addMagazineCargo ["HandGrenade", 10];
    
    // add some rockets
    _crate addWeaponCargo   ["launch_NLAW_F", 2];
    _crate addWeaponCargo   ["launch_B_Titan_F", 2];
    _crate addWeaponCargo   ["launch_B_Titan_short_F", 2];
    _crate addMagazineCargo ["Titan_AT", 10];
    _crate addMagazineCargo ["Titan_AA", 10];
    _crate addMagazineCargo ["NLAW_F", 10];
    
    // add some explosives
    _crate addMagazineCargo ["DemoCharge_Remote_Mag", 5];
    
    // add first aid kits
    _crate addItemCargo     ["FirstAidKit", 10];

    // add binos etc
    _crate addItemCargo     ["Binocular", 5];
    _crate addItemCargo     ["Rangefinder", 5];

    // add backpacks
    _crate addBackpackCargo ["B_AssaultPack_khk", 5]
    
    // wait
    sleep 600;
	diag_log format["Refreshing ammo crate at %1", _pos];
};
