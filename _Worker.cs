using System;
using System.Data.SqlClient;

/* dev5x.com (c) 2020
 * 
 * _Worker Class
 * General purpose internal methods
*/

namespace dev5x.StandardLibrary
{
    internal class _Worker : BaseClass
    {
        public string BuildConnectionString(string dataSource, string initialCatalog, string userID, string password, bool integratedSecurity)
        {
            try
            {
                // Build a sql connection string
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                Encryption encrypt = new Encryption();

                builder.DataSource = dataSource;
                builder.InitialCatalog = initialCatalog;

                if (integratedSecurity)
                {
                    builder.IntegratedSecurity = true;
                    builder.Pooling = false;  // Do not use pooling for integrated login (creates a pool for every user)
                }
                else
                {
                    builder.UserID = userID;
                    builder.Password = encrypt.Encrypt(password);
                }

                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                SetErrorMessage("BuildConnectionString - " + ex.Message);
                return string.Empty;
            }
        }

        public string DecryptConnectionString(string connectionString)
        {
            try
            {
                // Rebuild the sql connection string and decrypt the password
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                Encryption encrypt = new Encryption();

                builder.Password = encrypt.Decrypt(builder.Password);
                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                SetErrorMessage("DecryptConnectionString - " + ex.Message);
                return string.Empty;
            }
        }
    }
}