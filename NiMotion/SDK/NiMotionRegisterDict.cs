using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDKDemo
{
    public class NiMotionRegisterDict
    {
        public static Dictionary<string, int>  GetMotorSettingKeyDict()
        {
            return new Dictionary<string, int>()
            {
                {"MotorAddress",                0x00 },
                {"BaudRate",                    0x01 },
                {"DataFormat",                  0x02 },
                {"Segmentation",                0x1A },
                {"CurrentPosition",             0x21 },
                {"CurrentSpeed",                0x23 },
                {"MotorMode",                   0x39 },
                {"StartEnd",                    0x3A },
                {"EmergencyStop",               0x3B },
                {"FaultAction",                 0x3C },
                {"LocationSource",              0x3D },
                {"AngleSelection",              0x3E },
                {"EncoderUnit",                 0x3F },
                {"LoopSelection",               0x40 },
                {"LocationRecovery",            0x45 },
                {"MaxForwardRotationAngle",     0x46 },
                {"MinPosition",                 0x57 },
                {"MaxPosition",                 0x59 },
                {"WorkingTime",                 0x0C },
                {"MaxSpeed",                    0x5B },
                {"MinSpeed",                    0x5D },
                {"Acceleration",                0x5F },
                {"Deceleration",                0x61 },
                {"MaxAcceleration",             0x63 },
                {"MaxDeceleration",             0x65 },
                {"EmergencyStopDeceleration",   0x67 }
            };
        }

        public static Dictionary<string, int> GetMotorSettingLengthDict()
        {
            return new Dictionary<string, int>()
            {
                {"MotorAddress",                2 },
                {"BaudRate",                    2 },
                {"DataFormat",                  2 },
                {"Segmentation",                2 },
                {"CurrentPosition",             4 },
                {"CurrentSpeed",                4 },
                {"MotorMode",                   2 },
                {"StartEnd",                    2 },
                {"EmergencyStop",               2 },
                {"FaultAction",                 2 },
                {"LocationSource",              2 },
                {"AngleSelection",              2 },
                {"EncoderUnit",                 2 },
                {"LoopSelection",               2 },
                {"LocationRecovery",            2 },
                {"MaxForwardRotationAngle",     2 },
                {"MinPosition",                 4 },
                {"MaxPosition",                 4 },
                {"WorkingTime",                 4 },
                {"MaxSpeed",                    4 },
                {"MinSpeed",                    4 },
                {"Acceleration",                4 },
                {"Deceleration",                4 },
                {"MaxAcceleration",             4 },
                {"MaxDeceleration",             4 },
                {"EmergencyStopDeceleration",   4 }
            };
        }

        public static Dictionary<int, int> GetMotorSegmentDict()
        {
            return new Dictionary<int, int>()
            {
                {0, 200  },
                {1, 400  },
                {2, 800  },
                {3, 1600 },
                {4, 3200 }
            };
        }
    }
}
