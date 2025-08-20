using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Midterm
{
    public partial class Form1 : Form
    {
        string connectionString = "server=localhost;database=midterm;uid=root;pwd=;";


        public Form1()
        {
            InitializeComponent();

            cbGender.StartIndex = -1;
            cbDept.StartIndex = -1;
            cbZodiac.StartIndex = -1;
        }

        private void TextBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 

                Control me = sender as Control; 

                if (me == txtLastName) txtFirstName.Focus();
                else if (me == txtFirstName) txtMiddleName.Focus();
                else if (me == txtMiddleName) cbGender.Focus();
                else if (me == cbGender) txtAge.Focus();
                else if (me == txtAge) cbDept.Focus();
                else if (me == cbDept) txtUsername.Focus();
                else if (me == txtUsername) txtPassword.Focus();
                else if (me == txtPassword) cbZodiac.Focus();
                else if (me == cbZodiac)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = @"INSERT INTO users
                                    (lastname, firstname, middlename, gender, age, dept, username, password, zodiac)
                                    VALUES
                                    (@lastname, @firstname, @middlename, @gender, @age, @dept, @username, @password, @zodiac)";

                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@lastname", txtLastName.Text);
                                cmd.Parameters.AddWithValue("@firstname", txtFirstName.Text);
                                cmd.Parameters.AddWithValue("@middlename", txtMiddleName.Text);
                                cmd.Parameters.AddWithValue("@gender", cbGender.Text);
                                cmd.Parameters.AddWithValue("@age", txtAge.Text);
                                cmd.Parameters.AddWithValue("@dept", cbDept.Text);
                                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                                cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                                cmd.Parameters.AddWithValue("@zodiac", cbZodiac.Text);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Data saved successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }

                    txtLastName.Clear();
                    txtFirstName.Clear();
                    txtMiddleName.Clear();
                    cbGender.SelectedIndex = -1;
                    txtAge.Clear();
                    cbDept.SelectedIndex = -1;
                    txtUsername.Clear();
                    txtPassword.Clear();
                    cbZodiac.SelectedIndex = -1;
                    txtLastName.Focus();
                }
            }
        }
    }
}
