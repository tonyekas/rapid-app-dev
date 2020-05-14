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
    public static class OrderDB
    {
        
        public static List<int> GetOrderIDs()
        {
            List<int> orders = new List<int>(); // empty List
            int ord; // for reading

            using (SqlConnection con = NorthWind.GetConnection()) // connection established
            {
                string query = "SELECT OrderID  FROM Orders ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read()) // while reading ..
                        {
                            ord = (int)reader["OrderID"]; // fill up data
                            
                            orders.Add(ord); // add to the list
                        }
                    }
                }
            }

            return orders;

        }

        public static Order GetOrderByID(int orderID)
        {
            //List<Order> orders = new List<Order>();
            Order order = null;
            using (SqlConnection con = NorthWind.GetConnection())
            {
                string query =  "SELECT OrderID, CustomerID, OrderDate, RequiredDate, ShippedDate " +
                                "FROM Orders " +
                                "WHERE OrderID = @OrderID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderID);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read()) // if order is given with ID
                    {
                        order = new Order(); // fill up data
                        order.OrderID = (int)reader["OrderID"];
                        order.CustomerID = (string)reader["CustomerID"];
                        order.OrderDate = (DateTime)reader["Orderdate"];
                        order.RequiredDate = (DateTime)reader["RequiredDate"];
                        int col = reader.GetOrdinal("ShippedDate");
                        if (reader.IsDBNull(col))
                        {
                            order.ShippedDate = null;
                        }
                        else
                        {
                            order.ShippedDate = Convert.ToDateTime(reader["ShippedDate"]);

                        }
                    }// command object ended


                }// end of connection
            }

            return order;
        }

        // update Shipping Date
        public static bool UpdateShippingDate(Order oldDate, Order newDate)
        {
            bool success = false; // if no shipping date
            using (SqlConnection con = NorthWind.GetConnection())
            {
                string updateStatement = "UPDATE ORDERS SET " +         // not sure if it's ORDER or Shippedate??
                                          "  ShippedDate = @NewShippedDate " + // only ShippedDate can be updated
                                          "WHERE OrderID = @OldOrderID " + // identifies member
                                          "  AND CustomerID = @OldCustomerID " + // optimistic concurrency
                                          "  AND OrderDate = @OldOrderDate " +
                                          "  AND RequiredDate = @OldRequiredDate " +
                                           "  AND (ShippedDate = @OldShippedDate OR " + // either equal or both are null
                                          "   ShippedDate IS NULL AND @OldShippedDate IS NULL)";
                using (SqlCommand cmd = new SqlCommand(updateStatement, con))
                {
                    // provide values for parameters
                    // for every new column (new or old)that allows null have to check if null
                    if (newDate.ShippedDate == null)
                        cmd.Parameters.AddWithValue("@NewShippedDate", DBNull.Value);
                    else
                     cmd.Parameters.AddWithValue("@NewShippedDate", (DateTime)oldDate.ShippedDate);
                    cmd.Parameters.AddWithValue("@OldOrderID", oldDate.OrderID);
                    cmd.Parameters.AddWithValue("@OldCustomerID", oldDate.CustomerID);
                    cmd.Parameters.AddWithValue("@OldOrderDate", oldDate.OrderDate);
                    cmd.Parameters.AddWithValue("@OldRequiredDate", oldDate.RequiredDate);
                    // for every old column that allows null, also have to check if null
                    if (oldDate.ShippedDate == null)
                        cmd.Parameters.AddWithValue("@OldShippedDate", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@OldShippedDate", (DateTime)
                            oldDate.ShippedDate);

                    // open connection
                    con.Open();
                    // execute UPDATE command
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                        success = true;
                } // end of Object

            } // here the connection is close

            return success;
        } // end of public bool

    } // end of class

}// end of Namespace
