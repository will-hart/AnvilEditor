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
    
    // add weapons specified in mission_description.sqf
    {
        _crate addWeaponCargo _x;
    } forEach AFW_ammobox_weapons;

    // add magazines specified in mission_description.sqf
    {
        _crate addMagazineCargo _x;
    } forEach AFW_ammobox_magazines;

    // add items specified in mission_description.sqf
    {
        _crate addItemCargo _x;
    } forEach AFW_ammobox_items;

    // add backpacks specified in mission_description.sqf
    {
        _crate addBackpackCargo _x;
    } forEach AFW_ammobox_backpacks;
    
    // wait
    sleep 600;
    diag_log format["Refreshing ammo crate at %1", _pos];
};
