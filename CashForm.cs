using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class CashForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        MainForm main;

        public CashForm(MainForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            main = form;
            getTransno();
            //loadCash();
        }

        private void pictureBox_add_Click(object sender, EventArgs e)
        {
            CashProduct product = new CashProduct(this);
            product.uname = main.lblUsername.Text;
            product.ShowDialog();
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            CashCustomer customer = new CashCustomer(this);
            customer.ShowDialog();

            if (MessageBox.Show("Are you sure you want to cash this product?", "Cashing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (MessageBox.Show("Do you want to export this data to excel?", "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    exportExcel();
                }

                getTransno();
                main.loadDailySale();
                for (int i = 0; i < dgvCash.Rows.Count; i++)
                {
                    dbconn.executeQuery("UPDATE tblProduct SET pquantity = pquantity - " + int.Parse(dgvCash.Rows[i].Cells[4].Value.ToString()) + "WHERE pcode LIKE " + dgvCash.Rows[i].Cells[2].Value.ToString() + "");
                }
                dgvCash.Rows.Clear();
                main.RefreshDiagram();
            }
        }

        private void dgvCash_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCash.Columns[e.ColumnIndex].Name;
            if (colName == "Sterge")
            {
                if (MessageBox.Show("Are you sure you want to delete this cash?", "Delete Cash", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbconn.executeQuery("DELETE FROM tblCash WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    MessageBox.Show("Cash record has been successfully removed !", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            else if (colName == "Increase")
            {
                int i = checkPqty(dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString());
                if (int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString()) < i)
                {
                    dbconn.executeQuery("UPDATE tblCash SET quantity = quantity + " + 1 + " WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                }
                else
                {
                    MessageBox.Show($"Remaning quantity on hand is {i} !", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (colName == "Decrease")
            {
                int quantity = int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString());
                if (quantity == 1)
                {
                    colName = "Sterge";
                    // Execută acțiunile aferente coloanei "Delete" 
                    if (MessageBox.Show("Are you sure you want to delete this cash?", "Delete Cash", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dbconn.executeQuery("DELETE FROM tblCash WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                        MessageBox.Show("Cash record has been successfully removed !", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    dbconn.executeQuery("UPDATE tblCash SET quantity = quantity - " + 1 + " WHERE cashid LIKE '" + dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                }
            }
            loadCash();
        }

        public void exportExcel()
        {
            // Se copiază datele și denumirile câmpurilor în clipboard
            dgvCash.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Se selectează toate rândurile din dataGridView_User
            dgvCash.SelectAll();
            DataObject copyData = dgvCash.GetClipboardContent();
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

        #region Method

        public void getTransno()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                int count;
                string transno;

                cn.Open();
                cm = new SqlCommand("SELECT TOP 1 transno FROM tblCash WHERE transno LIKE '" + sdate + "%' ORDER BY cashid DESC", cn);
                dataReader = cm.ExecuteReader();
                dataReader.Read();

                if (dataReader.HasRows)
                {
                    transno = dataReader[0].ToString();
                    if (transno.Length >= sdate.Length + 4) //verifica dacă lungimea "transno" este mai mare sau egală cu lungimea sdate (lungimea datei) plus 4 caractere.
                    {
                        count = int.Parse(transno.Substring(8, 4)); //Se extrage un substring din transno, de la poziția 8 și cu o lungime de 4 caractere. Acest substring reprezintă un număr.
                        lblTransaction.Text = sdate + (count + 1).ToString();
                    }
                    else
                    {
                        lblTransaction.Text = transno;
                    }
                }
                else
                {
                    transno = sdate + "1001";
                    lblTransaction.Text = transno;
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

        public void loadCash()
        {

            try
            {
                int i = 0;
                double total = 0;
                dgvCash.Rows.Clear();
                cm = new SqlCommand("SELECT cashid, pcode, pname, quantity, price, total, c.name, cashier FROM tblCash as cash LEFT JOIN tblCustomer c ON cash.cid = c.id WHERE transno LIKE '" + lblTransaction.Text + "'", cn);
                cn.Open();
                dataReader = cm.ExecuteReader();
                while (dataReader.Read())
                {
                    i++;
                    dgvCash.Rows.Add(i, dataReader[0].ToString(), dataReader[1].ToString(), dataReader[2].ToString(), dataReader[3].ToString(), dataReader[4].ToString(), dataReader[5].ToString(), dataReader[6].ToString(), dataReader[7].ToString());
                    total += double.Parse(dataReader[5].ToString());
                }
                dataReader.Close();
                cn.Close();
                lblTotal.Text = total.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        public int checkPqty(string pcode)
        {
            int i = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT pquantity FROM tblProduct WHERE pcode LIKE '" + pcode + "'", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
            return i;
        }
        #endregion Method

        private void CashForm_Load(object sender, EventArgs e)
        {
            // Setarea colțurilor rotunjite pentru butonul "btnLogin"
            int cornerRadius = 20; // Ajustați valoarea pentru a obține colțurile dorite
            ApplyRoundedCorners(btnCash, cornerRadius);
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
