using System;
using System.Configuration;

/* dev5x.com (c) 2020
 * 
 * Config Class
 * Save and retrieve app settings and connection strings in .config files
*/

namespace dev5x.StandardLibrary
{
    public class Config : BaseClass
    {
        private DataList _allKeys = null;
        private readonly ExeConfigurationFileMap _configFileMap = new ExeConfigurationFileMap();
        private readonly string _configFilePath = string.Empty;
        private string _connectionString = string.Empty;

        #region Constructor
        public Config(string ConfigFilePath)
        {
            // Map the exe.config file
            _configFilePath = ConfigFilePath;
            _configFileMap.ExeConfigFilename = _configFilePath;
        }
        #endregion

        #region Properties
        public string ConfigFilePath
        {
            get { return _configFilePath; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
        #endregion

        #region Private Methods
        private void LoadAllKeys(string[] keys)
        {
            // Load the List with allkeys from appSettings
            try
            {
                _allKeys = new DataList();
                _allKeys.ImportArray(keys);
            }
            catch (Exception ex)
            {
                SetErrorMessage("LoadAllKeys - " + ex.Message);
            }
        }
        #endregion

        #region Public Methods
        public string GetAppSetting(string KeyName)
        {
            try
            {
                // Retrieve the config value for the keyname
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);

                // Load list with key values
                LoadAllKeys(config.AppSettings.Settings.AllKeys);

                // Return key value if key exists
                if (_allKeys.Contains(KeyName))
                {
                    return config.AppSettings.Settings[KeyName].Value;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetAppSetting - " + ex.Message);
                return string.Empty;
            }
        }

        public string GetAppSetting(string KeyName, string DefaultValue)
        {
            try
            {
                // Retrieve the config value for the keyname
                string settingValue = GetAppSetting(KeyName);

                // Return key value if key exists
                if (settingValue.Length > 0)
                {
                    return settingValue;
                }
                else
                {
                    // No value found, return the default
                    return DefaultValue;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetAppSetting - " + ex.Message);
                return string.Empty;
            }
        }

        public string[] GetAppSettings()
        {
            try
            {
                // Retrieve an array of all keynames
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);

                // Return array of keys
                return config.AppSettings.Settings.AllKeys;
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetAppSettings - " + ex.Message);
                return new string[0];
            }
        }

        public void SaveAppSetting(string KeyName, string KeyValue)
        {
            try
            {
                // Save the config value for the keyname in the appSettings section
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);

                // Load list with key values
                LoadAllKeys(config.AppSettings.Settings.AllKeys);

                // Determine whether the key exists and update app settings
                if (_allKeys.Contains(KeyName))
                {
                    config.AppSettings.Settings[KeyName].Value = KeyValue;
                }
                else
                {
                    config.AppSettings.Settings.Add(KeyName, KeyValue);
                }

                config.Save();
            }
            catch (Exception ex)
            {
                SetErrorMessage("SaveAppSetting - " + ex.Message);
            }
        }

        public string GetConnectionString(string KeyName)
        {
            try
            {
                // Retrieve the connection string for the keyname
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);
                ConnectionStringSettings connSettings = config.ConnectionStrings.ConnectionStrings[KeyName];
                
                // Get connection string
                if (connSettings == null)
                {
                    _connectionString = string.Empty;
                }
                else
                {
                    _connectionString = connSettings.ConnectionString;
                }

                // Return connection string
                return _connectionString;
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetConnectionString - " + ex.Message);
                return string.Empty;
            }
        }

        public string[] GetConnectionStrings()
        {
            try
            {
                // Retrieve the connection string keys
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);
                ConnectionStringSettingsCollection connSettings = config.ConnectionStrings.ConnectionStrings;

                // Remove the default connection (comes from machine.config)
                if (connSettings[0].Name == "LocalSqlServer")
                {
                    connSettings.RemoveAt(0);
                }

                string[] connections = new string[connSettings.Count];
                int cnt = 0;

                foreach (ConnectionStringSettings cs in connSettings)
                {
                    connections[cnt] = cs.Name;
                    cnt++;
                }

                // Return connection names array
                return connections;
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetConnectionStrings - " + ex.Message);
                return new string[0];
            }
        }

        public void SaveConnectionString(string KeyName, string KeyValue)
        {
            try
            {
                // Save the connection string for the keyname in the ConnectionStrings section
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(_configFileMap, ConfigurationUserLevel.None);
                ConnectionStringSettings connSettings = config.ConnectionStrings.ConnectionStrings[KeyName];

                if (connSettings == null)
                {
                    config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(KeyName, KeyValue));
                }
                else
                {
                    connSettings.ConnectionString = KeyValue;
                }

                config.Save();
                _connectionString = KeyValue;
            }
            catch (Exception ex)
            {
                SetErrorMessage("SaveConnectionString - " + ex.Message);
            }
        }
        #endregion
    }
}