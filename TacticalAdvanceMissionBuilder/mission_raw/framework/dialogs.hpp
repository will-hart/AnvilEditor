class FW_PersistMission_dialog
{
	idd=-1;
	movingenable=false;
	
	class controls 
	{
		////////////////////////////////////////////////////////
		// GUI EDITOR OUTPUT START (by |TG| Will, v1.063, #Xicyla)
		////////////////////////////////////////////////////////

		class FW_PersistMission_background: Box
		{
			idc = 1799;
			text = ""; //--- ToDo: Localize;
			x = 0.304113 * safezoneW + safezoneX;
			y = 0.291 * safezoneH + safezoneY;
			w = 0.381463 * safezoneW;
			h = 0.374 * safezoneH;
		};
		class FW_PersistMission_frame: RscFrame
		{
			idc = 1800;
			text = "Save or Load Mission State"; //--- ToDo: Localize;
			x = 0.304113 * safezoneW + safezoneX;
			y = 0.291 * safezoneH + safezoneY;
			w = 0.381463 * safezoneW;
			h = 0.374 * safezoneH;
		};
		class FW_PersistMission_textHeader: RscText
		{
			idc = 1000;
			text = "Use this box to save or load mission state - enter text in the text box or copy the text to save"; //--- ToDo: Localize;
			x = 0.314423 * safezoneW + safezoneX;
			y = 0.313 * safezoneH + safezoneY;
			w = 0.360844 * safezoneW;
			h = 0.066 * safezoneH;
		};
		class FW_PersistMission_missionDataText: RscEdit
		{
			idc = 1400;
			x = 0.324733 * safezoneW + safezoneX;
			y = 0.445 * safezoneH + safezoneY;
			w = 0.350534 * safezoneW;
			h = 0.033 * safezoneH;
		};
		class FW_PersistMission_loadMissionButton: RscButton
		{
			idc = 1600;
			text = "Load"; //--- ToDo: Localize;
			x = 0.582479 * safezoneW + safezoneX;
			y = 0.588 * safezoneH + safezoneY;
			w = 0.0412393 * safezoneW;
			h = 0.055 * safezoneH;
			action = "closeDialog 0;";
		};
		class FW_PersistMission_closeButton: RscButton
		{
			idc = 1601;
			text = "Close"; //--- ToDo: Localize;
			x = 0.634028 * safezoneW + safezoneX;
			y = 0.588 * safezoneH + safezoneY;
			w = 0.0412393 * safezoneW;
			h = 0.055 * safezoneH;
			action = "closeDialog 0;";
		};
		class FW_PersistMission_saveMissionButton: RscButton
		{
			idc = 1602;
			text = "Save"; //--- ToDo: Localize;
			x = 0.530929 * safezoneW + safezoneX;
			y = 0.588 * safezoneH + safezoneY;
			w = 0.0412393 * safezoneW;
			h = 0.055 * safezoneH;
			action = "ctrlSetText [1400, format[""%1"", [] call FW_fnc_getObjectiveState]]";
		};
		////////////////////////////////////////////////////////
		// GUI EDITOR OUTPUT END
		////////////////////////////////////////////////////////

	};
};

