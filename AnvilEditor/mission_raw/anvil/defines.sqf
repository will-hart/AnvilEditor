// Set up some macros to make life a bit easier. 
// The idea for these macros was borrowed from Aerson's group_manager script
// http://forums.bistudio.com/showthread.php?163206-Group-Manager
#define EL(A,B)                    ((A) select (B))
#define THIS(A)                    EL(this,A)
#define _THIS(A)                   EL(_this,A)
#define APPEND(A, B)               A set [count A, B]

// define some constants
#define AFW_NONE                    -1
#define AFW_MISSIONTYPES            ["Capture", "Gather intel in", "Assassinate officer in", "Destroy tower in", "Destroy AA in", "Clear enemy from "]

// easy element accessors         
#define O_ID(A)                    EL(A,  0)
#define O_DESCRIBE(A)              EL(A,  1)
#define O_MARKER(A)                EL(A,  2)
#define O_R(A)                     EL(A,  3)
#define O_INF(A)                   EL(A,  4)
#define O_VEH(A)                   EL(A,  5)
#define O_ARM(A)                   EL(A,  6)
#define O_AIR(A)                   EL(A,  7)
#define O_STR(A)                   EL(A,  8)
#define O_SPAWN(A)                 EL(A,  9)
#define O_AMMO(A)                  EL(A, 10)
#define O_SPECIAL(A)               EL(A, 11)
#define O_PREREQ(A)                EL(A, 12)
#define O_MISSIONTYPE(A)           EL(A, 13)
#define O_REWARDS(A)               EL(A, 14)

// some shortcut functions for objectives
#define O_MISSIONTYPE_DESC(A)      AFW_MISSIONTYPES select (O_MISSIONTYPE(A))
#define O_X(A)                     (O_POS(A) select 0)
#define O_Y(A)                     (O_POS(A) select 1)
#define O_POS(A)                   getMarkerPos O_MARKER(A)
#define O_SIZE(A)                  [ O_R(A), O_R(A) ]
#define O_EOS_NAME(A)              format ["obj_%1", O_ID(A)]
#define O_OBJ_NAME(A)              format ["objective_%1", O_ID(A)]
#define O_TASK_NAME(A)             format ["%1_%2", O_EOS_NAME(A), O_ID(A)]

