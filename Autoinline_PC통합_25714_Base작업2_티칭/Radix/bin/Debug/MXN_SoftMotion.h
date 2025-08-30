#pragma warning(disable : 4996)
#pragma once

#ifdef MXN_SOFTMOTION_EXPORTS
#define MXN_SOFTMOTION_API __declspec(dllexport)
#else
#define MXN_SOFTMOTION_API __declspec(dllimport)
#endif

#define IN
#define OUT

#define MXN_STATUS INT

#define		STATE_NONE			((UINT8) 0x00)
#define 	STATE_INIT			((UINT8) 0x01)
#define 	STATE_PREOP 		((UINT8) 0x02)
#define 	STATE_BOOT			((UINT8) 0x03)
#define 	STATE_SAFEOP		((UINT8) 0x04)
#define 	STATE_OP			((UINT8) 0x08)

#define REG_DATA		0
#define REG_BIT			1

typedef unsigned char   		INT8U;
typedef unsigned short  		INT16U;
typedef unsigned int			INT32U;
typedef unsigned long 			INT64U;
typedef signed char				INT8S; 
typedef signed short			INT16S; 
typedef signed int				INT32S;
typedef signed long				INT64S;
typedef float					FP32;
typedef double					FP64;

typedef enum
{
	SYS_NULL,
	SYS_IDLE,
	SYS_KILLING,
	SYS_KILLED,
	SYS_CREATING,
	SYS_CREATED,
	SYS_INITING,
	SYS_INITED
};

typedef enum
{
	RET_NO_ERROR				= 0,
	RET_ERROR_FUNCTION			= -1,	// Error (Functional buffer over flow)
	RET_ERROR_FULL				= -2,	// Buffer for command is full.
	RET_ERROR_WRONG_INDEX		= -3,	// Commanded Motion block index number is out of range.
	RET_ERROR_WRONG_AXISNO		= -4,	// Axis number does not exist.
	RET_ERROR_MOTIONBUSY		= -5,	// Commanded Motion block is already working.
	RET_ERROR_WRONG_SLAVENO		= -6,	// Slave number does not exist.
	RET_ERROR_WRONG_CAMTABLENO	= -7,	// CamTable number is out of range.
	RET_ERROR_WRONG_ECMASTERNO	= -8,	// ECamMaster number does not exist.
	RET_ERROR_WRONG_ECSLAVENO	= -9,	// ECamSlaver number does not exist.
	RET_ERROR_NOT_OPMODE		= -10,	// Slave is not op-mode.
	RET_ERROR_NOTRUNNING		= -11	// Motion kernel is not running
};

typedef enum
{
	SYSTEM_UNLICENSED		= -2,			// System is NOT licensed.
	SYSTEM_IDLE				= 1,			// System is no working.
	SYSTEM_KILLING			= 2,			// System is killing.
	SYSTEM_KILLED			= 3,			// System is killed. 
	SYSTEM_CREATING			= 4,			// System is creating.
	SYSTEM_CREATED			= 5,			// System is created.
	SYSTEM_INITING			= 6,			// System is initializing.
	SYSTEM_INITED			= 7,			// System is initialized. 
	SYSTEM_READY			= 8,			// System is Initialized. System is ready to run.
	SYSTEM_RUN				= 9				// System is running.
};

typedef struct
{
	INT8U					ucPowerOn;
	INT8U					ucIsHomed;
	INT						iVelocity;
}MXN_READAXISINFO_OUT;

typedef struct
{
	INT32U				uiAxisNo;							// Axis Number

	INT					iVelocity;						// Value of the maximum velocity (always positive) (not necessarily reached) [u/s].
	INT					iPosition;						// Target position for the motion (in technical unit [u]) (negative or positive) 
}MXN_MOVEABSOLUTE_IN;

typedef struct 
{
	INT32U				uiAxisNo;							// Axis Number

	INT					iVelocity;						// Value of the maximum velocity (always positive) (not necessarily reached) [u/s].
	INT					iDistance;						// Relative distance for the motion (in technical unit [u])
}MXN_MOVERELATIVE_IN;

// System Function
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_InitKernel(OUT INT16U &usStatus);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_Destroy();
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_GetOnlineMode(OUT INT16U &usStatus);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_GetKernelStatus();														
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_GetModelInfo(OUT BSTR *ModelPath, OUT BSTR *ModelName);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_GetExitProc(IN INT32U uiProcNum, OUT INT8U &ucStatus);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_SetExitProc(IN INT32U uiProcNum);

// Command Function
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_Power(IN INT16U usServoOn);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_Stop(IN INT32U uiAxisNo);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_MoveAbsolute(IN MXN_MOVEABSOLUTE_IN &InParam);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_MoveRelative(IN MXN_MOVERELATIVE_IN &InParam);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_ReadActualPosition(IN INT32U uiAxisNo, OUT INT32 &iPosition);
extern "C" MXN_SOFTMOTION_API MXN_STATUS __stdcall MXN_ReadAxisInfo(IN INT32U uiAxisNo, OUT MXN_READAXISINFO_OUT &OutAxisParam);

// Register Function
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_X(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_X(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_Y(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_Y(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_T(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_T(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_C(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_C(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_R(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_R(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_D(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_D(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_G(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_G(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_F(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, OUT INT32U &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_F(IN INT32 type, IN INT32 idx, IN INT32 startBit, IN INT32 endBit, IN INT32U data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PA(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PA(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PI(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PI(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PM(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PM(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PP(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PP(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PU(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PU(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_PS(IN INT32 axis, IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_PS(IN INT32 axis, IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_SV(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_SV(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_SN(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_SN(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_MGV(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_MGV(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_MGN(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_MGN(IN INT32 idx, IN FP64 data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Read_B(IN INT32 idx, OUT FP64 &data);
extern "C" MXN_SOFTMOTION_API MXN_STATUS MXN_Write_B(IN INT32 idx, IN FP64 data);
