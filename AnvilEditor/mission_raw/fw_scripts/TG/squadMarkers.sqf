/*
	AUTHOR: |TG| B for tacticalgamer.com. Feel free to use, modify and improve with credits intact.
	CREDIT: Butler for ideas and inspiration. Aeroson for code segments.
	
	VERSION: 2014_11_09
	
	USAGE:
		in (client's) init do (initPlayerLocal.sqf):
		0 = [] execVM 'squad_marker.sqf';
		
		or more efficient way would be:

		squadMarker = compile preprocessfilelinenumbers "squadMarker.sqf";
		[] spawn squadMarker;
*/

if (isDedicated) exitWith {}; //do not run it on the dedicated server

//Some private variable declared
private ["_marker","_markerNumber", "_markerName", "_getNextMarker","_vehicle", "_str", "_txt"];

//Helping function to create or update position of the marker.
_getNextMarker = 
{
	private ["_marker"];
	
	_markerNumber = _markerNumber + 1;
	_marker = format["um%1",_markerNumber];
	
	if(getMarkerType _marker == "") then 
	{
		createMarkerLocal [_marker, _this];
	} else 
	{
		_marker setMarkerPosLocal _this;
	};
	_marker;
};

while {true} do
{
	waitUntil {
		sleep 1;
		//true;
		(visibleMap or visibleGPS);
	};
	
	
	_markerNumber = 0;
	
	{ // forEach start
		if (side _x == side player) then 		// make sure they are on the same side
		{
			if (leader player == player) then 		// if the player is the leader of the squad
			{
				switch true do
				{			
					case (leader _x == _x and _x != player): 			//in this case make markers for each other leaders
					{					
						_vehicle = vehicle _x;
						_pos = getPosATL _vehicle;						
						_marker = _pos call _getNextMarker;
						_marker setMarkerColorLocal "ColorRed";						
						
						_txt = format["%1 [%2]",(name _x),(groupID (group _x))];					
					
						if(!visibleGPS || visibleMap) then{_marker setMarkerTextLocal _txt;} else {_marker setMarkerTextLocal "";};
						_marker setMarkerSizeLocal [0.5, 0.5];
						_marker setMarkerTypeLocal "n_inf";					
					};
					case (group _x == group player):
					{
						_vehicle = vehicle _x;
						_pos = getPosATL _vehicle;					
						_marker = _pos call _getNextMarker;
						if (_x == player) then {_marker setMarkerColorLocal "ColorBlue";} else {_marker setMarkerColorLocal "ColorGreen";};
						
						if(_vehicle != _x) then	//if the _x is in a vehicle create a listed names in there.
						{
							_str = "";
							_txt = "";
							
							{
								if(group _x == group player) then 
								{								
									if(_foreachindex == ((count (crew (vehicle _x))) - 1)) then {_str = format["%1",name _x];} 	else { _str = format["%1, ",name _x];};
									_txt = _txt + _str;
								};
							} forEach crew (vehicle _x);
						}
						else {_txt = name _x;};
						
						if(!visibleGPS || visibleMap) then{_marker setMarkerTextLocal _txt;} else {_marker setMarkerTextLocal "";};
						_marker setMarkerSizeLocal [0.5, 0.5];						
						_marker setMarkerDirLocal getDir _vehicle;						
						if (_x == player) then {_marker setMarkerTypeLocal "mil_arrow2";} else {_marker setMarkerTypeLocal "mil_arrow";};
						
					};					
				};
			}
			else
			{			
				switch true do
				{
					case (group _x == group player && _x == leader player): //in this case only make a marker for the SL
					{
						_vehicle = vehicle _x;
						_pos = getPosATL _vehicle;
						_marker = _pos call _getNextMarker;
				
						if(_vehicle != _x) then	//if the _x is in a vehicle create a listed names in there.
						{
							_str = "";
							_txt = "";					
							{
								if(group _x == group player) then
								{								
									if(_foreachindex == ((count (crew (vehicle _x))) - 1)) then {_str = format["%1",name _x];} 	else { _str = format["%1, ",name _x];};
									_txt = _txt + _str;
								};
							} forEach crew (vehicle _x);
						}
						else {_txt = name _x;};		
				
						if(!visibleGPS || visibleMap) then{_marker setMarkerTextLocal _txt;} else {_marker setMarkerTextLocal "";};
						_marker setMarkerSizeLocal [0.5, 0.5];
						_marker setMarkerDirLocal getDir _vehicle;
						_marker setMarkerColorLocal "ColorBlue";
						_marker setMarkerTypeLocal "mil_arrow2";
						
					};
					case (group _x == group player && _x != leader player):
					{
						_vehicle = vehicle _x;
						_pos = getPosATL _vehicle;
						_marker = _pos call _getNextMarker;
				
						if(_vehicle != _x) then	//if the _x is in a vehicle create a listed names in there.
						{
							_str = "";
							_txt = "";					
							{
								if(group _x == group player) then
								{								
									if(_foreachindex == ((count (crew (vehicle _x))) - 1)) then {_str = format["%1",name _x];} 	else { _str = format["%1, ",name _x];};
									_txt = _txt + _str;
								};
							} forEach crew (vehicle _x);
						}
						else {_txt = name _x;};						
						if(!visibleGPS || visibleMap) then{_marker setMarkerTextLocal _txt;} else {_marker setMarkerTextLocal "";};
						_marker setMarkerSizeLocal [0.5, 0.5];
						_marker setMarkerColorLocal "ColorGreen";						
						_marker setMarkerDirLocal getDir _vehicle;
						_marker setMarkerTypeLocal "mil_arrow";
					};
				};				
			};
		};		
	} forEach playableUnits;
	
	//clean up?
	
	_markerNumber = _markerNumber + 1;
	_marker = format["um%1",_markerNumber];	
	
	while {(getMarkerType _marker) != ""} do {	
		deleteMarkerLocal _marker;
		_markerNumber = _markerNumber + 1;
		_marker = format["um%1",_markerNumber];
	};	
};