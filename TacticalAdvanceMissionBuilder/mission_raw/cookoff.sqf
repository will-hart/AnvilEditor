private ['_target', '_sound'];

_target = _this select 0;

if (count(_target weaponsTurret [0]) > 0 && count(magazines _target) > 0) then {

	// don't blow up underwater
	if (surfaceIsWater position _target) exitWith {};
	
	sleep random 3;
	
	_sound = ['.ogg', '1.ogg', '2.ogg'] select floor random 3;
	
	playSound3D [MISSION_ROOT + "sounds\cookoff" + _sound, _target, false, getPos _target, 60, 1, 250];
	
	sleep 2;
	
	_source01 = "#particlesource" createVehicleLocal (position _target);
	_source01 setParticleClass "ExpSparks";
	_source01 attachTo [_target,[0,0,0]];
	
	sleep 6;
	deleteVehicle _source01;
};