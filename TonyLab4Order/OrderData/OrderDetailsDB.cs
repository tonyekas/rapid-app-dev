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
    public class OrderDetailsDB
    {


        public static List<OrderDetail> GetOrderDetailIDs(int orderID)

        {
            List<OrderDetail> orderDetail = new List<OrderDetail>();
            OrderDetail ordDetail;

            using (SqlConnection con = NorthWind.GetConnection())
            {
                string query = "SELECT OrderID, ProductID, UnitPrice, Quantity, Discount " +
                                "FROM [Order Details] " +
                                "WHERE OrderID = @OrderID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@OrderID", orderID);
                    con.Open();
                    cmd.Parameters.AddWithValue("@OrderID", orderID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ordDetail = new OrderDetail();
                            ordDetail.OrderID = (int)reader["OrderID"];
                            ordDetail.ProductID = (int)reader["ProductID"];
                            ordDetail.UnitPrice = (decimal)reader["UnitPrice"];
                            ordDetail.Quantity = (short)reader["Quantity"];
                            ordDetail.Discount = (float)reader["Discount"];

                            decimal afterDiscount = 1 - Convert.ToDecimal(ordDetail.Discount);
                            ordDetail.OrderTotal = (float)(ordDetail.UnitPrice * (afterDiscount) * ordDetail.Quantity);
                            orderDetail.Add(ordDetail);

                        } // end of reader
                    }
                } // end of using- recylced

            }// using get connection


            return orderDetail;
        }



    } // end of public class 

} // end of namespace


