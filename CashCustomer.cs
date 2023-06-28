using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class CashCustomer : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        CashForm cash;
        public CashCustomer(CashForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            cash = form;
            LoadCustomer();
        }

        private void dataGridView_Customer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView_Customer.Columns[e.ColumnIndex].Name;
            if (colName == "Choice")
            {
                dbconn.executeQuery("UPDATE tblCash SET cid="+int.Parse(dataGridView_Customer.Rows[e.RowIndex].Cells[1].Value.ToString())+ "WHERE transno='" + cash.lblTransaction.Text + "'");

                cash.loadCash();
                this.Dispose();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        { 
            LoadCustomer();
        } 

        #region Method
        public void LoadCustomer()
        {  
            try
            {
                int i = 0;
                dataGridView_Customer.Rows.Clear();
                cm = new SqlCommand("SELECT id, name, phone FROM tblCustomer WHERE name LIKE '%" + txtSearch.Text + "%'", cn);
                cn.Open();
                dataReader = cm.ExecuteReader();
                while (dataReader.Read()) 
                {
                    i++;
                    dataGridView_Customer.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString());
                }
                dataReader.Close();
                cn.Close();
            }   
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }
        #endregion Method
    }
}
