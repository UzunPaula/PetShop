using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Drawing.Drawing2D;

namespace PetShop
{
    public partial class UserModule : Form
    {
        SqlConnection cn=new SqlConnection();
        SqlCommand cm= new SqlCommand();
        DbConnect dbconn = new DbConnect();
        string title = "Pet Shop Management System";

        bool check = false;
        UserForm userForm;
        public UserModule(UserForm user)
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
            userForm = user;
            cbxRole.SelectedIndex = 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try 
            {
                CheckField();
                if (check)
                {
                    if (MessageBox.Show("Are you sure you want to register this user ?", "User Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("INSERT INTO tblUser(name, email, phone, role, dateBirth, password)VALUES(@name, @email, @phone, @role, @dateBirth, @password)", cn);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@email", txtEmail.Text);
                        cm.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cm.Parameters.AddWithValue("@role", cbxRole.Text);
                        cm.Parameters.AddWithValue("@dateBirth", dtpBirth.Value);
                        cm.Parameters.AddWithValue("@password", txtPass.Text);

                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("User has been successfully registered !", title);
                        Clear();
                        userForm.LoadUser();
                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                CheckField();
                if (check)
                {
                    if (MessageBox.Show("Are you sure you want to update this user ?", "Edit Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        cm = new SqlCommand("UPDATE tblUser SET name=@name, email=@email, phone=@phone, role=@role, dateBirth=@dateBirth, password=@password WHERE id=@id", cn);
                        cm.Parameters.AddWithValue("@id", lbluid.Text);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@email", txtEmail.Text);
                        cm.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cm.Parameters.AddWithValue("@role", cbxRole.Text);
                        cm.Parameters.AddWithValue("@dateBirth", dtpBirth.Value);
                        cm.Parameters.AddWithValue("@password", txtPass.Text);

                        cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("User's data has been successfully updated !", title);
                        Clear();
                        userForm.LoadUser();
                        this.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void cbxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRole.Text == "Employee")
            {
                this.Height = 450 - 29;
                lblPass.Visible = false;
                txtPass.Visible = false;                                                          
            }
            else
            {
                lblPass.Visible = true;
                txtPass.Visible = true;
                this.Height = 450;
            }
        }

        #region Method

        public void Clear()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtPass.Clear();  
            cbxRole.SelectedIndex = 0;
            dtpBirth.Value = DateTime.Now;

            btnUpdate.Enabled = false;
        }

        //check field and date of birth
        public void CheckField()
        {
            if (txtName.Text == "" | txtEmail.Text=="")
            {
                MessageBox.Show("Required data field !", "Warning !");
                return;
            }
            if (CheckAge(dtpBirth.Value) < 18) 
            {
                MessageBox.Show("User is child worker ! Under 18 year", "Warning !");
                return;
            }
            check = true;
        }

        //Calculate Age for under 18
        private static int CheckAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;
            return age;
        }

        #endregion Method

        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void UserModule_Load(object sender, EventArgs e)
        {
            // Setarea colțurilor rotunjite pentru butonul "btnLogin"
            int cornerRadius = 15; // Ajustați valoarea pentru a obține colțurile dorite
            ApplyRoundedCorners(btnSave, cornerRadius);
            ApplyRoundedCorners(btnUpdate, cornerRadius);
            ApplyRoundedCorners(btnCancel, cornerRadius);
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
