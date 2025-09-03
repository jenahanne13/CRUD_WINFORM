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
        private string connectionString = "server=localhost;database=jiro;uid=root;pwd=;";
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
                else if (me == txtPassword) cbPosition.Focus();
                else if (me == cbPosition)
                {
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                        string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                        string.IsNullOrWhiteSpace(txtMiddleName.Text) ||
                        string.IsNullOrWhiteSpace(txtAge.Text) ||
                        string.IsNullOrWhiteSpace(txtUsername.Text) ||
                        string.IsNullOrWhiteSpace(txtPassword.Text) ||
                        cbGender.SelectedIndex == -1 ||
                        cbDept.SelectedIndex == -1 ||
                        cbPosition.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please fill in all textboxes and select values in all combo boxes.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (selectedId == -1)
                    {
                        InsertRecord();
                    }
                    else
                    {
                        UpdateRecord();
                    }
                }
            }
        }

        private void InsertRecord()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO users
                (lastname, firstname, middlename, gender, age, dept, username, password, position)
                VALUES
                (@lastname, @firstname, @middlename, @gender, @age, @dept, @username, @password, @position)";

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
                        cmd.Parameters.AddWithValue("@position", cbPosition.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data saved successfully!");
                LoadData();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UpdateRecord()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string selectQuery = "SELECT lastname, firstname, middlename, gender, age, dept, username, password, position FROM users WHERE id=@id";
                    using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@id", selectedId);
                        using (MySqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool hasChanges =
                                    reader["lastname"].ToString() != txtLastName.Text ||
                                    reader["firstname"].ToString() != txtFirstName.Text ||
                                    reader["middlename"].ToString() != txtMiddleName.Text ||
                                    reader["gender"].ToString() != cbGender.Text ||
                                    reader["age"].ToString() != txtAge.Text ||
                                    reader["dept"].ToString() != cbDept.Text ||
                                    reader["username"].ToString() != txtUsername.Text ||
                                    reader["password"].ToString() != txtPassword.Text ||
                                    reader["position"].ToString() != cbPosition.Text;

                                if (!hasChanges)
                                {
                                    MessageBox.Show("No changes detected. Record not updated.");
                                    return;
                                }
                            }
                        }
                    }

                    string updateQuery = @"UPDATE users SET
                        lastname=@lastname, firstname=@firstname, middlename=@middlename,
                        gender=@gender, age=@age, dept=@dept,
                        username=@username, password=@password, position=@position
                        WHERE id=@id";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@firstname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@middlename", txtMiddleName.Text);
                        cmd.Parameters.AddWithValue("@gender", cbGender.Text);
                        cmd.Parameters.AddWithValue("@age", txtAge.Text);
                        cmd.Parameters.AddWithValue("@dept", cbDept.Text);
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                        cmd.Parameters.AddWithValue("@position", cbPosition.Text);
                        cmd.Parameters.AddWithValue("@id", selectedId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record updated successfully!");
                LoadData();
                ClearInputs();
                selectedId = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearInputs()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtAge.Clear();
            txtUsername.Clear();
            txtPassword.Clear();

            if (cbGender.Items.Count > 0) cbGender.SelectedIndex = 0;
            if (cbDept.Items.Count > 0) cbDept.SelectedIndex = 0;
            if (cbPosition.Items.Count > 0) cbPosition.SelectedIndex = 0;

            txtLastName.Focus();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, lastname, firstname, middlename, gender, age, dept, username, password, position FROM users";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns["id"] != null)
                        dataGridView1.Columns["id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            LoadData();

            cbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPosition.DropDownStyle = ComboBoxStyle.DropDownList;

            cbSearchBy.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSearchBy.Items.AddRange(new string[] { "Lastname", "Firstname", "Middlename", "Username" });
            cbSearchBy.SelectedIndex = 0;
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

                SetComboBoxValue(cbGender, row.Cells["gender"].Value.ToString().Trim());
                SetComboBoxValue(cbDept, row.Cells["dept"].Value.ToString().Trim());
                SetComboBoxValue(cbPosition, row.Cells["position"].Value.ToString().Trim());
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

        private void SearchData(string searchTerm)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string column = "lastname";
                    if (cbSearchBy.SelectedItem != null)
                    {
                        switch (cbSearchBy.SelectedItem.ToString())
                        {
                            case "Firstname": column = "firstname"; break;
                            case "Middlename": column = "middlename"; break;
                            case "Username": column = "username"; break;
                            default: column = "lastname"; break;
                        }
                    }

                    string query = $@"SELECT id, lastname, firstname, middlename, gender, age, dept, username, password, position
                             FROM users
                             WHERE {column} LIKE @search";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@search", MySqlDbType.VarChar).Value = "%" + searchTerm + "%";

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching data: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchData(txtSearch.Text.Trim());
        }

        private void CancelEdit()
        {
            selectedId = -1;
            ClearInputs();
            MessageBox.Show("Edit cancelled. You are now back in insert mode.");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (selectedId != -1)
                {
                    CancelEdit();
                }
            }
        }

        private void DeleteRecord()
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this record?",
                                          "Confirm Delete",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
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
                    ClearInputs();
                    selectedId = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting record: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }
    }
}