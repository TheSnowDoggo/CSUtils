using System.Runtime.InteropServices;
namespace CSUtils
{
    public static class DialogBox
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox

        // To indicate the buttons displayed in the message box, specify one of the following values.
        #region MBButtonConstants
        public const uint MB_ABORTRETRIGNORER = (uint)0x00000002L;
        public const uint MB_CANCELTRYCONTINUE = (uint)0x00000006L;
        public const uint MB_HELP = (uint)0x00004000L;
        public const uint MB_OK = (uint)0x00000000L;
        public const uint MB_OKCANCEL = (uint)0x00000001L;
        public const uint MB_RETRYCANCEL = (uint)0x00000005L;
        public const uint MB_YESNO = (uint)0x00000004L;
        public const uint MB_YESNOCANCEL = (uint)0x00000003L;
        #endregion

        // To display an icon in the message box, spcify one of the following values.
        #region MBIconConstants
        public const uint MB_ICONEXCLAMATION = (uint)0x00000030L;
        public const uint MB_ICONWARNING = (uint)0x00000030L;
        public const uint MB_ICONINFORMATION = (uint)0x00000040L;
        public const uint MB_ICONASTERISK = (uint)0x00000040L;
        public const uint MB_ICONQUESTION = (uint)0x00000020L;
        public const uint MB_ICONSTOP = (uint)0x00000010L;
        public const uint MB_ICONERROR = (uint)0x00000010L;
        public const uint MB_ICONHAND = (uint)0x00000010L;
        #endregion

        // To indicate the default button, specify one of the following values.
        #region MBDefaultButtonConstants
        public const uint MB_DEFBUTTON1 = (uint)0x00000000L;
        public const uint MB_DEFBUTTON2 = (uint)0x00000100L;
        public const uint MB_DEFBUTTON3 = (uint)0x00000200L;
        public const uint MB_DEFBUTTON4 = (uint)0x00000300L;
        #endregion

        // To indicate the modality of the dialog box, specify one of the following values.
        #region MBModalityConstants
        public const uint MB_APPLMODAL = (uint)0x00000000L;
        public const uint MB_SYSTEMMODAL = (uint)0x00001000L;
        public const uint MB_TASKMODAL = (uint)0x00002000L;
        #endregion

        // To specify other options, use one or more of the following values.
        #region MBOtherOptionsConstants
        public const uint MB_DEFAULT_DESKTOP_ONLY = (uint)0x00020000L;
        public const uint MB_RIGHT = (uint)0x00080000L;
        public const uint MB_RTLREADING = (uint)0x00100000L;
        public const uint MB_SETFOREGROUND = (uint)0x00010000L;
        public const uint MB_TOPMOST = (uint)0x00040000L;
        public const uint MB_SERVICE_NOTIFICATION = (uint)0x00200000L;
        #endregion

        // The codes returned by the message box.
        #region IDReturnCodeConstants
        public const int IDABORT = 3;
        public const int IDCANCEL = 2;
        public const int IDCONTINUE = 211;
        public const int IDIGNORE = 5;
        public const int IDNO = 7;
        public const int IDOK = 1;
        public const int IDRETRY = 4;
        public const int IDTRYAGAIN = 10;
        public const int IDYES = 6;
        #endregion 

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        public static int Box(IntPtr hWnd, string lpText, string lpCaption, uint uType = 0)
        {
            return MessageBox(hWnd, lpText, lpCaption, uType);
        }

        public static int Box(string lpText, string lpCaption, uint uType = 0)
        {
            return MessageBox(IntPtr.Zero, lpText, lpCaption, uType);
        }
    }
}