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
    public partial class Home : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        
        string title = "Pet Shop Management System";
        public Home()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
        }

        public int extractData(string str)//returneaza cantitatea totală a produselor dintr-o anumită categorie.
        {
            int data = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT ISNULL(SUM(pquantity),0) AS quantity FROM tblProduct WHERE pcategory='"+str+"'", cn);//extrage cantitatea totală a produselor
                data = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
            return data;
        }

        private void Home_Load(object sender, EventArgs e)
        { 
            lblDog.Text = extractData("Dog").ToString();
            lblCat.Text = extractData("Cat").ToString();
            lblBird.Text = extractData("Bird").ToString();

            // Setarea colțurilor rotunjite pentru butonul "btnLogin"
            int cornerRadius = 25; // Ajustați valoarea pentru a obține colțurile dorite
            ApplyRoundedCorners(panel1, cornerRadius);
            ApplyRoundedCorners(panel2, cornerRadius);
            ApplyRoundedCorners(panel3, cornerRadius);
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
