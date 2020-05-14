using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderData;
/*
 * Name: Tony Onyeka
 * Lab # 4
 * SAIT Software Development Program
 * Jan - May 2020
 * 
 * */

namespace TonyLab4Order
{
    public partial class CustomerForm : Form
    {
        Order current = null;
                       
        List<int> orderIDs = null;

        List<OrderDetail> ordDetails = new List<OrderDetail>(); // -- Used for details

        //OrderDetail orderdet = null; // = new List<OrderDetail>();
        
        public CustomerForm()
        {
            InitializeComponent();
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            //requiredDateDateTimePicker.Enabled = false;
            //orderDateDateTimePicker.Enabled = false;
                       
            LoadComboBox();

        }

        private void LoadComboBox()
        {
           orderIDs = OrderDB.GetOrderIDs();
            if (orderIDs.Count > 0) // triggers select...
            {
                orderIDComboBox.DataSource = orderIDs;
                orderIDComboBox.SelectedIndex = 0;
                
            }
            else // if no customer orders
            {
                MessageBox.Show("Nothing to display, Please Restart Application", "Empty Load");
                Application.Exit();
            }
            
        }

        private void orderIDComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            int selectedID = (int)orderIDComboBox.SelectedValue;

            try
            {
                current = OrderDB.GetOrderByID(selectedID);
                DisplayCurrentCustomerData();
                ordDetails = OrderDetailsDB.GetOrderDetailIDs(selectedID);
                orderDetailDataGridView.DataSource = ordDetails; //ordDetails

                //Calculating order total
                float total = 0;
                foreach (OrderDetail order in ordDetails)
                {
                    total += order.OrderTotal;
                }
                string Total = total.ToString();
                textOrderTotal.Text = total.ToString("c2");

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error while retrieving customer with selected ID: " + ex.Message,
                    ex.GetType().ToString());
            }
        }

        private void DisplayCurrentCustomerData()
        {
            if (current != null)
            {
                textCustomerID.Text = current.CustomerID;
                requiredDateDateTimePicker.Value = current.RequiredDate;
                orderDateDateTimePicker.Value = current.OrderDate;
                                

                if (current.ShippedDate == null)
                {
                    shippedDateDateTimePicker.ShowCheckBox = true;
                    shippedDateDateTimePicker.Checked = false;
                }
                else
                {
                    shippedDateDateTimePicker.ShowCheckBox = true;

                    DateTime shippingDate = (DateTime)current.ShippedDate;                    
                    shippedDateDateTimePicker.Value = shippingDate;

                    //Required Date
                    DateTime requireDate = (DateTime)current.RequiredDate;
                    requiredDateDateTimePicker.Value = requireDate;
                    //requiredDateDateTimePicker.Enabled = false;

                    //order Date
                    DateTime orderingDate = (DateTime)current.OrderDate;
                    orderDateDateTimePicker.Value = orderingDate;
                    //orderDateDateTimePicker.Enabled = false;
                }

            }
            else
                LoadComboBox();
        }// end of display

        
       

        private void shippedDateDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            

            if (((DateTimePicker)sender).ShowCheckBox == true)
            {
                if (((DateTimePicker)sender).Checked == false) //if checked, it's null'
                {
                    ((DateTimePicker)sender).CustomFormat = " "; // " " if the date has no value
                    ((DateTimePicker)sender).Format = DateTimePickerFormat.Custom;
                }
                else
                {
                    ((DateTimePicker)sender).Format = DateTimePickerFormat.Short;
                }
            }
            else
            {
                ((DateTimePicker)sender).Format = DateTimePickerFormat.Short;
            }

            //
            //shippedDateDateTimePicker.ValueChanged += new System.EventHandler(this.shippedDateDateTimePicker_ValueChanged);
            //
            //shippedDateDateTimePicker.Checked = false;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (IsShippedDateValid())
            {
                Order newOrder = new Order();

                newOrder.OrderID = current.OrderID;
                newOrder.CustomerID = current.CustomerID;
                newOrder.OrderDate = current.OrderDate;
                newOrder.RequiredDate = current.RequiredDate;

                if (shippedDateDateTimePicker == null)
                {
                    newOrder.ShippedDate = null;
                    shippedDateDateTimePicker.ShowCheckBox = true;
                    shippedDateDateTimePicker.Checked = true;
                }
                else
                    newOrder.ShippedDate = shippedDateDateTimePicker.Value;
                                

                try
                {
                    if (!OrderDB.UpdateShippingDate(current, newOrder))
                    {
                        MessageBox.Show("Another user has updated this or Shipping update is nullified due to ", "Concurrency Error");
                        this.DialogResult = DialogResult.Retry;
                    }
                    else
                    {
                        current = newOrder;
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error Occured while updating: " + ex.Message, ex.GetType().ToString());
                }
                finally
                {
                    MessageBox.Show("Your Request is Noted");
                }
            }
        }

        private bool IsShippedDateValid()
        {
            bool validDate = true; // empty is valid
            DateTime endDate; 
            if (shippedDateDateTimePicker.Value != null)// if not empty
            {
                string shipping = Convert.ToString(shippedDateDateTimePicker.Value);
                if (DateTime.TryParse(shipping, out endDate))//valid date
                {
                    DateTime startDate = orderDateDateTimePicker.Value.Date;
                    DateTime reqDate = requiredDateDateTimePicker.Value.Date;
                    if ((startDate >= endDate) || (reqDate < endDate))
                    {
                        validDate = false;
                        MessageBox.Show("SHIPPING DATE must be later than ORDER DATE but less than REQUIRED DATE", "Data Error");
                       
                    }
                }
                else
                {
                    validDate = false;
                    MessageBox.Show("Please Show Future Date for Shipping Date", "Data Error");
                  
                }
            }
            return validDate;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Sure! And you want to Close this APP?", "Form Closing", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
