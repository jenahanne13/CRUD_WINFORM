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
        private string connectionString = "server=localhost;database=midterm;uid=root;pwd=;";
        private int selectedId = -1;

        public Form1()
        {
            InitializeComponent();
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
                                LoadData();
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
                    txtAge.Clear();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtLastName.Focus();
                }
            }
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, lastname, firstname, middlename, gender, age, dept, username, password, zodiac FROM users";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();

            cbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cbZodiac.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["id"].Value);

                txtLastName.Text = row.Cells["lastname"].Value.ToString();
                txtFirstName.Text = row.Cells["firstname"].Value.ToString();
                txtMiddleName.Text = row.Cells["middlename"].Value.ToString();
                txtAge.Text = row.Cells["age"].Value.ToString();
                txtUsername.Text = row.Cells["username"].Value.ToString();
                txtPassword.Text = row.Cells["password"].Value.ToString();

                // Safe ComboBox update with repaint
                SetComboBoxValue(cbGender, row.Cells["gender"].Value.ToString().Trim());
                SetComboBoxValue(cbDept, row.Cells["dept"].Value.ToString().Trim());
                SetComboBoxValue(cbZodiac, row.Cells["zodiac"].Value.ToString().Trim());
            }
        }

        private void SetComboBoxValue(ComboBox cb, string value)
        {
            int index = cb.FindStringExact(value);
            if (index >= 0)
                cb.SelectedIndex = index;
            else
                cb.SelectedIndex = -1;
            cb.Refresh();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a record first!");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE users SET
                            lastname=@lastname, firstname=@firstname, middlename=@middlename,
                            gender=@gender, age=@age, dept=@dept,
                            username=@username, password=@password, zodiac=@zodiac
                            WHERE id=@id";

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
                        cmd.Parameters.AddWithValue("@id", selectedId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record updated successfully!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a record first!");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?",
                                                  "Confirm Delete",
                                                  MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM users WHERE id=@id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selectedId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Record deleted successfully!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtAge.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtLastName.Focus();
        }
    }
}