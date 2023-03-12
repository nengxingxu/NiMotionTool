using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NimServoSDK_DLL
{
    public enum CanBaudRate
    {
        CAN_BT_10K = 0,
        CAN_BT_20K,
        CAN_BT_50K,
        CAN_BT_100K,
        CAN_BT_125K,
        CAN_BT_250K,
        CAN_BT_500K,
        CAN_BT_800K,
        CAN_BT_1000K
    };

    public enum ServoWorkMode
    {
        SERVO_PP_MODE = 1,  //轮廓位置模式
        SERVO_VM_MODE = 2,  //速度模式
        SERVO_PV_MODE = 3,  //轮廓速度模式
        SERVO_PT_MODE = 4,  //轮廓转矩模式
        SERVO_HM_MODE = 6,  //原点回归模式
        SERVO_IP_MODE = 7,  //位置插补模式
        SERVO_CSP_MODE,     //循环同步位置模式
        SERVO_CSV_MODE,     //循环同步速度模式
        SERVO_CST_MODE      //循环同步转矩模式
    };

    public enum ServoSDK_Error
    {
        ServoSDK_NoError = 0,           //没有错误
        ServoSDK_NotRegisted,           //SDK没有注册
        ServoSDK_NotInitialized,        //SDK没有初始化
        ServoSDK_UnsupportedCommType,   //不支持的通信方式
        ServoSDK_ParamError,            //输入参数错误
        ServoSDK_CreateMasterFailed,    //创建主站失败
        ServoSDK_MasterNotExist,        //主站不存在
        ServoSDK_MasterStartFailed,     //主站启动失败
        ServoSDK_MasterNotRunning,      //主站未运行
        ServoSDK_SlaveNotOnline,        //从站不在线
        ServoSDK_LoadParamSheetFailed,  //加载参数表错误
        ServoSDK_ParamNotExist,         //请求的参数不存在
        ServoSDK_ReadSDOFailed,         //读SDO失败
        ServoSDK_WriteSDOFailed,        //写SDO失败
        ServoSDK_OperationNotAllowed,   //操作不允许
        ServoSDK_MasterInternalError,   //主站内部错误
        ServoSDK_SlaveInternalError,    //从站内部错误
        ServoSDK_Cia402ModeError,       //从站402模式错误
        ServoSDK_ReadWorkModeFailed,    //读取工作模式失败
        ServoSDK_ReadStatusWordFailed,  //读取状态字失败
        ServoSDK_ReadCurrentPosFailed,  //读取当前位置失败
        ServoSDK_ReadRPDOConfigFailed,  //读取PDO配置失败
        ServoSDK_ReadTPDOConfigFailed,  //读取PDO配置失败
        ServoSDK_WriteControlWordFailed,  //写控制字失败
        ServoSDK_WriteTargetPosFailed,  //写目标位置失败
        ServoSDK_WriteTargetVelFailed,  //写目标速度失败
        ServoSDK_WriteGoHomeTypeFailed,  //写原点回归方式失败
        ServoSDK_GetHostInfoFailed,     //获取主机信息失败
        ServoSDK_SaveParamsFailed,      //保存参数失败
        ServoSDK_NoAvailableDevice,     //没有可用的设备
        ServoSDK_Unknown = 255          //未知错误
    };

    public static class NimServoSDK
    {
        /**
         * @brief SDK初始化
         * @param nCommType 通信方式：0 CANopen；1 EtherCAT ； 2 Modbus
         * @param strSdkPath 库路径，与设置的系统环境一致
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_init(string strSdkPath);

        /**
         * @brief SDK 反初始化
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_clean",CallingConvention = CallingConvention.Cdecl)]
        public static extern void Nim_clean();

        /**
         * @brief 创建主站对象
         * @param nCommType 通信方式：0 CANopen；1 EtherCAT; 2 Modbus
         * @param handle 输出参数，成功时返回创建的主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_create_master",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_create_master(int nCommType, ref uint handle);

        /**
         * @brief 销毁主站对象
         * @param handle 由Nim_create_master函数创建的主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_destroy_master",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_destroy_master(uint handle);

        /**
         * @brief 启动通信主站
         * @param hMaster 主站对象句柄
         * @param conn_str 连接字符串，json格式，具体内容参见使用手册
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_master_run",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_master_run(uint hMaster, string conn_str);

        /**
         * @brief 关闭通信主站
         * @param hMaster 主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_master_stop",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_master_stop(uint hMaster);

        /**
         * @brief 主站进入OP模式
         * @param hMaster 主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_master_changeToOP",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_master_changeToOP(uint hMaster);

        /**
         * @brief 主站进入PreOP模式
         * @param hMaster 主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_master_changeToPreOP",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_master_changeToPreOP(uint hMaster);

        /**
         * @brief 按照指定的地址范围扫描从站是否在线
         * @param hMaster 主站对象句柄
         * @param from 起始地址
         * @param to 结束地址
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_scan_nodes",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_scan_nodes(uint hMaster, int from, int to);

        /**
         * @brief 查询从站是否在线
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @return 非零 在线；0 不在线
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_is_online",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_is_online(uint hMaster, int nodeId);

        /**
         * @brief 读取从站PDO配置
         * @param hMaster 主站对象句柄
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_read_PDOConfig",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_read_PDOConfig(uint hMaster, int nodeId);

        /**
         * @brief 加载电机参数表
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param db_name 参数表数据库文件名
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_load_params",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_load_params(uint hMaster, int nodeId, string db_name);

        /**
         * @brief 读取从站参数
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param pParamNO 参数编号
         * @param puiValue 参数值
         * @param bSDO 1 使用SDO读；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_param_value",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_param_value(uint hMaster, int nodeId, string ParamNO, ref uint puiValue, int bSDO);

        /**
         * @brief 设置从站参数
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param pParamNO 参数编号
         * @param uiValue 参数值
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_param_value",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_param_value(uint hMaster, int nodeId, string ParamNO, uint uiValue, int bSDO);

        /**
         * @brief 电机抱机
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_power_on",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_power_on(uint hMaster, int nodeId, int bSDO);

        /**
         * @brief 电机脱机
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return true 成功；false 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_power_off",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_power_off(uint hMaster, int nodeId, int bSDO);

        /**
         * @brief 设置控制字(6040)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param cw 控制字
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_controlWord",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_controlWord(uint hMaster, int nodeId, UInt16 cw, int bSDO);

        /**
         * @brief 获取电机状态字(6041)
         * @param hMaster 主站对象句柄
         * @param nodeId 电机地址
         * @param sw 输出参数，成功时返回电机状态字
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_statusWord",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_statusWord(uint hMaster, int nodeId, ref UInt16 sw, int bSDO);

        /**
         * @brief 设置电机工作模式（6060,在脱机状态下设置）
         * @param hMaster 主站对象句柄
         * @param nodeId 电机地址
         * @param mode 模式
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_workMode",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_workMode(uint hMaster, int nodeId, int mode, int bSDO);

        /**
         * @brief 获取电机工作模式显示值(6061)
         * @param hMaster 主站对象句柄
         * @param nodeId 电机地址
         * @param mode 输出参数，成功时返回电机工作模式
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_workModeDisplay",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_workModeDisplay(uint hMaster, int nodeId, ref int mode, int bSDO);

        /**
         * @brief 原点回归
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param type 回原点方式(请参考电机通信手册)
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_goHome",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_goHome(uint hMaster, int nodeId, int type, int bSDO);

        /**
         * @brief 轮廓速度模式下正转
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param fVelocity  速度（用户单位/s）
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_forward",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_forward(uint hMaster, int nodeId, double fVelocity, int bSDO);

        /**
         * @brief 轮廓速度模式下反转
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param fVelocity  速度（用户单位/s）
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_backward",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_backward(uint hMaster, int nodeId, double fVelocity, int bSDO);

        /**
         * @brief 设置目标速度(60FF)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param fVelocity 目标速度（用户单位/s）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_targetVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_targetVelocity(uint hMaster, int nodeId, double fVelocity, int bSDO);

        /**
         * @brief 设置VM模式下的目标速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param speed 目标速度（rpm）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_vmTargetSpeed",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_vmTargetSpeed(uint hMaster, int nodeId, int speed, int bSDO);

        /**
         * @brief 获取VM模式下的当前速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param speed 输出参数，成功时返回当前速度（rpm）
         * @param bSDO 1 使用SDO读；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_vmCurrentSpeed",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_vmCurrentSpeed(uint hMaster, int nodeId, ref int speed, int bSDO);

        /**
         * @brief 轮廓位置模式下绝对位置运动
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param target 目标位置（用户单位）
         * @param bChangeImmediatly 是否立即更新：1 立即更新；0 非立即更新
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_moveAbsolute",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_moveAbsolute(uint hMaster, int nodeId, double position, int bChangeImmediatly, int bSDO);

        /**
         * @brief 轮廓位置模式下相对位置运动
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param distance 目标位置（用户单位）
         * @param bChangeImmediatly 是否立即更新：1 立即更新；0 非立即更新
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_moveRelative",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_moveRelative(uint hMaster, int nodeId, double distance, int bChangeImmediatly, int bSDO);

        /**
         * @brief 设置目标位置(607A)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param fPos 目标位置（用户单位）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_targetPosition",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_targetPosition(uint hMaster, int nodeId, double fPos, int bSDO);

        /**
         * @brief 设置插补位置(60C1:01)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param fPos  位置（用户单位）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_ipPosition",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_ipPosition(uint hMaster, int nodeId, double fPos, int bSDO);

        /**
         * @brief 设置目标转矩(6071)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque  目标转矩（0.001倍额定转矩）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_targetTorque",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_targetTorque(uint hMaster, int nodeId, int torque, int bSDO);

        /**
         * @brief 获取当前转矩(6077)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque 输出参数，成功时返回当前转矩（0.001倍额定转矩）
         * @param bSDO 1 使用SDO写；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_currentTorque",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_currentTorque(uint hMaster, int nodeId, ref int torque, int bSDO);

        /**
         * @brief 快速停止当前动作
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_fastStop",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_fastStop(uint hMaster, int nodeId, int bSDO);

        /**
         * @brief 清除轴故障
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_clearError",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_clearError(uint hMaster, int nodeId, int bSDO);

        /**
         * @brief 获取最新报警(603F)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param alarmCode 输出参数，执行成功时返回报警码
         * @param bSDO 1 使用SDO控制；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_newestAlarm",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_newestAlarm(uint hMaster, int nodeId, ref uint alarmCode, int bSDO);

        /**
         * @brief 获取历史报警数量(1003:00)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param alarmCode 输出参数，执行成功时返回历史报警数量
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_alarmCount",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_alarmCount(uint hMaster, int nodeId, ref int count);

        /**
         * @brief 获取历史报警(1003:01~10h)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param index 历史报警序号,取值范围：1~16
         * @param alarmCode 输出参数，执行成功时返回报警码
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_alarm",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_alarm(uint hMaster, int nodeId, int index, ref uint alarmCode);

        /**
         * @brief 获取轮廓速度(6081)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param fVelocity 速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_profileVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_profileVelocity(uint hMaster, int nodeId, ref double fVelocity);

        /**
         * @brief 获取轮廓加速度(6083)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param accel 加速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_profileAccel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_profileAccel(uint hMaster, int nodeId, ref double accel);

        /**
         * @brief 获取轮廓减速度(6084)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param decel 减速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_profileDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_profileDecel(uint hMaster, int nodeId, ref double decel);

        /**
         * @brief 获取快速停机减速度(6085)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param decel 减速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_quickStopDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_quickStopDecel(uint hMaster, int nodeId, ref double decel);

        /**
         * @brief 设置轮廓速度(6081)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param fVelocity 速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_profileVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_profileVelocity(uint hMaster, int nodeId, double fVelocity);

        /**
         * @brief 设置轮廓加速度(6083)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param accel 加速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_profileAccel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_profileAccel(uint hMaster, int nodeId, double accel);

        /**
         * @brief 设置轮廓减速度(6084)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param decel 减速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_profileDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_profileDecel(uint hMaster, int nodeId, double decel);

        /**
         * @brief 设置快速停机减速度(6085)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param decel 减速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_quickStopDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_quickStopDecel(uint hMaster, int nodeId, double decel);

        /**
         * @brief 获取原点偏移
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param offset 原点偏移，用户单位
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_homeOffset",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_homeOffset(uint hMaster, int nodeId, ref double offset);

        /**
         * @brief 获取原点回归速度(6099:01/02)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity1 寻找开关速度（用户单位/s）
         * @param velocity2 寻找原点速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_goHome_velocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_goHome_velocity(uint hMaster, int nodeId, ref double velocity1, ref double velocity2);

        /**
         * @brief 获取原点回归加速度(609A)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param accel 加速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_goHome_accel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_goHome_accel(uint hMaster, int nodeId, ref double accel);

        /**
         * @brief 设置原点偏移
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param offset 原点偏移，用户单位
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_homeOffset",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_homeOffset(uint hMaster, int nodeId, double offset);

        /**
         * @brief 设置原点回归速度(6099:01/02)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity1 寻找开关速度（用户单位/s）
         * @param velocity2 寻找原点速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_goHome_velocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_goHome_velocity(uint hMaster, int nodeId, double velocity1, double velocity2);

        /**
         * @brief 设置原点回归加速度(609A)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param accel 加速度（用户单位/s^2）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_goHome_accel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_goHome_accel(uint hMaster, int nodeId, double accel);

        /**
         * @brief 获取VM模式下的加速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param accel 输出参数，成功时返回加速度（rpm/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_vmAccel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_vmAccel(uint hMaster, int nodeId, ref double accel);

        /**
         * @brief 设置VM模式下的加速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param deltaV 速度变化量（rpm）
         * @param deltaT 时间变化量（S）
         *        加速度 = deltaV/deltaT
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_vmAccel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_vmAccel(uint hMaster, int nodeId, uint deltaV, uint deltaT);

        /**
         * @brief 获取VM模式下的减速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param accel 输出参数，成功时返回减速度（rpm/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_vmDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_vmDecel(uint hMaster, int nodeId, ref double decel);

        /**
         * @brief 设置VM模式下的减速度
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param deltaV 速度变化量（rpm）
         * @param deltaT 时间变化量（S）
         *        减速度 = deltaV/deltaT
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_vmDecel",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_vmDecel(uint hMaster, int nodeId, uint deltaV, uint deltaT);

        /**
         * @brief 通过6069获取当前速度
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity 输出参数，执行成功时返回当前速度（用户单位/s）
         * @param bSDO 1 使用SDO输出；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_currentVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_currentVelocity(uint hMaster, int nodeId, ref double velocity, int bSDO);

        /**
         * @brief 通过606C获取当前速度
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity 输出参数，执行成功时返回当前速度（用户单位/s）
         * @param bSDO 1 使用SDO输出；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_currentVelocity2",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_currentVelocity2(uint hMaster, int nodeId, ref double velocity, int bSDO);

        /**
         * @brief 通过606C获取当前电机速度
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param speed 输出参数，执行成功时返回当前速度（rpm）
         * @param bSDO 1 使用SDO输出；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_currentMotorSpeed",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_currentMotorSpeed(uint hMaster, int nodeId, ref int speed, int bSDO);

        /**
         * @brief 通过6064获取当前位置
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param position 输出参数，执行成功时返回当前位置
         * @param bSDO 1 使用SDO输出；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_currentPosition",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_currentPosition(uint hMaster, int nodeId, ref double position, int bSDO);

        /**
         * @brief 获取位置限制值(607D:01/02)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param minPos 输出参数，执行成功时返回最小极限位置
         * @param maxPos 输出参数，执行成功时返回最大极限位置
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_posLimit",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_posLimit(uint hMaster, int nodeId, ref double minPos, ref double maxPos);

        /**
         * @brief 设置位置限制值(607D:01/02)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param pos 输出参数，执行成功时返回最大极限位置
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_posLimit",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_posLimit(uint hMaster, int nodeId, double minPos, double maxPos);

        /**
         * @brief 设置最大速度(607F)
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity 最大速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_maxVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_maxVelocity(uint hMaster, int nodeId, double velocity);

        /**
         * @brief 获取最大速度
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param velocity 输出参数，成功时返回最大速度（用户单位/s）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_maxVelocity",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_maxVelocity(uint hMaster, int nodeId, ref double velocity);

        /**
         * @brief 获取最大电机速度(6080)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque 输出参数，成功时返回最大电机速度（rpm）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_maxMotorSpeed",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_maxMotorSpeed(uint hMaster, int nodeId, ref uint speed);

        /**
         * @brief 设置最大电机速度(6080)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque  最大电机速度（rpm）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_maxMotorSpeed",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_maxMotorSpeed(uint hMaster, int nodeId, uint speed);

        /**
         * @brief 设置最大转矩(6072)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque  最大转矩（0.001倍额定转矩）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_maxTorque",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_maxTorque(uint hMaster, int nodeId, uint torque);

        /**
         * @brief 获取最大转矩(6072)
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param torque 输出参数，成功时返回最大转矩（0.001倍额定转矩）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_maxTorque",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_maxTorque(uint hMaster, int nodeId, ref uint torque);

        /**
         * @brief 获取VM模式下的速度限制值
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param minSpeed 最小速度（rpm）
         * @param maxSpeed 最大速度（rpm）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_vmSpeedLimit",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_vmSpeedLimit(uint hMaster, int nodeId, ref uint minSpeed, ref uint maxSpeed);

        /**
         * @brief 设置VM模式下的速度限制值
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param minSpeed 最小速度（rpm）
         * @param maxSpeed 最大速度（rpm）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_vmSpeedLimit",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_vmSpeedLimit(uint hMaster, int nodeId, uint minSpeed, uint maxSpeed);

        /**
        * @brief 设置用户单位的转换系数
         * @param hMaster 主站对象句柄
        * @param nodeId 轴地址
        * @param factor 转换系数（电机编码器单位/用户单位）
        * @return 0 成功；其它 失败
        */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_unitsFactor",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_unitsFactor(uint hMaster, int nodeId, double factor);

        /**
         * @brief 获取用户单位的转换系数
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param factor 转换系数（电机编码器单位/用户单位）
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_unitsFactor",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_unitsFactor(uint hMaster, int nodeId, ref double factor);

        /**
         * @brief  设置DO输出
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param nDOs DO值：每位表示一路DO：bit0表示DO1；bit1表示DO2，以此类推
         *                  1 输出高电平；0 输出低电平
         * @param bSDO 1 使用SDO输出；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_DOs",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_DOs(uint hMaster, int nodeId, uint nDOs, int bSDO);

        /**
         * @brief  读取DI输入
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param nDIs DI值：每位表示一路DI：bit0表示DI1；bit1表示DI2，以此类推
         *                  1 输入高电平；0 输入低电平
         * @param bSDO 1 使用SDO读取；0 使用PDO
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_get_DIs",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_get_DIs(uint hMaster, int nodeId, ref uint nDIs, int bSDO);

        /**
         * @brief  设置VDI值
         * @param hMaster 主站对象句柄
         * @param nodeId 轴地址
         * @param nVDIs VDI值：每位表示一路VDI：bit0表示VDI1；bit1表示VDI2，以此类推
         *                  1 输出高电平；0 输出低电平
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_set_VDIs",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_set_VDIs(uint hMaster, int nodeId, uint nVDIs);

        /**
         * @brief 保存所有参数到设备
         * @param hMaster 主站对象句柄
         * @param nodeId 从站地址
         * @param timeoutMS 超时时间，单位：ms
         * @return 0 成功；其它 失败
         */
        [DllImport("NimServoSDK.dll", EntryPoint = "Nim_save_AllParams",CallingConvention = CallingConvention.Cdecl)]
        public static extern int Nim_save_AllParams(uint hMaster, int nodeId, int timeoutMS);
    }
}
