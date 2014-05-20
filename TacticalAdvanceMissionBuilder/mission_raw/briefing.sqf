/*
  *	Unofficial Zeus Briefing Template v0.01
  *
  *
  *	Notes: 
  *		- Use the tsk prefix for any tasks you add. This way you know what the varname is for by just looking at it, and 
  *			aids you in preventing using duplicate varnames.
  *		- To add a newline: 		<br/>
  *		- To add a marker link:	<marker name='mkrObj1'>attack this area!</marker>
  *		- To add an image: 		<img image='somePic.jpg'/>
  *
  *		Required briefing commands:		
  *		- Create Note:			player createDiaryRecord ["Diary", ["*The Note Title*", "*The Note Message*"]]; 
  *		- Create Task:			tskExample = player createSimpleTask ["*The Task Title*"];
  *		- Set Task Description:	tskExample setSimpleTaskDescription ["*Task Message*", "*Task Title*", "*Task HUD Title*"];
  *		
  *		Optional briefing commands:
  * 		- Set Task Destination:	tskExample setSimpleTaskDestination (getMarkerPos "mkrObj1"); // use existing "empty marker" 
  *		- Set the Current Task:	player setCurrentTask tskExample;
  *		
  *		Commands to use in-game:
  *		- Set Task State:		tskExample setTaskState "SUCCEEDED";   // states: "SUCCEEDED"  "FAILED"  "CANCELED" 
  *		- Get Task State:		taskState tskExample;
  *		- Get Task Description:	taskDescription tskExample;   // returns the *task title* as a string
  *		- Show Task Hint:		[tskExample] call mk_fTaskHint; // make sure you've set the taskState before using this function 
  *							
  *
  *	Authors: Jinef & mikey
  */

// since we're working with the player object here, make sure it exists
waitUntil { !isNil {player} };
waitUntil { player == player };

[
	[],
	[
		[
			"SITUATION",
			"You have landed with a small number of men in the North East of Altis. Follow HQ's orders and capture the island. As you gain more ground HQ will be able to deploy more resources to the theatre.",
			true
		],

		[
			"MISSION",
			"Insert at the <marker name='respawn_west'>beachhead</marker>, and then following the objectives laid out by HQ in order to capture the island.",
			true
		],

		[
			"HINTS",
			"Name Tag script is active. Press U to see the names of your squadmates. Leaders are able to see other leaders as well.",
			true
		],

		[
			"Assets",
			"Assets will become available as the infrastructure to support them is captured. Vehicles will respawn at the same location once they are destroyed.<br><br>
			Fast roping is available from the helicopters - the pilot must bring the chopper into a hover and use the scroll wheel to 'Deploy Ropes'.  Players can then exit the chopper one by one using their scroll wheel menu.<br><br>",
			true
		],

		[
			"Medical System",
			"Currently respawn only. As you capture objectives new respawn points will open up.",
			true
		],

		[
			"Credits",
			"Mission by |TG| Will for tacticalgamer.com. Edit or update as you wish, just keep the credits:
			<br/>
			<br/>
			aeroson - group manager script.
			<br/>
			<br/>
			|TG|Toptonic-Butler - name tag script.
			<br/>
			<br/>
			|TG| Unkl - TGBanner graphics.
			<br/>
			<br/>
			Tonic - View Distance script
			<br/>
			<br/>
			Bangabob - Enemy Occupation System
			<br/>
			<br/>
			Shuko - taskmaster script.
			",
			true
		]
	]
] call compile preprocessfilelinenumbers "shk_taskmaster.sqf";