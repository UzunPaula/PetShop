using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PetShop
{
    public partial class MainForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;//citeste și acceseaza datele returnate în urma unei interogări SQL.
        Home formHome = new Home();
        public string destinationFolder = $@"D:\COLEGIU 2022-2023\PRACTICA\PetShop Management System\PetShop\MainImagesUser";
        //public string destinationFolder = $@"{Application.StartupPath}\MainImagesUser";
        public MainForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            btnHome.PerformClick();// declanșa un eveniment ca și când utilizatorul ar fi făcut clic pe buton.
            loadDailySale();
            openChildForm(new Home());
        }

        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit Application ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void pictureBox_minimize_Click(object sender, EventArgs e)
        {
             this.WindowState = FormWindowState.Minimized;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            openChildForm(new Home());//deschide o fereastră secundara în cadrul ferestrei principale sau a unui container principal.
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            openChildForm(new CustomerForm());
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            openChildForm(new UserForm());
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            openChildForm(new ProductForm());
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            openChildForm(new CashForm(this));
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Logout Application ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LoginForm login = new LoginForm();
                this.Dispose();
                login.ShowDialog();
            }
        }

        public class RoundPictureBox : PictureBox
        {
            public int CornerRadius { get; set; } = 20;

            protected override void OnPaint(PaintEventArgs pe)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                    path.AddArc(Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                    path.AddArc(Width - CornerRadius, Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                    path.AddArc(0, Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);

                    Region = new Region(path);
                }

                base.OnPaint(pe);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshDiagram();
        }

        public void RoundPictureBoxToCircle(PictureBox picBoxImgUser, int diameter)
        {
            Bitmap originalImage = (Bitmap)picBoxImgUser.Image;
            Bitmap circularImage = new Bitmap(diameter, diameter);
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, diameter, diameter);

            using (Graphics graphics = Graphics.FromImage(circularImage))
            {
                graphics.Clear(Color.Transparent);
                graphics.SetClip(path);
                graphics.DrawImage(originalImage, 0, 0, diameter, diameter);
            }

            picBoxImgUser.Image = circularImage;
            picBoxImgUser.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void RefreshDiagram()
        {
            try
            {
                cn.Open();
                // Inițializarea variabilelor pentru vânzările animalelor
                int dogs = 0;
                int cats = 0;
                int birds = 0;

                SqlCommand cmd = new SqlCommand("SELECT tblProduct.pcategory, SUM(tblCash.quantity) AS total FROM tblProduct INNER JOIN tblCash ON tblProduct.pcode = tblCash.pcode GROUP BY tblProduct.pcategory", cn);

                // Executați interogarea și obțineți rezultatul
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    birds = Convert.ToInt32(reader["total"]);
                }

                if (reader.Read())
                {
                    cats = Convert.ToInt32(reader["total"]);
                }

                if (reader.Read())
                {
                    dogs = Convert.ToInt32(reader["total"]);
                }

                reader.Close();

                // Crearea seriei pentru diagramă
                Series series = new Series("Sales");
                series.ChartType = SeriesChartType.Pie;

                // Adăugarea datelor pentru fiecare criteriu
                series.Points.AddXY("Dogs", dogs);
                series.Points.AddXY("Cats", cats);
                series.Points.AddXY("Birds", birds);

                // Setarea valorilor de tip Tooltip pentru fiecare punct de date
                series.Points[0].ToolTip = dogs.ToString();
                series.Points[1].ToolTip = cats.ToString();
                series.Points[2].ToolTip = birds.ToString();

                // Adăugarea seriei la diagramă
                chart1.Series.Clear();
                chart1.Series.Add(series);

                // Setarea culorilor pentru fiecare secțiune a diagramă
                series.Points[0].Color = Color.RoyalBlue;    // Culoarea pentru Dogs
                series.Points[1].Color = Color.MediumVioletRed;   // Culoarea pentru Cats
                series.Points[2].Color = Color.PaleVioletRed;  // Culoarea pentru Birds

                // Setarea datelor în diagramă
                chart1.Series["Sales"].Points.Clear();
                chart1.Series["Sales"].Points.AddXY("Dogs", dogs);
                chart1.Series["Sales"].Points.AddXY("Cats", cats);
                chart1.Series["Sales"].Points.AddXY("Birds", birds);

                // Actualizarea afișării diagramă
                chart1.Update();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Method
        private Form activeForm = null;
        public void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            lblTitle.Text = childForm.Text;
            panelChild.Controls.Add(childForm);
            panelChild.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        public void loadDailySale()
        {
            string sdate = DateTime.Now.ToString("yyyyMMdd");
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT ISNULL(SUM(total),0) AS total FROM tblCash WHERE transno LIKE '" + sdate + "%'", cn);
                lblDailySale.Text = double.Parse(cm.ExecuteScalar().ToString()).ToString("#,##0.00");
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }
        #endregion Method

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var hitTestResult = chart1.HitTest(e.X, e.Y);

            if (hitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                var dataPoint = hitTestResult.Series.Points[hitTestResult.PointIndex];
                var value = dataPoint.YValues[0].ToString();

                tooltipLabel.Text = value; // tooltipLabel este un control Label în care se va afișa valoarea
            }
            else
            {
                tooltipLabel.Text = ""; // Dacă cursorul nu se află pe un punct de date, se șterge textul
            }
        }

        private void picBoxImgUser_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            picBoxImgUser.SizeMode = PictureBoxSizeMode.Zoom;
            RoundPictureBoxToCircle(picBoxImgUser, 88);
            picBoxImgUser.LoadCompleted -= picBoxImgUser_LoadCompleted;
        }

        private void panelChild_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
