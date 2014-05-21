// Set up some macros to make life a bit easier. 
// These macros are borrowed directly from Aerson's group_manager script
// http://forums.bistudio.com/showthread.php?163206-Group-Manager
#define VARNAME(A,B) ##A##_##B
#define EL(A,B)                    ((A) select (B))
#define THIS(A)                    EL(this,A)
#define _THIS(A)                    EL(_this,A)

// define some constants
#define FW_NONE                    -1

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
#define O_X(A)                     (O_POS(A) select 0)
#define O_Y(A)                     (O_POS(A) select 1)
#define O_POS(A)                   getMarkerPos O_MARKER(A)
#define O_SIZE(A)                  [ O_R(A), O_R(A) ]
#define O_EOS_NAME(A)              format ["obj_%1", O_ID(A)]

// define some rewards            
#define REWARD_NONE              FW_NONE
#define REWARD_NEW_SPAWN            0
#define REWARD_AMMO_DROP            1
#define REWARD_UAV                  2
#define REWARD_VEHICLE_DEPOT        3
#define REWARD_APC_DEPOT            4
#define REWARD_TANK_DEPOT           5
#define REWARD_TRANSPORT_HELO       6
#define REWARD_CAS_HELO             7
#define REWARD_CAS_PLANE            8
#define REWARD_BOAT_DEPOT           9
#define REWARD_ATTACK_BOAT_DEPOT   10
