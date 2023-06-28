using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace PetShop
{
    class DbConnect
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        private string con;

        public string connection()
        {
            con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\COLEGIU 2022-2023\PRACTICA\PetShop Management System\PetShop\dbPetShop.mdf"";Integrated Security=True; Connect Timeout=30";
            //string dbFileName = "dbPetShop.mdf";
            //string dbFilePath = Path.Combine(Application.StartupPath, dbFileName);
            //con = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{dbFilePath}"";Integrated Security=True;Connect Timeout=30";
            return con;
        }

        public void executeQuery(string sql)
        {
            try
            {
                conn.ConnectionString = connection();//stabilirea conexiunii la baza de date
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
