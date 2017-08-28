using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TwitterApp.Common;
using TwitterApp.Common.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterApp.Data.Providers
{
    /// <summary>
    /// User provider SQL DB + Dapper 
    /// </summary>
    public class UserProvider : IUserProvider, IDisposable
    {
        private string _connectionString = null;

        /// <summary>
        /// Default initialization static method.
        /// </summary>
        /// <returns></returns>
        public static UserProvider Create()
        {
            return new UserProvider(ConfigurationManager.ConnectionStrings["AppUser"].ConnectionString);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connectionString">SQL connection string to inject</param>
        public UserProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool SaveUser(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sqlQuery = $"EXEC [dbo].[SaveAppUser] {user.Id}, '{user.Email}', '{user.Password}', {(int)user.Type}";
                var idResult = db.ExecuteReader(sqlQuery);
                if (idResult.Read())
                {
                    if (int.TryParse(idResult[0].ToString(), out int id))
                    {
                        user.Id = id;
                        return true;
                    }
                }
            }

            return false;
        }

        public AppUser GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<AppUser>($"EXEC [dbo].[GetUser] '{email}'").SingleOrDefault();
            }
        }

        public AppUser GetUserById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(nameof(id));
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<AppUser>($"EXEC [dbo].[GetUserById] '{id}'").SingleOrDefault();
            }
        }

        public Task<bool> RemoveUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var affected = db.Execute($"EXEC [dbo].[DeleteAppUser] '{email}'");
                return Task.FromResult(affected > 0);
            }
        }

        public void Dispose()
        {
        }

        public Task<List<AppUser>> GetUsers()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return Task.FromResult(db.Query<AppUser>("EXEC [dbo].[GetUsers]").ToList());
            }
        }
    }
}
