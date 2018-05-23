using System;
using System.Runtime.InteropServices;

namespace FlightWatch
{
    public class B777_SDK
    {
        public const string PMDG_777X_DATA_NAME = "PMDG_777X_Data";
        public const string PMDG_777X_CONTROL_NAME = "PMDG_777X_Control";

        // B777 data structure
        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
        public struct PMDG_777X_Data
        {
            // Warnings
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public byte[] WARN_annunMASTER_WARNING;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public byte[] WARN_annunMASTER_CAUTION;

            public byte WARN_annunFLT_CONT;
            public byte WARN_annunIRS;
            public byte WARN_annunFUEL;
            public byte WARN_annunELEC;
            public byte WARN_annunAPU;
            public byte WARN_annunOVHT_DET;
            public byte WARN_annunANTI_ICE;
            public byte WARN_annunHYD;
            public byte WARN_annunDOORS;
            public byte WARN_annunENG;
            public byte WARN_annunOVERHEAD;
            public byte WARN_annunAIR_COND;

            // The rest of the controls and indicators match their standard FSX counterparts
            // and can be accessed using the standard SimConnect means.


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 156)] public byte[] reserved;
            // ReSharper restore FieldCanBeMadeReadOnly.Local
        }

        public const UInt32 THIRD_PARTY_EVENT_ID_MIN = 0x00011000;		// equals to 69632

        public enum PMDGEvents : uint
        {
            // Add it when we need it
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct PMDG_777X_Control
        {
            public PMDGEvents Event;
            public UInt32 Parameter;
        }

        // Notification Group priority values (not in managed simmconnect ?)
        public enum SIMCONNECT_GROUP_PRIORITY : uint
        {
            HIGHEST = 1,      // highest priority
            HIGHEST_MASKABLE = 10000000,      // highest priority that allows events to be masked
            STANDARD = 1900000000,      // standard priority
            DEFAULT = 2000000000,      // default priority
            LOWEST = 4000000000,      // priorities lower than this will be ignored
        }
    }
}
