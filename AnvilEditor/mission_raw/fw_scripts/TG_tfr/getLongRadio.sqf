_caller = _this select 1;
_type = typeOf _caller;

if (_type IN _leaders) then
{
	removeBackpack _caller;
	_caller addBackpack "tf_rt1523g";
	hintSilent "A long range radio backpack has replaced your backpack.";
}
else
{
	hintSilent "Only authorized personnel (leaders-pilots-crewmen) may create a long range radio.";
};