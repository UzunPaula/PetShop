using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class CashProduct : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        public string uname;
        CashForm cash;
        public CashProduct(CashForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            cash = form;
            LoadProduct();
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridView_Product.Rows)
            {
                bool checkbox = Convert.ToBoolean(dr.Cells["Select"].Value);
                if (checkbox)
                {
                    try
                    {
                        cm = new SqlCommand("INSERT INTO tblCash (transno, pcode, pname, quantity, price, cashier) VALUES (@transno, @pcode, @pname, @quantity, @price, @cashier)", cn);
                        cm.Parameters.AddWithValue("@transno", cash.lblTransaction.Text);
                        cm.Parameters.AddWithValue("@pcode", dr.Cells[1].Value.ToString());
                        cm.Parameters.AddWithValue("@pname", dr.Cells[2].Value.ToString());
                        cm.Parameters.AddWithValue("@quantity", 1);
                        cm.Parameters.AddWithValue("@price", Convert.ToDouble(dr.Cells[5].Value.ToString()));
                        cm.Parameters.AddWithValue("@cashier", uname);

                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                    }
                    catch (Exception ex)
                    {
                        cn.Close();
                        MessageBox.Show(ex.Message, title);
                    }
                }
            }
            cash.loadCash(); 
            this.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        #region Method
        public void LoadProduct()
        {
            int i = 0;
            dataGridView_Product.Rows.Clear();
            cm = new SqlCommand("SELECT pcode, pname, ptype, pcategory, pprice FROM tblProduct WHERE CONCAT(pname,ptype,pcategory) LIKE '%" + txtSearch.Text + "%' AND pquantity > "+ 0 +"", cn);
            cn.Open();
            dataReader = cm.ExecuteReader();
            while (dataReader.Read())
            {
                i++;
                dataGridView_Product.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString());
            }
            dataReader.Close();
            cn.Close();
        }
        #endregion Method 

        private void CashProduct_Load(object sender, EventArgs e)
        {
            // Setarea colțurilor rotunjite pentru butonul "btnLogin"
            int cornerRadius = 15; // Ajustați valoarea pentru a obține colțurile dorite
            ApplyRoundedCorners(btnSubmit, cornerRadius);
        }
        public void ApplyRoundedCorners(Control control, int cornerRadius)
        {
            Rectangle bounds = new Rectangle(Point.Empty, control.Size);
            using (GraphicsPath roundedPath = CreateRoundedPath(bounds, cornerRadius))
            {
                control.Region = new Region(roundedPath);
            }
        }

        public GraphicsPath CreateRoundedPath(Rectangle bounds, int cornerRadius)
        {
            GraphicsPath roundedPath = new GraphicsPath();
            roundedPath.AddArc(bounds.X, bounds.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedPath.AddArc(bounds.Right - cornerRadius * 2, bounds.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedPath.AddArc(bounds.Right - cornerRadius * 2, bounds.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedPath.AddArc(bounds.X, bounds.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedPath.CloseFigure();

            return roundedPath;
        }
    }
}
