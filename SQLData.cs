using System;
using System.Data;
using System.Data.SqlClient;

/* dev5x.com (c) 2020
 * 
 * SQLData class
 * SQL DB communications
*/

namespace dev5x.StandardLibrary
{
    public class SQLData : BaseClass, IDisposable
    {
        private SqlConnection _sqlConnection = null;
        private SqlCommand _sqlCommand = null;
        private SqlParameterCollection _sqlParameters = new SqlCommand().Parameters;
        private readonly string _connectionString = string.Empty;
        private int _sqlCommandTimeOut = 90;

        #region Constructor
        public SQLData(string ConnectionString)
        {
            // Init objects
            try
            {
                _Worker worker = new _Worker();
                _connectionString = worker.DecryptConnectionString(ConnectionString);
            }
            catch { }
        }
        #endregion

        #region Properties
        public int CommandTimeout
        {
            // SQL timeout for command execution
            get { return _sqlCommandTimeOut; }
            set { _sqlCommandTimeOut = value; }
        }

        public ConnectionState ConnectionState
        {
            // SQL connection state
            get 
            {
                if (_sqlConnection != null)
                {
                    return _sqlConnection.State;
                }
                else
                {
                    return ConnectionState.Closed;
                }
            }
        }

        public string ConnectionString
        {
            // Decrypted connection string
            get { return _connectionString; }
        }
        #endregion

        #region Private Methods
        private void InitCommand(string sqlStmt, CommandType commandType)
        {
            // Create SQL command
            try
            {
                // Open connection if it was closed
                if (_sqlConnection == null)
                {
                    OpenConnection();
                }

                // Recreate command object
                if (_sqlCommand != null)
                {
                    _sqlCommand.Dispose();
                }

                _sqlCommand = new SqlCommand(sqlStmt, _sqlConnection)
                {
                    CommandTimeout = _sqlCommandTimeOut,
                    CommandType = commandType
                };

                // Attach parameters to the command object (if any were specified)
                AttachParameters();
            }
            catch (Exception ex)
            {
                SetErrorMessage("InitCommand - " + ex.Message);
            }
        }

        private void AttachParameters()
        {
            // Attach the parms to the command object
            try
            {
                foreach (SqlParameter parm in _sqlParameters)
                {
                    _sqlCommand.Parameters.Add(parm.ParameterName, parm.SqlDbType);
                    _sqlCommand.Parameters[parm.ParameterName].Value = parm.Value;
                }

                // Clear the parms after they have been added to the command object
                _sqlParameters.Clear();
                _sqlParameters = new SqlCommand().Parameters;
            }
            catch (Exception ex)
            {
                SetErrorMessage("AttachParameters - " + ex.Message);
            }
        }
        #endregion

        #region Public Methods
        public void OpenConnection()
        {
            // Open SQL connection
            try
            {
                _sqlConnection = new SqlConnection(_connectionString);
                _sqlConnection.Open();
            }
            catch (Exception ex)
            {
                SetErrorMessage("OpenConnection - " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            // Close and dispose SQL objects
            try
            {
                if (_sqlCommand != null)
                {
                    _sqlCommand.Dispose();
                }

                if (_sqlConnection != null)
                {
                    _sqlConnection.Dispose();
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("CloseConnection - " + ex.Message);
            }
        }

        public void AddParameter(string Name, object Value, SqlDbType DataType)
        {
            // Add parm to the collection
            try
            {
                SqlParameter sqlParm = new SqlParameter(Name, Value);
                sqlParm.SqlDbType = DataType;

                _sqlParameters.Add(sqlParm);
            }
            catch (Exception ex)
            {
                SetErrorMessage("AddParameter - " + ex.Message);
            }
        }

        public SqlDataReader ExecuteDataReader(string Statement, CommandType CmdType)
        {
            // Execute SQL statement and return a data reader
            try
            {
                InitCommand(Statement, CmdType);

                return _sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                SetErrorMessage("ExecuteDataReader - " + ex.Message);
                return null;
            }
        }

        public DataSet ExecuteDataSet(string Statement, CommandType CmdType)
        {
            // Execute SQL statement and return a data set
            try
            {
                InitCommand(Statement, CmdType);

                // Create the DataAdapter & DataSet
                using (SqlDataAdapter da = new SqlDataAdapter(_sqlCommand))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                SetErrorMessage("ExecuteDataSet - " + ex.Message);
                return null;
            }
        }

        public int ExecuteNonQuery(string Statement, CommandType CmdType)
        {
            // Execute SQL statement and return rows affected
            try
            {
                InitCommand(Statement, CmdType);

                return _sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SetErrorMessage("ExecuteNonQuery - " + ex.Message);
                return 0;
            }
        }

        public object ExecuteScalar(string Statement, CommandType CmdType)
        {
            // Execute SQL statement and return first column in the first row (ignore others)
            try
            {
                InitCommand(Statement, CmdType);

                return _sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                SetErrorMessage("ExecuteScalar - " + ex.Message);
                return null;
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
                if (_sqlParameters != null)
                {
                    _sqlParameters.Clear();
                }

                CloseConnection();
            }
        }
        #endregion
    }
}