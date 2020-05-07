using System.Collections.Generic;

/* dev5x.com (c) 2020
 * 
 * DataList Class
 * Handles general data storage needs
*/

namespace dev5x.StandardLibrary
{
    public class DataList : BaseClass
    {
        private List<string> _list;

        #region Constructor
        public DataList()
        {
            _list = new List<string>();
        }

        public DataList(int Capacity)
        {
            _list = new List<string>(Capacity);
        }
        #endregion

        #region Properties
        public List<string> List
        {
            get { return _list; }
        }
        #endregion

        #region Public Methods
        public void Add(string Value)
        {
            // Add new item
            try
            {
                // Add unique items only
                if (!_list.Contains(Value))
                {
                    _list.Add(Value);
                }
            }
            catch
            {
                return;
            }
        }

        public bool Contains(string Value)
        {
            // Find existing item
            try
            {
                return _list.Contains(Value);
            }
            catch
            {
                return false;
            }
        }

        public void ImportArray(string[] Values)
        {
            // Import 1D array
            try
            {
                // Recreate the list object and set the size
                _list = new List<string>(Values.Length);

                // Add each array item to list
                foreach (string value in Values)
                {
                    _list.Add(value);
                }
            }
            catch
            {
                return;
            }
        }
        #endregion
    }
}