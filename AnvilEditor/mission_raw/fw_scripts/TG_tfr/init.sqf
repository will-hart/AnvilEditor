if (isNil "tfr_ammobox") then {
    hint "No 'tfr_ammobox' object found. Radios will not be available to players";
};

//TaskForceRadio controls
//Stop spawning new long range radios
tf_no_auto_long_range_radio = true;
//Give out personal radios
TF_give_personal_radio_to_regular_soldier = true;

//Same frequencies for everyone
tf_same_sw_frequencies_for_side = true;
tf_same_lr_frequencies_for_side = true;
//Same frequencies for everyone
tf_same_sw_frequencies_for_side = true;
tf_same_lr_frequencies_for_side = true;

tf_defaultWestBackpack = "tf_rt1523g";
tf_defaultWestPersonalRadio = "tf_anprc152"; 
tf_defaultWestRiflemanRadio = "tf_rf7800str"; 
tf_defaultWestAirborneRadio = "tf_anarc210";
 
tf_defaultEastBackpack = "tf_rt1523g"; 
tf_defaultEastPersonalRadio = "tf_anprc152"; 
tf_defaultEastRiflemanRadio = "tf_rf7800str"; 
tf_defaultEastAirborneRadio = "tf_anarc210";

if !(isDedicated) then
{
    player unassignItem "ItemRadio";
    player removeItem "ItemRadio";
};
