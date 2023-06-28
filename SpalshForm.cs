﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetShop
{
    public partial class SpalshForm : Form
    {
        public SpalshForm()
        {
            InitializeComponent();
            this.Hide();
        }

        int startPoint = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            startPoint += 2;
            progressBar1.Value = startPoint;
            if (progressBar1.Value==100) 
            {
                progressBar1.Value = 0;
                timer1.Stop();
                LoginForm login = new LoginForm();
                login.Show();
                this.Hide();
            }
        }

        private void SpalshForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
