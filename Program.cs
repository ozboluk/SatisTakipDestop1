using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.Entity;
namespace baybars
{
    class Program
    {
        static BarkodcuboEntities db = new BarkodcuboEntities();
        private static int WH_KEYBOARD_LL = 13;
        private static int WM_KEYDOWN = 0x0100;
        private static IntPtr hook = IntPtr.Zero;
        private static LowLevelKeyboardProc llkProcedure = HookCallback;
        static int sw_hide = 0;

        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static void Main(string[] args)
        {
            IntPtr myWindow = GetConsoleWindow();
            ShowWindow(myWindow, sw_hide);
            hook = SetHook(llkProcedure);
            Application.Run();
            UnhookWindowsHookEx(hook);


        }
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam, char[] arr);

        static List<char> harfekle = new List<char>();
        static int vkCode = 0;
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam, char[] ar)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {


                vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode).ToString() == "OemPeriod")
                {
                    Console.Out.Write(".");
                    StreamWriter output = new StreamWriter(@"C:\ProgramData1\mylog.txt", true);
                    output.Write(".");
                    output.Close();
                }
                else if (((Keys)vkCode).ToString() == "Oemcomma")
                {
                    Console.Out.Write(",");
                    StreamWriter output = new StreamWriter(@"C:\ProgramData1\mylog.txt", true);
                    output.Write(",");
                    output.Close();
                }
                else if (((Keys)vkCode).ToString() == "Space")
                {
                    Console.Out.Write(" ");
                    StreamWriter output = new StreamWriter(@"C:\ProgramData1\mylog.txt", true);
                    output.Write(" ");
                    output.Close();
                }
                else
                {  //Barkodda çalışan yer
                    try
                    {

                        var bu1 = (Keys)vkCode;
                        var chary = Convert.ToChar(bu1);
                        Console.Out.Write(bu1);
                        harfekle.Add(chary);
                        if (bu1 == Keys.Enter)
                        {
                            if (harfekle.Count > 0)

                            {
                                return Kaydedici(nCode, wParam, lParam, harfekle);
                            }
                            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                        }
                    }
                    catch

                    {

                        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                    }


                }


            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }


        private static IntPtr Kaydedici(int nCode, IntPtr wParam, IntPtr lParam, List<char> ar)

        {

            try
            {

                
                string barkod = ConvertStringArrayToString(ar);
            
                    Bay_SatisTarihi bay_SatisTarihi = new Bay_SatisTarihi
                    {
                        Barkod = barkod,
                        SatisTarihi = DateTime.Now,
                        SatisAdeti = 1
                    };
                    db.Bay_SatisTarihi.Add(bay_SatisTarihi);
                    db.SaveChanges();
                    harfekle.Clear();
                
        
            }
            catch 
            {
             

            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        static string ConvertStringArrayToString(List<char> array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char value in array)
            {
                builder.Append(value);

            }
            return builder.ToString();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);
    }
}
