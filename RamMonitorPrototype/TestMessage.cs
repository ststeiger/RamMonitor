
namespace RamMonitorPrototype
{
    class TestMessage
    {


        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int MessageBox(int hWnd, string text, string caption, uint type);
        // MessageBox(0, "Text", "Caption", 0);

        public static void MsgBox(string text, string caption)
        {
            MessageBox(0, "Text", "Caption", 0);
        }

        public static void MsgBox(string text)
        {
            MsgBox( text, "Caption");
        }




        public static void Test()
        {
            MessageBox(0, "Text", "Caption", 0);
        }

    }
}
