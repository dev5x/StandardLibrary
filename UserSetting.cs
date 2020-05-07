using System;
using Microsoft.Win32;

/* dev5x.com (c) 2020
 * 
 * UserSetting Class
 * Save and retrieve user app settings in registry
*/

namespace dev5x.StandardLibrary
{
    public class UserSetting : BaseClass, IDisposable
    {
		private RegistryKey _regApp = null;

        public UserSetting(string SubKeyName)
		{
			try
			{
                // Make sure the base keys are in place
                using (RegistryKey regSoftware = Registry.CurrentUser.CreateSubKey("Software"))
                using (RegistryKey regDev5x = regSoftware.CreateSubKey("dev5x"))
                {
                    _regApp = regDev5x.CreateSubKey(SubKeyName);
                }
			}
            catch { }
        }

        public void SaveProgramSetting(string KeyName, string KeyValue)
		{
			// Save program setting
			try
			{
				_regApp.SetValue(KeyName, KeyValue);
			}
			catch(Exception ex)
			{
                SetErrorMessage("SaveProgramSetting - " + ex.Message);
			}
		}
		
        public string GetProgramSetting(string KeyValue, string DefaultValue)
		{
			// Get program setting
			try
			{
				return _regApp.GetValue(KeyValue, DefaultValue).ToString();
			}
			catch(Exception ex)
			{
                SetErrorMessage("GetProgramSetting - " + ex.Message);
				return DefaultValue;
			}
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed objects
                if (_regApp != null)
                {
                    _regApp.Dispose();
                }
            }
        }
    }
}