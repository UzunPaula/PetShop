using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class UserForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        public string destinationFolder = $@"D:\COLEGIU 2022-2023\PRACTICA\PetShop Management System\PetShop\MainImagesUser";
        //public string destinationFolder = $@"{Application.StartupPath}\MainImagesUser";
        public UserForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            LoadUser();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUser();
        }

        private void pictureBox_add_Click(object sender, EventArgs e)
        {
            UserModule module = new UserModule(this);
            module.ShowDialog();
        }

        private void dataGridView_User_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView_User.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                UserModule module = new UserModule(this);
                module.lbluid.Text = dataGridView_User.Rows[e.RowIndex].Cells[1].Value.ToString();
                module.txtName.Text = dataGridView_User.Rows[e.RowIndex].Cells[2].Value.ToString();
                module.txtEmail.Text = dataGridView_User.Rows[e.RowIndex].Cells[3].Value.ToString();
                module.txtPhone.Text = dataGridView_User.Rows[e.RowIndex].Cells[4].Value.ToString();
                module.cbxRole.Text = dataGridView_User.Rows[e.RowIndex].Cells[5].Value.ToString();
                module.dtpBirth.Text = dataGridView_User.Rows[e.RowIndex].Cells[6].Value.ToString();
                module.txtPass.Text = dataGridView_User.Rows[e.RowIndex].Cells[7].Value.ToString();

                module.btnSave.Enabled = false;
                module.btnUpdate.Enabled = true;
                module.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this record ?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string image = "";
                    cn.Open();
                    cm = new SqlCommand("SELECT * FROM tblUser WHERE id=@id", cn);
                    cm.Parameters.AddWithValue("@id", dataGridView_User.Rows[e.RowIndex].Cells[1].Value.ToString());
                    dataReader = cm.ExecuteReader();
                    dataReader.Read();
                    if (dataReader.HasRows)
                    {
                        image = dataReader["photo"].ToString();
                        //MessageBox.Show(image);
                        File.Delete($@"{destinationFolder}\{image}");
                    }
                    cn.Close();

                    dbconn.executeQuery("DELETE FROM tblUser WHERE id LIKE '" + dataGridView_User.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    
                    MessageBox.Show("User data has been successfully removed !", title, MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            LoadUser();
        }

        #region Method
        public void LoadUser()
        {
            int i = 0;
            dataGridView_User.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tblUser WHERE CONCAT(name,email,phone,dateBirth,role) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dataReader = cm.ExecuteReader();
            while (dataReader.Read())
            {
                i++;
                dataGridView_User.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString(), DateTime.Parse(dataReader[5].ToString()).ToShortDateString(), dataReader[6].ToString());
            }
            dataReader.Close();
            cn.Close();
        }
        #endregion Method

        private void picBoxExport_Click(object sender, EventArgs e)
        {
            // Se copiază datele și denumirile câmpurilor în clipboard
            dataGridView_User.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Se selectează toate rândurile din dataGridView_User
            dataGridView_User.SelectAll();
            DataObject copyData = dataGridView_User.GetClipboardContent();
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
