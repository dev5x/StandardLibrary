using System;
using System.Diagnostics;

/* dev5x.com (c) 2020
 * 
 * Utility Class
 * Handles general functions
*/

namespace dev5x.StandardLibrary
{
    public static class Utility
    {
        public static bool IsNumeric(string Value)
        {
            // Return true if value is a number
            return int.TryParse(Value, out _);
        }

        public static void ShellExec(string AppFile, string ParmList)
        {
            // Launch process
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(AppFile, ParmList)
                {
                    ErrorDialog = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Process.Start(psi);
            }
            catch
            {
                return;
            }
        }

        public static void OpenByAssociation(string FileName)
        {
            // Open the file using the Windows association
            try
            {
                if (FileName.Length > 0)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(FileName)
                    {
                        ErrorDialog = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };
                    Process.Start(psi);
                }
            }
            catch
            {
                return;
            }
        }

        public static string GetDateStamp()
        {
            try
            {
                return DateTime.Now.ToString("_yyyyMMddHHmmss");
            }
            catch
            {
                return "_????";
            }
        }
    }
}