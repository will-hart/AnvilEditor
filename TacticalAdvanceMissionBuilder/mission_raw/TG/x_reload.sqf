// by Xeno - modified by ghost, added delay of 4 seconds if object moves out then no repair
private ["_config","_count","_i","_magazines","_object","_type","_type_name"];

_object = _this select 0;
_Rearmlist = _this select 1;

_type = typeof _object;

if ((_object isKindOf "ParachuteBase") or (_object isKindOf "Man")) exitWith {};

WaitUntil{_object in list _Rearmlist};
sleep 4;
if(!(_object in list _Rearmlist)) exitWith {_object vehiclechat "Repair Aborted"};

if (isNil "x_reload_time_factor") then {x_reload_time_factor = 1;};

//if (!local _object) exitWith {};

if (!alive _object) exitWith {};
_fuelstore = fuel _object;
_object setFuel 0;
_object setVehicleAmmo 1;	// Reload turrets / drivers magazine

_Object_name = (getText (configFile >> "cfgVehicles" >> (_type) >> "displayName"));

_object vehicleChat format ["Servicing %1... Please stand by...", _Object_name];

_magazines = getArray(configFile >> "CfgVehicles" >> _type >> "magazines");

if (count _magazines > 0) then {
	_removed = [];
	{
		if (!(_x in _removed)) then {
			_object removeMagazines _x;
			_removed set [count _removed, _x];
		};
	} forEach _magazines;
	{
		_object vehicleChat format ["Reloading %1", _x];
		sleep x_reload_time_factor;
		if (!alive _object) exitWith {};
		_object addMagazine _x;
		sleep 1;
	} forEach _magazines;
};

_count = count (configFile >> "CfgVehicles" >> _type >> "Turrets");

if (_count > 0) then {
	for "_i" from 0 to (_count - 1) do {
		scopeName "xx_reload2_xx";
		_config = (configFile >> "CfgVehicles" >> _type >> "Turrets") select _i;
		_magazines = getArray(_config >> "magazines");
		_removed = [];
		{
			if (!(_x in _removed)) then {
				_object removeMagazines _x;
				_removed set [count _removed, _x];
			};
		} forEach _magazines;
		{
			_object vehicleChat format ["Reloading %1", _x];
			sleep x_reload_time_factor;
			if (!alive _object) then {breakOut "xx_reload2_xx"};
			_object addMagazine _x;
			sleep x_reload_time_factor;
			if (!alive _object) then {breakOut "xx_reload2_xx"};
			sleep 1;
		} forEach _magazines;
		// check if the main turret has other turrets
		_count_other = count (_config >> "Turrets");
		// this code doesn't work, it's not possible to load turrets that are part of another turret :(
		// nevertheless, I leave it here
		if (_count_other > 0) then {
			for "_i" from 0 to (_count_other - 1) do {
				_config2 = (_config >> "Turrets") select _i;
				_magazines = getArray(_config2 >> "magazines");
				_removed = [];
				{
					if (!(_x in _removed)) then {
						_object removeMagazines _x;
						_removed set [count _removed, _x];
					};
				} forEach _magazines;
				{
					_object vehicleChat format ["Reloading %1", _x]; 
					sleep x_reload_time_factor;
					if (!alive _object) then {breakOut "xx_reload2_xx"};
					_object addMagazine _x;
					sleep x_reload_time_factor;
					if (!alive _object) then {breakOut "xx_reload2_xx"};
					sleep 1;
				} forEach _magazines;
			};
		};
	};
};

_object setVehicleAmmo 1;	// Reload turrets / drivers magazine

sleep x_reload_time_factor;
if (!alive _object) exitWith {};
_object vehicleChat "Repairing...";
_object setDamage 0;
sleep x_reload_time_factor;
if (!alive _object) exitWith {};
_object vehicleChat "Refueling...";
_f = 0.01;
while {fuel _object < 0.99} do {
	if(!(_object in list _Rearmlist)) exitWith {_object vehicleChat "Fueling Aborted";	_object setFuel _fuelstore + _f;};
	_object setFuel _fuelstore + _f;
	sleep 0.1;
	_f = _f + 0.01;
};
sleep x_reload_time_factor;
if (!alive _object) exitWith {};
_object vehicleChat format ["%1 is ready...", _Object_name];

if (true) exitWith {};
