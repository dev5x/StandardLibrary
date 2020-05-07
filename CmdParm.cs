using System;
using System.Collections.Generic;

/* dev5x.com (c) 2020
 * 
 * CmdParm Class
 * Parse the command parms and store in a dictionary object
*/

namespace dev5x.StandardLibrary
{
    public class CmdParm : BaseClass
    {
        private readonly Dictionary<string, string> _allParms = new Dictionary<string, string>();

        #region Constructor
        public CmdParm(string[] CommandParms)
        {
            try
            {
                // Load the dictionary object with command parms

                foreach (string parm in CommandParms)
                {
                    string parmVal = parm.Replace("/", "").Trim();
                    int pos = parmVal.IndexOf("=");

                    if (pos > 0)
                    {
                        if (!_allParms.ContainsKey(parmVal.Substring(0, pos).ToUpper()))
                        {
                            _allParms.Add(parmVal.Substring(0, pos).ToUpper(), parmVal.Substring(pos + 1));
                        }
                    }
                    else
                    {
                        if (parmVal.Length > 0 && !_allParms.ContainsKey(parmVal.ToUpper()))
                        {
                            _allParms.Add(parmVal.ToUpper(), " "); // No value, use space placeholder to indicate a valid entry
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Public Methods
        public string GetParmValue(string KeyName)
        {
            // Return the value for the key
            try
            {
                if (ContainsParmKey(KeyName))
                {
                    return _allParms[KeyName.ToUpper()].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("GetParmValue - " + ex.Message);
                return string.Empty;
            }
        }

        public bool ContainsParmKey(string KeyName)
        {
            // Tests for existence of key
            try
            {
                return _allParms.ContainsKey(KeyName.ToUpper());
            }
            catch (Exception ex)
            {
                SetErrorMessage("ContainsParmKey - " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}