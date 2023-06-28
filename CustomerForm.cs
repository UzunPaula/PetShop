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
    public partial class CustomerForm : Form
    {
        
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlConnection cn = new SqlConnection();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        public CustomerForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            LoadCustomer();
        }

        private void pictureBox_add_Click(object sender, EventArgs e)
        {
            CustomerModule customerModule = new CustomerModule(this);
            customerModule.ShowDialog();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void dataGridView_Customer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView_Customer.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                CustomerModule module = new CustomerModule(this);
                module.lblcid.Text = dataGridView_Customer.Rows[e.RowIndex].Cells[1].Value.ToString();
                module.txtName.Text = dataGridView_Customer.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.txtAddress.Text = dataGridView_Customer.Rows[e.RowIndex].Cells[3].Value.ToString();
                module.txtPhone.Text = dataGridView_Customer.Rows[e.RowIndex].Cells[4].Value.ToString();

                module.btnSave.Enabled = false;
                module.btnUpdate.Enabled = true;
                module.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this customer record ?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbconn.executeQuery("DELETE FROM tblCustomer WHERE id LIKE '" + dataGridView_Customer.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    MessageBox.Show("Customer data has been successfully removed !", title, MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            LoadCustomer();
        }

        #region Method
        public void LoadCustomer()
        {
            int i = 0;
            dataGridView_Customer.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tblCustomer WHERE CONCAT(name,address,phone) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dataReader = cm.ExecuteReader();
            while (dataReader.Read())
            { 
                i++;
                dataGridView_Customer.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString());
            }
            dataReader.Close();
            cn.Close();
        }
        #endregion Method

        private void picBoxExport_Click(object sender, EventArgs e)
        {
            // Se copiază datele și denumirile câmpurilor în clipboard
            dataGridView_Customer.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Se selectează toate rândurile din dataGridView_User
            dataGridView_Customer.SelectAll();
            DataObject copyData = dataGridView_Customer.GetClipboardContent();
            if (copyData != null)
                Clipboard.SetDataObject(copyData);

            // Se creează o instanță a aplicației Excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;

            // Se creează un nou workbook
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);

            // Se selectează prima foaie de lucru
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;

            // Se obține gama de celule pentru a copia datele și denumirile
            Microsoft.Office.Interop.Excel.Range range = worksheet.Cells[1, 1];
            range.Select();

            // Se lipesc datele și denumirile în foaia de lucru
            worksheet.PasteSpecial(range, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

            // Se formatează rândurile și coloanele pentru a obține un aspect de tabel
            worksheet.ListObjects.AddEx(Microsoft.Office.Interop.Excel.XlListObjectSourceType.xlSrcRange, range.CurrentRegion, Type.Missing, Microsoft.Office.Interop.Excel.XlYesNoGuess.xlYes).Name = "Table1";
            worksheet.ListObjects["Table1"].TableStyle = "TableStyleMedium2";

            // Se ajustează lățimea coloanelor pentru a se potrivi conținutului
            range.CurrentRegion.Columns.AutoFit();
        }
    }
}
