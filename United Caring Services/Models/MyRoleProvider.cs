using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace United_Caring_Services.Models
{
    public class MyRoleProvider : RoleProvider
    {
        public MyRoleProvider() : base()
        {

        }

        public override string ApplicationName
        {
            get
            {
                return "MyRoleProvider";
            }

            set
            {
                
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
        }

        public override void CreateRole(string roleName)
        {
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return new string[] { };
        }

        public override string[] GetAllRoles()
        {
            return new string[] { "Admin", "User" };
        }

        public override string[] GetRolesForUser(string username)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT name FROM customUserRoles WHERE username  =  @user";
                cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.NVarChar) { Value = username });
                string role = (string)cmd.ExecuteScalar();
                connection.Close();
                return new string[] { role };
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
            {
                List<string> users = new List<string>();
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT username FROM customUserRoles WHERE role  =  '{roleName}'";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(reader.GetString(0));
                }
                connection.Close();
                return users.ToArray();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"exec getUserRoles @user = @user";
                cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.NVarChar) { Value = username });
                string role = (string)cmd.ExecuteScalar();
                connection.Close();
                return role.Equals(roleName, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {

        }

        public override bool RoleExists(string roleName)
        {
            return roleName == "Admin" || roleName == "User";
        }
    }
}