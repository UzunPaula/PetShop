using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class LoginForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbconn = new DbConnect();
        SqlDataReader dataReader;
        string title = "Pet Shop Management System";
        public string filePath;
        public string _image = "";
        public string destinationFolder = $@"D:\COLEGIU 2022-2023\PRACTICA\PetShop Management System\PetShop\MainImagesUser";
        //public string destinationFolder = $@"{Application.StartupPath}\MainImagesUser";

        public LoginForm()
        {
            InitializeComponent();
            cn = new SqlConnection(dbconn.connection());
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {  
            try
            {
                string _name = "", _role = "";
                cn.Open();
                cm = new SqlCommand("SELECT * FROM tblUser WHERE name=@name and password=@password", cn);
                cm.Parameters.AddWithValue("@name", txtUserName.Text);
                cm.Parameters.AddWithValue("@password", txtPass.Text);
                dataReader = cm.ExecuteReader();
                dataReader.Read();
                if (dataReader.HasRows)
                {
                    _name = dataReader["name"].ToString();
                    _role = dataReader["role"].ToString();
                    _image = dataReader["photo"].ToString();
                    MessageBox.Show($"Welcome {_name} !", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainForm main = new MainForm(); //Se creează o nouă instanță a formularului
                    main.lblUsername.Text = _name; //se stabilesc valorile etichetelor
                    main.lblRole.Text = _role;
                    main.picBoxImgUser.ImageLocation = $@"{destinationFolder}\{_image}";

                    if (_role == "Administrator")
                        main.btnUser.Enabled = true;
                    this.Hide();
                    main.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Invalid username and password !", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }
        public void LoginForm_Load(object sender, EventArgs e)
        {
            // Setarea colțurilor rotunjite pentru butonul "btnLogin"
            int cornerRadius = 25; // Se ajusteaza valoarea pentru a obține colțurile dorite
            ApplyRoundedCorners(btnLogin, cornerRadius);
            ApplyRoundedCorners(btnRegister, cornerRadius);

            KeyPreview = true; // Permite formularului să primească evenimentul KeyDown înainte de a fi procesat de alte controale
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

        private bool isValidRegistration = false; // Variabilă pentru a verifica dacă înregistrarea este validă
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtRegUser.Text;
                string password = txtRegPass.Text;
                string confirmPassword = txtRegConfirmPass.Text;
                string email = txtRegEmail.Text;

                // Verificăm dacă câmpurile de înregistrare nu sunt goale
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Please fill in all the fields!", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    isValidRegistration = true;
                }

                // Verificăm dacă parola contine mai mult de 8 caractere: litere, cifre si simboluri
                if (!IsPasswordValid(password))
                {
                    MessageBox.Show("The password must contain 8+ characters: letters, numbers and symbols !", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    isValidRegistration = true;
                }

                // Verificăm dacă emailul este valid
                if (!IsEmailValid(email))
                {
                    MessageBox.Show("Email invalid. Please enter valid values!", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    isValidRegistration = true;
                }

                // Verificăm dacă parola și confirmarea parolei sunt diferite
                if (password != confirmPassword)
                {
                    MessageBox.Show("Password incorrect! Please make sure that passwords are the same.", "Sign Up Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    panelRegPass.BackColor = Color.Red;
                    panelRegConfPass.BackColor = Color.Red;
                    return;
                }
                else
                {
                    // Resetăm culoarea panoului de parolă
                    panelRegPass.BackColor = Color.Green;
                    panelRegConfPass.BackColor = Color.Green;
                    //isValidRegistration = true;
                }

                if (!isValidRegistration)
                {
                    MessageBox.Show("Please complete the registration form correctly.", "Registration Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Verificăm dacă utilizatorul cu același nume deja există în baza de date
                cn.Open();
                cm = new SqlCommand("SELECT COUNT(*) FROM tblUser WHERE name=@name", cn);
                cm.Parameters.AddWithValue("@name", name);
                int userCount = (int)cm.ExecuteScalar(); //returneaza prima coloană a primei înregistrări 
                cn.Close();

                if (userCount > 0)
                {
                    MessageBox.Show("Username already exists! Please choose a different username.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Salvăm utilizatorul nou în baza de date
                cn.Open();
                cm = new SqlCommand("INSERT INTO tblUser (name, password,email,role,dateBirth,photo) VALUES (@name, @password,@email,@role,@dateBirth,@photo)", cn);
                cm.Parameters.AddWithValue("@name", txtRegUser.Text);
                cm.Parameters.AddWithValue("@password", txtRegPass.Text);
                cm.Parameters.AddWithValue("@email", txtRegEmail.Text);
                cm.Parameters.AddWithValue("@role", "Employee");
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileExtension = Path.GetExtension(filePath);
                cm.Parameters.AddWithValue("@photo", $"{fileName}{fileExtension}");
                cm.Parameters.AddWithValue("@dateBirth", DateTime.Now.ToShortDateString());
                cm.ExecuteNonQuery();
                cn.Close();


                MessageBox.Show("Registration successful! You can now log in with your new account.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                panelRegister.Height = 0;
                panelLogin.Location = new Point(316, 0);
                panelL.Location = new Point(0, 0);

                // Resetăm câmpurile de înregistrare
                txtRegUser.Text = "";
                txtRegPass.Text = "";
                txtRegConfirmPass.Text = "";
                txtRegEmail.Text = "";
                panelRegPass.BackColor = Color.MediumTurquoise;
                panelRegConfPass.BackColor = Color.MediumTurquoise;
                linkLabelPhoto.BackColor = Color.Transparent;

                isValidRegistration = true; // Setăm validarea înregistrării ca fiind corectă
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        // Verifică dacă parola respectă cerințele
        private bool IsPasswordValid(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            bool hasLetter = false;
            bool hasDigit = false;
            bool hasSymbol = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (char.IsSymbol(c) || char.IsPunctuation(c))
                {
                    hasSymbol = true;
                }
            }

            return hasLetter && hasDigit && hasSymbol;
        }

        // Verifică dacă emailul respectă cerințele
        private bool IsEmailValid(string email)
        {
            // Definește o expresie regulată pentru a verifica formatul general al emailului
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Verifică dacă emailul se potrivește cu expresia regulată
            return Regex.IsMatch(email, emailPattern);
        }

        private void pictureBox_close_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit Application ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void pictureBox_backLogReg_Click(object sender, EventArgs e)
        {

            panelRegister.Height = panelLogin.Height;
            panelRegister.Location = new Point(0, 440);
            panelRegister.Visible = false;
            panelLogin.Location = new Point(312, 0);
            panelL.Location = new Point(0, 0);
        }
        private void pictureBoxTogglePass_Click(object sender, EventArgs e)
        {
            if (txtPass.UseSystemPasswordChar)
            {
                pictureBoxTogglePass.Image = Properties.Resources.eye_closed_icon;
                txtPass.UseSystemPasswordChar = false;
            }
            else
            {
                pictureBoxTogglePass.Image = Properties.Resources.eye_open_icon;
                txtPass.UseSystemPasswordChar = true;
            }
        }
        private void pBoxRegPass_Click(object sender, EventArgs e)
        {
            if (txtRegPass.UseSystemPasswordChar)
            {
                pBoxRegPass.Image = Properties.Resources.eye_closed_icon;
                txtRegPass.UseSystemPasswordChar = false;
            }
            else
            {
                pBoxRegPass.Image = Properties.Resources.eye_open_icon;
                txtRegPass.UseSystemPasswordChar = true;
            }
        }
        private void pBoxRegPassCon_Click(object sender, EventArgs e)
        {
            if (txtRegConfirmPass.UseSystemPasswordChar)
            {
                pBoxRegPassCon.Image = Properties.Resources.eye_closed_icon;
                txtRegConfirmPass.UseSystemPasswordChar = false;
            }
            else
            {
                pBoxRegPassCon.Image = Properties.Resources.eye_open_icon;
                txtRegConfirmPass.UseSystemPasswordChar = true;
            }
        }
        private void linkLabel_CreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelRegister.Height = panelLogin.Height;
            panelLogin.Location = new Point(0, 0);
            panelL.Location = new Point(495, 0);
            panelRegister.Visible = true;
        }
        private void linkLabelPhoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFile();
        }
        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fisiere imagine|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Title = "Select an image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                //MessageBox.Show("Ai selectat fisierul: " + filePath);
                CopyImageToDestination();
                linkLabelPhoto.BackColor = Color.LightGreen;
            }
        }
        public void CopyImageToDestination()
        {
            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(destinationFolder, fileName);

            try
            {
                File.Copy(filePath, destinationPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying image! " + ex.Message);
            }
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick(); // Simulează făcând clic pe butonul "Login"
                e.Handled = true; // Doar butonul "Login" va răspunde la apăsarea tastei Enter, fără efecte suplimentare a evenimentului în alte controale sau logici.
            }
        }
    }
}
