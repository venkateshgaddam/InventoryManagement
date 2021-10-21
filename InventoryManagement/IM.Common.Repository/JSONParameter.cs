using Npgsql;
using NpgsqlTypes;
using System.Data;
using static Dapper.SqlMapper;

namespace IM.Common.Repository
{
    /// <summary>
    /// Adds Json as parameter to command
    /// </summary>
    public class JsonParameter : ICustomQueryParameter
    {
        private readonly string _value;
        /// <summary>
        /// Initializes Json parameter
        /// </summary>
        /// <param name="value"></param>
        public JsonParameter(string value)
        {
            _value = value;
        }
        /// <summary>
        /// Adds parameter to command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        public void AddParameter(IDbCommand command, string name)
        {
            var parameter = new NpgsqlParameter(name, NpgsqlDbType.Json)
            {
                Value = _value
            };

            command.Parameters.Add(parameter);
        }
    }
}
