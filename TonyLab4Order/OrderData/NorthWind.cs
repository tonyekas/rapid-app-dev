using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Name: Tony Onyeka
 * Lab # 4
 * SAIT Software Development Program
 * Jan - May 2020
 * 
 * */

namespace OrderData
{
    public static class NorthWind
    {
        public static SqlConnection GetConnection()
        {
            string connectionString = @"Data Source=localhost\sqlexpress01;Initial Catalog=NORTHWND;Integrated Security=True";

            SqlConnection con = new SqlConnection(connectionString);

            return con;

        }
    }
}
