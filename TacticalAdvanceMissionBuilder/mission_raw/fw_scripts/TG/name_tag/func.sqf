fn_createLabels = {
	_idx = addMissionEventHandler ["Draw3D", {
		{	
			if(side _x == side player) then {
				if(leader player == player) then {
					if(_x != player) then {
						if(vehicle _x == _x) then {
							_playerPos = visiblePosition _x;
							_dist = player distance _x;
							_dir = ([player, _x] call BIS_fnc_relativeDirTo) + 180;
							_h = (_playerPos select 2) + ((_dist/30) + 2);
							_fade = (((_dist - 60) - 11) * (-1) )/10;
							if(_dist < 60) then {
								if (leader _x == _x) then {
									_str = format["%1 [%2]",(name _x),(groupID (group _x))];
									drawIcon3D ["", [1, 1, 0, 1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, _str, 2, 0.03, "PuristaLight" ];
								} else {
									if(group _x == group player) then {
										drawIcon3D ["", [0,1,0,1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];
									};
								};
							};
							if(_dist >= 60 && _dist <= 75) then {
								if (leader _x == _x) then {
									_str = format["%1 [%2]",(name _x),(groupID (group _x))];
									drawIcon3D ["", [1, 1,0 ,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir,_str, 2, 0.03, "PuristaLight" ];
								} else {	
									if(group _x == group player) then {
										drawIcon3D ["", [0,1,0,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];
									};
								};				
							};
						} else {
							_str = "";
							_txt = "";
							{
								if(group _x == group player) then { 
									if(_foreachindex == ((count (crew (vehicle _x))) - 1)) then { 
										_str = format["%1",name _x];
									} else { 
										_str = format["%1, ",name _x];
									};
									_txt = _txt + _str;
								};
							} forEach crew (vehicle _x);
							_playerPos = visiblePosition (vehicle _x);
							_dist = player distance (vehicle _x);
							_dir = ([player, (vehicle _x)] call BIS_fnc_relativeDirTo) + 180;
							_h = (_playerPos select 2) + ((_dist/30) + 3);
							_fade = (((_dist - 60) - 11) * (-1) )/10;
							if(_dist < 60) then {
								drawIcon3D ["", [0,1,0,1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, _txt, 2, 0.035, "PuristaLight" ];
							};
							if(_dist >= 60 && _dist <= 75) then {
								drawIcon3D ["", [0,1,0,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, _txt, 2, 0.035, "PuristaLight" ];
							};
						};
					};
				} else {
					if(group _x == group player) then {
						if(_x != player) then {
							if(vehicle _x == _x) then {
								_playerPos = visiblePosition _x;
								_dist = player distance _x;
								_dir = ([player, _x] call BIS_fnc_relativeDirTo) + 180;
								_h = (_playerPos select 2) + ((_dist/30) + 2);
								_fade = (((_dist - 30) - 11) * (-1) )/10;
								if(_dist < 30) then {
									if (_x == leader player) then {
										drawIcon3D ["", [1, 0.5, 0, 1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];
									} else {			
										drawIcon3D ["", [0,1,0,1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];
									};
								};
								if(_dist >= 30 && _dist <= 45) then {
									if (_x == leader player) then {
										drawIcon3D ["", [1, 0.5,0 ,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];
									} else {			
										drawIcon3D ["", [0,1,0,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, name _x, 2, 0.03, "PuristaLight" ];			
									};				
								};
							} else {
								_str = "";
								_txt = "";
								{
									if(group _x == group player) then { 
										if(_foreachindex == ((count (crew (vehicle _x))) - 1)) then { 
											_str = format["%1",name _x];
										} else { 
											_str = format["%1, ",name _x];
										};
										_txt = _txt + _str;
									};
								} forEach crew (vehicle _x);
								_playerPos = visiblePosition (vehicle _x);
								_dist = player distance (vehicle _x);
								_dir = ([player, (vehicle _x)] call BIS_fnc_relativeDirTo) + 180;
								_h = (_playerPos select 2) + ((_dist/30) + 3);
								_fade = (((_dist - 30) - 11) * (-1) )/10;
								if(_dist < 30) then {
									drawIcon3D ["", [0,1,0,1], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, _txt, 2, 0.035, "PuristaLight" ];
								};
								if(_dist >= 30 && _dist <= 45) then {
									drawIcon3D ["", [0,1,0,_fade], [_playerPos select 0,_playerPos select 1,_h], 0, 0, _dir, _txt, 2, 0.035, "PuristaLight" ];
								};
							};
						};
					};
				};
			};
		} forEach playableUnits;
	}];
	sleep 40;
	removeMissionEventHandler ["Draw3D",_idx];
};