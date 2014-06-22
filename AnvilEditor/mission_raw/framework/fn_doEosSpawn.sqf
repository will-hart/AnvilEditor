/*
	Author: Will Hart

	Description:
	  Performs an EOS spawn of enemy troops based on the objective description

	Parameter(s):
	  _this select 0: ARRAY, the objective being completed
	  _this select 1: FUNCTION, the function to call when the EOS objective is complete

    Example:
	  [objective, FW_fnc_NOP] call FW_fnc_doEosSpawn;
	
	Returns:
	  Nothing
*/

#include "defines.sqf"

private ['_obj', '_callback'];

_obj = _THIS(0);
_callback = _THIS(1);

// set up the EOS zone
_str = "FW_EnemyStrength" call BIS_fnc_getParamValue;
_nul = [[O_EOS_NAME(_obj)],
        [O_INF(_obj) * _str, O_STR(_obj), 90],          //House Groups, Size of groups, Probability
        [O_INF(_obj) * _str, O_STR(_obj), 90],          //Patrol Groups, Size of groups, Probability
        [O_VEH(_obj) * _str, O_STR(_obj), 90],          //Light Vehicles, Size of Cargo, Probability
        [O_ARM(_obj) * _str, 70],                       //Armoured Vehicles, Probability
        [O_VEH(_obj) * _str, 50],                       //Static Vehicles, Probability
        [O_AIR(_obj) * _str, 0, 80],                    //Helicopters, Size of Cargo, Probability
        [0, 1, 1000, enemyTeam, FALSE, FALSE, [_obj, _callback]]] call EOS_Spawn;
                                                //Faction, Markertype, Distance, Side, HeightLimit, Debug