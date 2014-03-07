using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PhotoTagManager.Lib.WinAPI
{
    static class IconExtractor
    {
        #region API Declarations
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public enum FolderType
        {
            Closed,
            Open
        }

        public enum IconSize
        {
            Large,
            Small
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyIcon(IntPtr hIcon);

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint SHGFI_OPENICON = 0x000000002;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint SHGFI_PIDL = 0x000000008;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        private enum CSIDL
        {
            CSIDL_DESKTOP = 0x0000,    // <desktop>
            CSIDL_INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
            CSIDL_PROGRAMS = 0x0002,    // Start Menu\Programs
            CSIDL_CONTROLS = 0x0003,    // My Computer\Control Panel
            CSIDL_PRINTERS = 0x0004,    // My Computer\Printers
            CSIDL_PERSONAL = 0x0005,    // My Documents
            CSIDL_FAVORITES = 0x0006,    // <user name>\Favorites
            CSIDL_STARTUP = 0x0007,    // Start Menu\Programs\Startup
            CSIDL_RECENT = 0x0008,    // <user name>\Recent
            CSIDL_SENDTO = 0x0009,    // <user name>\SendTo
            CSIDL_BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
            CSIDL_STARTMENU = 0x000b,    // <user name>\Start Menu
            CSIDL_MYDOCUMENTS = 0x000c,    // logical "My Documents" desktop icon
            CSIDL_MYMUSIC = 0x000d,    // "My Music" folder
            CSIDL_MYVIDEO = 0x000e,    // "My Videos" folder
            CSIDL_DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
            CSIDL_DRIVES = 0x0011,    // My Computer
            CSIDL_NETWORK = 0x0012,    // Network Neighborhood (My Network Places)
            CSIDL_NETHOOD = 0x0013,    // <user name>\nethood
            CSIDL_FONTS = 0x0014,    // windows\fonts
            CSIDL_TEMPLATES = 0x0015,
            CSIDL_COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
            CSIDL_COMMON_PROGRAMS = 0X0017,    // All Users\Start Menu\Programs
            CSIDL_COMMON_STARTUP = 0x0018,    // All Users\Startup
            CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
            CSIDL_APPDATA = 0x001a,    // <user name>\Application Data
            CSIDL_PRINTHOOD = 0x001b,    // <user name>\PrintHood
            CSIDL_LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
            CSIDL_ALTSTARTUP = 0x001d,    // non localized startup
            CSIDL_COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
            CSIDL_COMMON_FAVORITES = 0x001f,
            CSIDL_INTERNET_CACHE = 0x0020,
            CSIDL_COOKIES = 0x0021,
            CSIDL_HISTORY = 0x0022,
            CSIDL_COMMON_APPDATA = 0x0023,    // All Users\Application Data
            CSIDL_WINDOWS = 0x0024,    // GetWindowsDirectory()
            CSIDL_SYSTEM = 0x0025,    // GetSystemDirectory()
            CSIDL_PROGRAM_FILES = 0x0026,    // C:\Program Files
            CSIDL_MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
            CSIDL_PROFILE = 0x0028,    // USERPROFILE
            CSIDL_SYSTEMX86 = 0x0029,    // x86 system directory on RISC
            CSIDL_PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
            CSIDL_PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
            CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
            CSIDL_COMMON_TEMPLATES = 0x002d,    // All Users\Templates
            CSIDL_COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
            CSIDL_COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
            CSIDL_ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
            CSIDL_CONNECTIONS = 0x0031,    // Network and Dial-up Connections
            CSIDL_COMMON_MUSIC = 0x0035,    // All Users\My Music
            CSIDL_COMMON_PICTURES = 0x0036,    // All Users\My Pictures
            CSIDL_COMMON_VIDEO = 0x0037,    // All Users\My Video
            CSIDL_CDBURN_AREA = 0x003b    // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, CSIDL nFolder, ref IntPtr ppidl);

        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        #endregion

        public static ImageSource GetFolderIcon(string path, IconSize size, FolderType folderType)
        {
            // Need to add size check, although errors generated at present!    
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;

            AddPictureOptions(size, folderType, ref flags);

            // Get the folder icon    
            var shfi = new SHFILEINFO();

            var res = SHGetFileInfo(path,
                FILE_ATTRIBUTE_DIRECTORY,
                out shfi,
                (uint)Marshal.SizeOf(shfi),
                flags);

            if (res == IntPtr.Zero)
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());

            // Load the icon from an HICON handle  
            var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            shfi.hIcon,
                            Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            icon.Freeze();

            DestroyIcon(shfi.hIcon);        // Cleanup    
            
            return icon;
        }

        private static void AddPictureOptions(IconSize size, FolderType folderType, ref uint flags)
        {
            if (FolderType.Open == folderType)
            {
                flags += SHGFI_OPENICON;
            }
            if (IconSize.Small == size)
            {
                flags += SHGFI_SMALLICON;
            }
            else
            {
                flags += SHGFI_LARGEICON;
            }
        }

        public static ImageSource GetSpecialFolderIcon(Environment.SpecialFolder folder, IconSize size, FolderType folderType)
        {
            uint flags = SHGFI_ICON | SHGFI_PIDL;
            AddPictureOptions(size, folderType, ref flags);

            var shfi = new SHFILEINFO();
            var csidl = SpecialFolderToCIDL(folder);

            IntPtr ptrDir = IntPtr.Zero;
            SHGetSpecialFolderLocation(IntPtr.Zero, csidl, ref ptrDir);

            var res = SHGetFileInfo(ptrDir, 0, out shfi, (uint)Marshal.SizeOf(shfi), flags);

            Marshal.FreeCoTaskMem(ptrDir);


            var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(shfi.hIcon,
                            Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            icon.Freeze();
            DestroyIcon(shfi.hIcon);

            return icon;

        }

        private static CSIDL SpecialFolderToCIDL(Environment.SpecialFolder folder)
        {
            CSIDL retValue;
            switch (folder)
            {
                case Environment.SpecialFolder.MyComputer:
                    retValue = CSIDL.CSIDL_DRIVES;
                    break;
                case Environment.SpecialFolder.MyDocuments:
                    retValue = CSIDL.CSIDL_MYDOCUMENTS;
                    break;
                case Environment.SpecialFolder.MyPictures:
                    retValue = CSIDL.CSIDL_MYPICTURES;
                    break;
                default:
                    throw new ArgumentException("Special folder is not supported");
            }

            return retValue;
        }

    }

    

    
}
