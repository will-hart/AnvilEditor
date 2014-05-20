/*Class based ammo box by B. The class based ammobox is created on client side.
It replenishes every _timer seconds.

Usage: 
1. Must have a marker with a name where you want the ammobox to appear.
2. Add the following line into the init field of the playable soldier.
*/
waitUntil { !isNil {player} };
waitUntil { player == player };
if (_this != player) exitWith {};  // exit all other clients.

_timer = 1200;
_marker = "ammobox";
_marker1 = "clothes";
_marker2 = "side_box";
_dir = 0;

//default items, weapons and backpacks
_weapons    = ["arifle_MXC_Black_F"];
				
_magazines  = [	"30Rnd_65x39_caseless_mag"];
				
_items      = [	"optic_Holosight",
				"optic_ACO_red",
				"muzzle_snds_H",				
				"acc_pointer_IR",
				"acc_flashlight"];

_backpacks 	= [];    
                
_clothes	= [	"H_Cap_headphones",
				"H_HelmetIA",
                "NVGoggles",
				"G_Combat",
				"G_Tactical_Clear",
                "V_TacVest_blk",
				"V_PlateCarrier1_blk"];
				
_sidewpns	= ["hgun_ACPC2_snds_F", "launch_NLAW_F", "Binocular"];
_sideitms   = [];
_sidemags 	= ["9Rnd_45ACP_Mag",
                "HandGrenade",
                "MiniGrenade",
				"SmokeShell", 
				"SmokeShellRed", 
				"SmokeShellGreen",
                "I_IR_Grenade",
				"SmokeShell", 
				"SmokeShellRed", 
				"SmokeShellGreen",
                "Chemlight_green",
                "Chemlight_red",
				"DemoCharge_Remote_Mag", 
                "NLAW_F"];
                
_fac_count 	= 1;

_leaders    = ["B_Soldier_SL_F", "B_Soldier_TL_F", "I_officer_F", "I_Soldier_SL_F", "I_Soldier_TL_F"];
_medics     = ["B_medic_F", "I_medic_F"];
_ats        = ["B_soldier_AT_F", "I_Soldier_AT_F"];
_ars        = ["B_soldier_AR_F", "I_Soldier_AR_F"];
_engis      = ["B_engineer_F", "I_engineer_F"];
_crews      = ["I_crew_F", "B_crew_F"];
_helos      = ["I_helipilot_F"];
_jets       = ["I_pilot_F", "B_Pilot_F"];
_uavs       = ["B_soldier_UAV_F", "I_soldier_UAV_F"];
_snipers    = ["B_sniper_F", "I_Sniper_F"];
_spotters   = ["B_spotter_F", "I_Spotter_F"];
_marksmen   = ["I_Soldier_M_F"];
_mules      = ["I_Soldier_A_F"];


//specialized weapons based on class. classnames
_type = typeOf player;
switch (true) do
{
	case (_type IN _medics): 
    {
		_items     = _items     + ["Medikit"];		
        //_backpacks 	= ["B_BergenC_grn"];
        _fac_count = 10;
	};
    case (_type IN _leaders): 
    {        
        _weapons   = _weapons   + ["arifle_Mk20_GL_F"];
        _sidewpns  = _sidewpns  + ["Rangefinder", "Laserdesignator"];
        _sidemags = _sidemags   + ["Laserbatteries"];
		_magazines  = _magazines + ["1Rnd_HE_Grenade_shell", "1Rnd_SmokeRed_Grenade_shell", "1Rnd_SmokeGreen_Grenade_shell", "UGL_FlareWhite_F"];
	};	
	case (_type IN _ats): 
    {
		_sidewpns = _sidewpns + ["launch_I_Titan_short_F"];
        _sidemags  = _sidemags  + ["Titan_AP", "Titan_AT"];
    };
	case (_type IN _ars): 
    {
		_weapons    = ["LMG_Mk200_F"];
		_magazines  = ["200Rnd_65x39_cased_Box", "200Rnd_65x39_cased_Box_Tracer"];
		_items      = _items     + ["muzzle_snds_H_MG"];
    };	
	case (_type IN _engis): 
    {    
        _backpacks 	= ["B_Kitbag_rgr"];
		_magazines = _magazines + ["SatchelCharge_Remote_Mag", "APERSMine_Range_Mag", "SLAMDirectionalMine_Wire_Mag"];
		_items     = _items     + ["ToolKit", "MineDetector"];		
    };
    case (_type IN _mules): 
    {
        _fac_count 	= 10;
        _backpacks 	= ["B_Carryall_oli"];
		_magazines = _magazines + ["200Rnd_65x39_cased_Box"];
        _sidemags = _sidemags + ["Titan_AT"];
    };
    case (_type IN _crews): 
    {
		_weapons    = ["arifle_Mk20C_ACO_F"];
        _magazines  = ["30Rnd_556x45_Stanag", "30Rnd_556x45_Stanag_Tracer_Yellow"];
        _items      = ["NVGoggles_INDEP"];
        _backpacks 	= [];
        _clothes	= ["V_BandollierB_oli", "H_HelmetCrew_I","G_Combat","G_Aviator","G_Spectacles",	"G_Sport_Blackred",	"G_Tactical_Clear"];
        _sidewpns	= ["hgun_ACPC2_snds_F"];
        _sideitms   = [];
        _sidemags 	= ["9Rnd_45ACP_Mag"];
    };
   	case (_type IN _helos): 
    {
		_weapons    = ["hgun_PDW2000_Holo_F"];
        _magazines  = ["30Rnd_9x21_Mag"];
        _items      = ["NVGoggles_INDEP"];
        _backpacks 	= [];
        _clothes	= ["V_TacVest_oli", "H_CrewHelmetHeli_I", "H_PilotHelmetHeli_I"];				
        _sidewpns	= ["hgun_ACPC2_snds_F"];
        _sideitms   = [];
        _sidemags 	= ["9Rnd_45ACP_Mag"];
    };
    case (_type IN _uavs): 
    {
        _clothes    = _clothes   + ["I_UavTerminal"];
        _backpacks 	= _backpacks + ["I_UAV_01_backpack_F"];
    };
    case (_type IN _snipers): 
    {        
        _weapons   = _weapons   + ["srifle_GM6_F"];
        _items     = _items     + ["optic_LRPS", "optic_SOS"];
        _sidewpns  = _sidewpns  + ["Rangefinder", "Laserdesignator"];
        _sidemags = _sidemags   + ["Laserbatteries"];
		_magazines  = _magazines + ["5Rnd_127x108_Mag", "5Rnd_127x108_APDS_Mag"];
	};	
    case (_type IN _spotters): 
    {
        _items     = _items     + ["optic_tws"];
        _sidewpns  = _sidewpns  + ["Rangefinder", "Laserdesignator"];
        _sidemags = _sidemags   + ["Laserbatteries"];		
	};	
    case (_type IN _marksmen): 
    {        
        _weapons   = _weapons   + ["srifle_EBR_F"];
        _items     = _items     + ["optic_DMS", "muzzle_snds_B"];
        _sidewpns  = _sidewpns  + ["Rangefinder", "Laserdesignator"];
        _sidemags = _sidemags   + ["Laserbatteries"];
		_magazines  = _magazines + ["20Rnd_762x51_Mag"];
        
	}; 

    default {false};
};

_box = "Box_IND_AmmoVeh_F" createVehicleLocal (getMarkerPos _marker); //Place this vehicle(box) on the marker called "ammobox".
_box1 = "I_supplyCrate_F" createVehicleLocal (getMarkerPos _marker1); //Place this vehicle(box) on the marker called "ammobox".
_box2 = "Box_IND_AmmoVeh_F" createVehicleLocal (getMarkerPos _marker2);

_box setDir _dir; // Sets the direction of the box
_box1 setDir _dir; // Sets the direction of the box
_box2 setDir _dir; // Sets the direction of the box

_box allowDamage false; // Sets the direction of the box
_box1 allowDamage false; // Sets the direction of the box
_box2 allowDamage false; // Sets the direction of the box

player reveal _box;
player reveal _box1;
player reveal _box2;
//refill the box every _timer seconds
while {alive _box} do 
{
    clearMagazineCargo _box;
    clearWeaponCargo   _box;
    clearItemCargo     _box;
	clearBackpackCargo _box;
	
	clearMagazineCargo _box1;
    clearWeaponCargo   _box1;
    clearItemCargo     _box1;
	clearBackpackCargo _box1;    
	
	clearMagazineCargo _box2;
    clearWeaponCargo   _box2;
    clearItemCargo     _box2;
	clearBackpackCargo _box2;
    
    {_box addWeaponCargo   [_x, 1];} foreach _weapons;
    {_box addMagazineCargo [_x, 12];} foreach _magazines;
    {_box addItemCargo     [_x, 1];} foreach _items;

	_box addItemCargo     ["FirstAidKit", _fac_count];
	
	//clothes	
    {_box1 addItemCargo     [_x, 1];} foreach _clothes;
    {_box1 addBackpackCargo [_x, 1];} foreach _backpacks;
	
	//launchers
	{_box2 addWeaponCargo   [_x, 1];} foreach _sidewpns;
    {_box2 addItemCargo   [_x, 1];} foreach _sideitms;
    {_box2 addMagazineCargo [_x, 8];} foreach _sidemags;
	
	sleep _timer;
};