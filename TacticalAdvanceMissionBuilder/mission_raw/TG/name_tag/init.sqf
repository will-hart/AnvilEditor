call compile PreprocessFileLineNumbers "TG\name_tag\func.sqf";
_key = actionKeysnames ["teamswitch",1];
cutText [format["Press %1 to show your squadmates!",_key],"plain",2,true];
while {true} do
{
	waitUntil {(inputAction "teamswitch")>0};
	call fn_createLabels;
	sleep 10;
};