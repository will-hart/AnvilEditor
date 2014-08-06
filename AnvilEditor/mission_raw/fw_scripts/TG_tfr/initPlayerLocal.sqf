_is_mod_tfr_enabled_locally = isClass(configFile/"CfgPatches"/"task_force_radio");
if (_is_mod_tfr_enabled_locally) then
{
 short_radio = compile preprocessfilelinenumbers "TG_tfr/getShortRadio.sqf";
 long_radio  = compile preprocessfilelinenumbers "TG_tfr/getLongRadio.sqf";

 tfr_ammobox addAction ["<t color='#00ffff'>Get Long Range Radio</t> ", "call long_radio"];
 tfr_ammobox addAction ["<t color='#008db8'>Get Short Range Radio</t> ", "call short_radio"];
};
