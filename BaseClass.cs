using System;

/* dev5x.com (c) 2020
 * 
 * Base class
 * Contains:
 *  Error handling
*/

namespace dev5x.StandardLibrary
{
    public abstract class BaseClass
    {
        public event EventHandler ErrorMessage;

        // General error event handler
        internal void SetErrorMessage(string msg)
        {
            // Fire error message event if referenced
            ErrorMessage?.Invoke(msg, EventArgs.Empty);
        }
    }
}