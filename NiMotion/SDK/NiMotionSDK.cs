using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NiMotion.Model;

namespace SDKDemo
{
    class NiMotionSDK
    {
        private const string DLL_PATH = @"NIMSTMMODBUS\NiMotionMotorSDK.dll";

        unsafe public struct MOTOR_INFO
        {
            public uint nAddr;  //电机地址
            public fixed byte szSerialNumber[20];    //电机序列号
            public fixed byte szHardVersion[20];     //硬件版本号
            public fixed byte szSoftVersion[20];     //软件版本号
        }

        unsafe public struct SELFCHECK_RESULT
        {
            public uint nAddr;  //电机地址
            public fixed int nResult[4]; //电机自检
        }


        public enum WORK_MODE
        {
            POSITION_MODE = 1,
            VELOCITY_MODE = 2,
            GOHOME_MODE = 3
        }

        /*======================================================单串口函数组=====================================================================*/

        /**************************错误码********************************/
        /**
         * 0   执行成功
         * 1   不支持的设备类型
         * -1  执行失败
         * -2  电机地址选择错误
         * -3  参数传入错误
         * -4  当前电机运动模式错误
         * -5  电机未在使能状态
         * -7  当前电机不支持的操作
        */

        /***********************通信设备操作函数****************************/
        /**
         * @brief 打开通信设备
         * @param nType 通信设备类型 (0:RTU 1:TCP，暂不支持)
         * @param strConnectString 连接字符串，描述设备连接参数()
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_openDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_openDevice(int nType, string strConnectString);

        /**
         * @brief 关闭通信设备
         * @return 0 成功，其它表示错误码
        */
        [DllImport(DLL_PATH, EntryPoint = "NiM_closeDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_closeDevice();

        /**
         * @brief 指定通信电机类型
         * param nMotorType 通信电机类型 (0:原Modbus电机 1:无刷Modbus电机)
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_specifyMotorType", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_specifyMotorType(int nMotorType);

        /***********************在线电机管理函数****************************/
        /**
         * @brief 扫描电机
         * @param nFromAddr 起始地址
         * @param nToAddr 结束地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_scanMotors", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_scanMotors(int nFromAddr, int nToAddr);

        /**
         * @brief 获取在线电机列表
         * @param pAddrs 电机地址数组指针
         * @param pCount 数量指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getOnlineMotors", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getOnlineMotors(System.IntPtr pAddrs, ref int pCount);

        /**
         * @brief 判断电机是否在线
         * @param nAddr 电机地址
         * @param pOnline 指针，返回在线状态
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_isMotorOnline", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_isMotorOnline(int nAddr, ref bool pOnline);

        /**
         * @brief 获取电机基本信息
         * @param nAddr 电机地址
         * @param pInfo 电机信息结构体指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getMotorInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getMotorInfo(int nAddr, ref MOTOR_INFO pInfo);

        /**
         * @brief 执行电机自检
         * @param nAddr 电机地址
         * @param pResult 自检结果结构体指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_selfcheck", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_selfcheck(int nAddr, ref SELFCHECK_RESULT pResult);

        /**
         * @brief 获取电机最近的报警
         * @param nAddr 电机地址
         * @param pAlarmCode  指针，返回报警值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getLatestAlarm", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getLatestAlarm(int nAddr, ref int pAlarmCode);

        /**
         * @brief 获取电机故障码
         * @param nAddr 电机地址
         * @param pErrorCode 指针，返回故障码
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getErrorCode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getErrorCode(int nAddr, ref int pErrorCode);

        /**
         * @brief 获取电机历史报警
         * @param nAddr 电机地址
         * @param pAlarmCode 数组指针，返回报警值列表
         * @param pCount 指针，返回报警数量
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getHistoryAlarms", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getHistoryAlarms(int nAddr, ref int[] pAlarmCode, ref int pCount);

        /**
         * @brief 清除电机报警
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_clearAlarms", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_clearAlarms(int nAddr);

        /**
         * @brief 清除电机故障
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_clearErrorState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_clearErrorState(int nAddr);

        /**
         * @brief 重启电机
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_rebootMotor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_rebootMotor(int nAddr);

        /**********************电机控制函数******************************/

        /**
         * @brief 获取电机参数值
         * @param nAddr 电机地址
         * @param nParamID 参数ID
         * @param nBytes 字节数
         * @param pParamValue 指针，返回参数值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_readParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_readParam(int naddr, int nparamid, int nbytes, ref int pparamvalue);

        /**
         * @brief 设置电机参数值
         * @param nAddr 电机地址
         * @param nParamID 参数ID
         * @param nBytes 字节数
         * @param nParamValue 参数值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_writeParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_writeParam(int nAddr, int nParamID, int nBytes, int nParamValue);

        /**
         * @brief 保存电机参数
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_saveParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_saveParams(int nAddr);

        /**
         * @brief 恢复电机出厂设置
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_restoreFactorySettings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_restoreFactorySettings(int nAddr);

        /**
         * @brief 修改电机地址
         * @param nCurAddr 电机当前地址
         * @param nNewAddr 修改后的新地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_changeAddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_changeAddr(int nCurAddr, int nNewAddr);

        /**
         * @brief 改变DO状态
         * @param nAddr 电机地址
         * @param nDOValue DO配置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_setDOState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_setDOState(int nAddr, int nDOValue);

        /**
         * @brief 读取DI状态
         * @param nAddr 电机地址
         * @param *pDIState 指向存储DI状态的指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_readDIState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_readDIState(int nAddr, ref int pDIState);

        /**
         * @brief 读取DO状态
         * @param nAddr 电机地址
         * @param *pDOState 指向存储DO状态的指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_readDOState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_readDOState(int nAddr, ref int pDOState);

        /**
         * @brief 修改电机运行模式
         * @param nAddr 电机地址
         * @param nMode 运行模式
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_changeWorkMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_changeWorkMode(int nAddr, WORK_MODE nMode);

        /**
         * @brief 获取电机状态字
         * @param nAddr 电机地址
         * @param pStatusWord 指针，返回状态字
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getCurrentStatus", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getCurrentStatus(int nAddr, ref int pStatusWord);

        /**
         * @brief 获取电机当前位置
         * @param nAddr 电机地址
         * @param pPosition 指针，返回当前位置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_getCurrentPosition", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_getCurrentPosition(int nAddr, ref int pPosition);

        /**
         * @brief 将电机当前位置保存为原点
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_saveAsHome", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_saveAsHome(int nAddr);

        /**
         * @brief 将电机当前位置保存为零点
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_saveAsZero", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_saveAsZero(int nAddr);

        /**
         * @brief 绝对位置运动
         * @param nAddr 电机地址
         * @param nPosition 目标位置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_moveAbsolute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_moveAbsolute(int nAddr, int nPosition);

        /**
         * @brief 相对位置运动
         * @param nAddr 电机地址
         * @param nDistance 运动距离
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_moveRelative", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_moveRelative(int nAddr, int nDistance);

        /**
         * @brief 速度模式运动
         * @param nAddr 电机地址
         * @param nVelocity 目标速度
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_moveVelocity", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_moveVelocity(int nAddr, int nVelocity);

        /**
         * @brief 原点回归
         * @param nAddr 电机地址
         * @param nType 原点回归方式
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_goHome", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_goHome(int nAddr, int nType);

        /**
         * @brief 给电机驱动电路上电（抱机）
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_powerOn", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_powerOn(int nAddr);

        /**
         * @brief 给电机驱动电路断电（脱机）
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_powerOff", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_powerOff(int nAddr);

        /**
         * @brief 停止当前动作
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_stop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_stop(int nAddr);

        /**
         * @brief 急停
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_fastStop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_fastStop(int nAddr);

        /**
         * @brief SDK调试模式
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_setDebug", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_setDebug(bool flag);

        /*======================================================多串口函数组=====================================================================*/

        /**************************错误码********************************/
        /**
         * 0   执行成功
         * 1   不支持的设备类型
         * -1  执行失败
         * -2  电机地址选择错误
         * -3  参数传入错误
         * -4  当前电机运动模式错误
         * -5  电机未在使能状态
         * -6  串口被占用或有通信问题
         * -7  当前电机不支持的操作
        */

        /***********************通信设备操作函数****************************/
        /**
         * @brief 打开通信设备
         * @param nType 通信设备类型 (0:RTU 1:TCP，暂不支持)
         * @param strConnectString 连接字符串，描述设备连接参数()
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPopenDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPopenDevice(int nType, string strConnectString);

        /**
         * @brief 关闭通信设备
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPcloseDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPcloseDevice();

        /***********************在线电机管理函数****************************/
        /**
         * @brief 扫描电机
         * @param strPort 当前操作的端口号
         * @param nFromAddr 起始地址
         * @param nToAddr 结束地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPscanMotors", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPscanMotors(string strPort, int nFromAddr, int nToAddr);

        /**
         * @brief 获取在线电机列表
         * @param strPort 当前操作的端口号
         * @param pAddrs 电机地址数组指针
         * @param pCount 数量指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetOnlineMotors", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetOnlineMotors(string strPort, ref int pAddrs, ref int pCount);

        /**
         * @brief 判断电机是否在线
         * @param strPort 当前操作的端口号
         * @param pOnline 指针，返回在线状态
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPisMotorOnline", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPisMotorOnline(string strPort, int nAddr, ref bool pOnline);

        /**
         * @brief 获取电机基本信息
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pInfo 电机信息结构体指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetMotorInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetMotorInfo(string strPort, int nAddr, ref MOTOR_INFO pInfo);

        /**
         * @brief 执行电机自检
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pResult 自检结果结构体指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPselfcheck", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPselfcheck(string strPort, int nAddr, ref SELFCHECK_RESULT pResult);

        /**
         * @brief 获取电机最近的报警
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pAlarmCode 指针，返回报警值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetLatestAlarm", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetLatestAlarm(string strPort, int nAddr, ref int pAlarmCode);

        /**
         * @brief 获取电机故障码
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pErrorCode 指针，返回故障码
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetErrorCode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetErrorCode(string strPort, int nAddr, ref int pErrorCode);

        /**
         * @brief 获取电机历史报警
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pAlarmCode 数组指针，返回报警值列表
         * @param pCount 指针，返回报警数量
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetHistoryAlarms", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetHistoryAlarms(string strPort, int nAddr, ref int[] pAlarmCode, ref int pCount);

        /**
         * @brief 清除电机报警
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPclearAlarms", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPclearAlarms(string strPort, int nAddr);

        /**
         * @brief 清除电机故障
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPclearErrorState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPclearErrorState(string strPort, int nAddr);

        /**
         * @brief 重启电机
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPrebootMotor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPrebootMotor(string strPort, int nAddr);

        /**********************电机控制函数******************************/

        /**
         * @brief 获取电机参数值
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nParamID 参数ID
         * @param nBytes 字节数
         * @param pParamValue 指针，返回参数值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPreadParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPreadParam(string strPort, int nAddr, int nParamID, int nBytes, ref int pParamValue);

        /**
         * @brief 设置电机参数值
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nParamID 参数ID
         * @param nBytes 字节数
         * @param nParamValue 参数值
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPwriteParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPwriteParam(string strPort, int nAddr, int nParamID, int nBytes, int nParamValue);

        /**
         * @brief 保存电机参数
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPsaveParams", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPsaveParams(string strPort, int nAddr);

        /**
         * @brief 恢复电机出厂设置
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPrestoreFactorySettings", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPrestoreFactorySettings(string strPort, int nAddr);

        /**
         * @brief 修改电机地址
         * @param strPort 当前操作的端口号
         * @param nCurAddr 电机当前地址
         * @param nNewAddr 修改后的新地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPchangeAddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPchangeAddr(string strPort, int nCurAddr, int nNewAddr);

        /**
         * @brief 改变DO状态
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nDOValue DO配置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPsetDOState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPsetDOState(string strPort, int nAddr, int nDOValue);

        /**
         * @brief 读取DI状态
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param *pDIState 指向存储DI状态的指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPreadDIState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPreadDIState(string strPort, int nAddr, ref int pDIState);

        /**
         * @brief 读取DO状态
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param *pDOState 指向存储DO状态的指针
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPreadDOState", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPreadDOState(string strPort, int nAddr, ref int pDOState);

        /**
         * @brief 修改电机运行模式
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nMode 运行模式
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPchangeWorkMode", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPchangeWorkMode(string strPort, int nAddr, WORK_MODE nMode);

        /**
         * @brief 获取电机状态字
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pStatusWord 指针，返回状态字
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetCurrentStatus", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetCurrentStatus(string strPort, int nAddr, ref int pStatusWord);

        /**
         * @brief 获取电机当前位置
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param pPosition 指针，返回当前位置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgetCurrentPosition", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgetCurrentPosition(string strPort, int nAddr, ref int pPosition);

        /**
         * @brief 将电机当前位置保存为原点
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPsaveAsHome", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPsaveAsHome(string strPort, int nAddr);

        /**
         * @brief 将电机当前位置保存为零点
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPsaveAsZero", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPsaveAsZero(string strPort, int nAddr);

        /**
         * @brief 绝对位置运动
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nPosition 目标位置
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPmoveAbsolute", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPmoveAbsolute(string strPort, int nAddr, int nPosition);

        /**
         * @brief 相对位置运动
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nDistance 运动距离
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPmoveRelative", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPmoveRelative(string strPort, int nAddr, int nDistance);

        /**
         * @brief 速度模式运动
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nVelocity 目标速度
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPmoveVelocity", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPmoveVelocity(string strPort, int nAddr, int nVelocity);

        /**
         * @brief 原点回归
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @param nType 原点回归方式
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPgoHome", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPgoHome(string strPort, int nAddr, int nType);

        /**
         * @brief 给电机驱动电路上电（抱机）
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPpowerOn", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPpowerOn(string strPort, int nAddr);

        /**
         * @brief 给电机驱动电路断电（脱机）
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPpowerOff", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPpowerOff(string strPort, int nAddr);

        /**
         * @brief 停止当前动作
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPstop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPstop(string strPort, int nAddr);

        /**
         * @brief 急停
         * @param strPort 当前操作的端口号
         * @param nAddr 电机地址
         * @return 0 成功，其它表示错误码
         */
        [DllImport(DLL_PATH, EntryPoint = "NiM_MPfastStop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NiM_MPfastStop(string strPort, int nAddr);
    }
}
