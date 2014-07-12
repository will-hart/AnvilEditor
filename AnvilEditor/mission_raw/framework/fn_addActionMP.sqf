/*
	Author: Will Hart

	Description:
	  Creates an addAction for all human multiplayer players

	Parameter(s):
	  _this select 0: OBJECT, The object to apply the action to
	  _this select 1: STRING, The title to display for the object
	  _this select 2: FUNCTION, The function to call when complete
	  _this select 3: ARRAY or OBJECT, The arguments to pass to the function

	Example:
	    [[_my_item, "Remove", { 
		    deleteVehicle (_this select 3) select 0; 
	    }, [_my_vehicle], "FW_fnc_addActionMP", nil, false] spawn BIS_fnc_MP;
	
	Returns:
	  Nothing
*/

private["_obj", "_title", "_fn","_args"];
_obj = _this select 0;
_title = _this select 1;
_fn = _this select 2;
_args = _this select 3;

_obj addAction [_title, _fn, _args];
