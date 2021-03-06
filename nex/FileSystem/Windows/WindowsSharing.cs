﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace nex.FileSystem.Windows
{
    class WindowsSharing
    {
        #region enums
        public enum MethodStatus : uint
        {
            Success = 0, 	//Success
            AccessDenied = 2, 	//Access denied
            UnknownFailure = 8, 	//Unknown failure
            InvalidName = 9, 	//Invalid name
            InvalidLevel = 10, 	//Invalid level
            InvalidParameter = 21, 	//Invalid parameter
            DuplicateShare = 22, 	//Duplicate share
            RedirectedPath = 23, 	//Redirected path
            UnknownDevice = 24, 	//Unknown device or directory
            NetNameNotFound = 25 	//Net name not found
        }

        public enum ShareType : uint
        {
            DiskDrive = 0x0, 	//Disk Drive
            PrintQueue = 0x1, 	//Print Queue
            Device = 0x2, 	//Device
            IPC = 0x3, 	//IPC
            DiskDriveAdmin = 0x80000000, 	//Disk Drive Admin
            PrintQueueAdmin = 0x80000001, 	//Print Queue Admin
            DeviceAdmin = 0x80000002, 	//Device Admin
            IpcAdmin = 0x80000003 	//IPC Admin
        }
        #endregion

        #region Fields
        private readonly ManagementObject mWinShareObject;
        private WindowsSharing(ManagementObject obj)
        {
            mWinShareObject = obj;
        }

        #endregion

        #region Props
        public uint AccessMask
        {
            get
            {
                return Convert.ToUInt32(mWinShareObject["AccessMask"]);
            }
        }

        public bool AllowMaximum
        {
            get
            {
                return Convert.ToBoolean(mWinShareObject["AllowMaximum"]);
            }
        }

        public string Caption
        {
            get
            {
                return Convert.ToString(mWinShareObject["Caption"]);
            }
        }

        public string Description
        {
            get
            {
                return Convert.ToString(mWinShareObject["Description"]);
            }
        }

        public DateTime InstallDate
        {
            get
            {
                return Convert.ToDateTime(mWinShareObject["InstallDate"]);
            }
        }

        public uint MaximumAllowed
        {
            get
            {
                return Convert.ToUInt32(mWinShareObject["MaximumAllowed"]);
            }
        }

        public string Name
        {
            get
            {
                return Convert.ToString(mWinShareObject["Name"]);
            }
        }

        public string Path
        {
            get
            {
                return Convert.ToString(mWinShareObject["Path"]);
            }
        }

        public string Status
        {
            get
            {
                return Convert.ToString(mWinShareObject["Status"]);
            }
        }

        public ShareType Type
        {
            get
            {
                return (ShareType)Convert.ToUInt32(mWinShareObject["Type"]);
            }
        }
        #endregion

        #region Methods
        public MethodStatus Delete()
        {
            object result = mWinShareObject.InvokeMethod("Delete", new object[] { });
            uint r = Convert.ToUInt32(result);

            return (MethodStatus)r;
        }

        public static MethodStatus Create(string path, string name, ShareType type, uint maximumAllowed, string description, string password)
        {
            ManagementClass mc = new ManagementClass("Win32_Share");
            object[] parameters = new object[] { path, name, (uint)type, maximumAllowed, description, password, null };

            object result = mc.InvokeMethod("Create", parameters);
            uint r = Convert.ToUInt32(result);

            return (MethodStatus)r;
        }

        public static IList<WindowsSharing> GetAllShares()
        {
            IList<WindowsSharing> result = new List<WindowsSharing>();
            ManagementClass mc = new ManagementClass("Win32_Share");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                WindowsSharing share = new WindowsSharing(mo);
                result.Add(share);
            }

            return result;
        }

        public static WindowsSharing GetNamedShare(string name)
        {
            IList<WindowsSharing> shares = GetAllShares();

            foreach (WindowsSharing s in shares)
                if (s.Name == name)
                    return s;

            return null;
        }
        #endregion
    }
}