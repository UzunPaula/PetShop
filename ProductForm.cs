using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class ProductForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        public ProductForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            LoadProduct();
        }

        private void pictureBox_add_Click(object sender, EventArgs e)
        {
            ProductModule module = new ProductModule(this);
            module.ShowDialog();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void dataGridView_Product_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView_Product.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                ProductModule module = new ProductModule(this);
                module.lblPcode.Text = dataGridView_Product.Rows[e.RowIndex].Cells[1].Value.ToString();
                module.txtName.Text = dataGridView_Product.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.txtType.Text = dataGridView_Product.Rows[e.RowIndex].Cells[3].Value.ToString();
                module.cbxCategory.Text = dataGridView_Product.Rows[e.RowIndex].Cells[4].Value.ToString();
                module.txtOty.Text = dataGridView_Product.Rows[e.RowIndex].Cells[5].Value.ToString();
                module.txtPrice.Text = dataGridView_Product.Rows[e.RowIndex].Cells[6].Value.ToString();

                module.btnSave.Enabled = false;
                module.btnUpdate.Enabled = true;
                module.ShowDialog();
            }
            else if(colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this items?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
                {
                    dbconn.executeQuery("DELETE FROM tblProduct WHERE pcode LIKE '" + dataGridView_Product.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    //cn.Open();
                    //cm = new SqlCommand("DELETE FROM tblProduct WHERE pcode LIKE '" + dataGridView_Product.Rows[e.RowIndex].Cells[1].Value.ToString() + "'", cn);
                    //cm.ExecuteNonQuery();
                    //cn.Close();
                    MessageBox.Show("Item record has been successfully removed !", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            LoadProduct();
        }
        #region Method
        public void LoadProduct()
        {
            int i = 0;
            dataGridView_Product.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tblProduct WHERE CONCAT(pname,ptype,pcategory) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dataReader = cm.ExecuteReader();
            while (dataReader.Read())
            {
                i++;
                dataGridView_Product.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString());
            }
            dataReader.Close();
            cn.Close();
        }
        #endregion Method

        private void picBoxExport_Click(object sender, EventArgs e)
        {
            // Se copiază datele și denumirile câmpurilor în clipboard
            dataGridView_Product.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Se selectează toate rândurile din dataGridView_User
            dataGridView_Product.SelectAll();
            DataObject copyData = dataGridView_Product.GetClipboardContent();
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
