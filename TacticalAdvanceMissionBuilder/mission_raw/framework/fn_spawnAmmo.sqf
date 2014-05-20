#include "defines.sqf"

if (!isServer) exitWith {};

// Creates and refreshes a custom ammo crate
private ["_pos", "_crate"];

_pos = _THIS(0);

// create and place an ammobox
_crate = "I_supplyCrate_F" createVehicle _pos;
_crate setvariable ["BTC_cannot_lift",1,true];
_crate setVariable ["BTC_cannot_drag",1,true];
_crate setVariable ["BTC_cannot_load",1,true];

// fill the ammo box
while {alive _crate} do {
    
    // remove existing
    clearMagazineCargo _crate;
    clearWeaponCargo   _crate;
    clearItemCargo     _crate;
    clearBackpackCargo _crate;
    
    // add new magazines
    _crate addMagazineCargo ["30Rnd_65x39_caseless_mag", 60];
    _crate addMagazineCargo ["30Rnd_65x39_caseless_mag_Tracer", 60];
    _crate addMagazineCargo ["9Rnd_45ACP_Mag", 20];
    _crate addMagazineCargo ["1Rnd_HE_Grenade_shell", 10];
    
    // add some grenades
    _crate addMagazineCargo ["SmokeShell", 10];
    _crate addMagazineCargo ["SmokeShellRed", 10];
    _crate addMagazineCargo ["SmokeShellGreen", 10];
    _crate addMagazineCargo ["HandGrenade", 10];
    
    // add some rockets
    _crate addWeaponCargo   ["launch_NLAW_F", 1];
    _crate addMagazineCargo ["Titan_AT", 5];
    _crate addMagazineCargo ["Titan_AA", 5];
    _crate addMagazineCargo ["NLAW_F", 5];
    
    // add some explosives
    _crate addMagazineCargo ["DemoCharge_Remote_Mag", 5];
    
    // add first aid kits
    _crate addItemCargo     ["FirstAidKit", 10];
    
    // wait
    sleep 600;
};