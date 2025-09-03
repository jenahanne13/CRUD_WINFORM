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
        private string connectionString = "server=localhost;database=jenah_db;uid=root;pwd=;";
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
                else if (me == txtFirstName) txtAddress.Focus();
                else if (me == txtAddress) cbGender.Focus();
                else if (me == cbGender) txtAge.Focus();
                else if (me == txtAge) cbSport.Focus();
                else if (me == cbSport) txtTeam.Focus();
                else if (me == txtTeam) txtNickname.Focus();
                else if (me == txtNickname) cbPosition.Focus();
                else if (me == cbPosition)
                {
                    if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                        string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                        string.IsNullOrWhiteSpace(txtAddress.Text) ||
                        string.IsNullOrWhiteSpace(txtAge.Text) ||
                        string.IsNullOrWhiteSpace(txtTeam.Text) ||
                        string.IsNullOrWhiteSpace(txtNickname.Text) ||
                        cbGender.SelectedIndex == -1 ||
                        cbSport.SelectedIndex == -1 ||
                        cbPosition.SelectedIndex == -1)
                    {
                        MessageBox.Show("Please fill in all textboxes and select values in all combo boxes.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (selectedId == -1)
                    {
                        InsertRecord();
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
                (lastname, firstname, address, gender, age, sport, team, nickname, position)
                VALUES
                (@lastname, @firstname, @address, @gender, @age, @sport, @team, @nickname, @position)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@firstname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@gender", cbGender.Text);
                        cmd.Parameters.AddWithValue("@age", txtAge.Text);
                        cmd.Parameters.AddWithValue("@sport", cbSport.Text);
                        cmd.Parameters.AddWithValue("@team", txtTeam.Text);
                        cmd.Parameters.AddWithValue("@nickname", txtNickname.Text);
                        cmd.Parameters.AddWithValue("@position", cbPosition.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Saved successfully!");
                LoadData();
                ClearInputs();
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
            txtAddress.Clear();
            txtAge.Clear();
            txtTeam.Clear();
            txtNickname.Clear();

            if (cbGender.Items.Count > 0) cbGender.SelectedIndex = 0;
            if (cbSport.Items.Count > 0) cbSport.SelectedIndex = 0;
            if (cbPosition.Items.Count > 0) cbPosition.SelectedIndex = 0;

            cbGender.Refresh();
            cbSport.Refresh();
            cbPosition.Refresh();

            txtLastName.Focus();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, lastname, firstname, address, gender, age, sport, team, nickname, position FROM users";
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
            cbSport.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPosition.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["id"].Value);

                txtLastName.Text = row.Cells["lastname"].Value.ToString();
                txtFirstName.Text = row.Cells["firstname"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtAge.Text = row.Cells["age"].Value.ToString();
                txtTeam.Text = row.Cells["team"].Value.ToString();
                txtNickname.Text = row.Cells["nickname"].Value.ToString();

                SetComboBoxValue(cbGender, row.Cells["gender"].Value.ToString().Trim());
                SetComboBoxValue(cbSport, row.Cells["sport"].Value.ToString().Trim());
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT id, lastname, firstname, address, gender, age, sport, team, nickname, position
                             FROM users
                                WHERE lastname LIKE @search
                                OR firstname LIKE @search
                                OR sport LIKE @search
                                OR position LIKE @search
                                OR nickname LIKE @search
                                OR team LIKE @search";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@search", MySqlDbType.VarChar).Value = "%" + txtSearch.Text + "%";

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

        private void btnClear_Click(object sender, EventArgs e)
        {
            selectedId = -1;
            ClearInputs();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId <= 0)
            {
                MessageBox.Show("Please select a record to update first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string selectQuery = "SELECT lastname, firstname, address, gender, age, sport, team, nickname, position FROM users WHERE id=@id";
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
                                    reader["address"].ToString() != txtAddress.Text ||
                                    reader["gender"].ToString() != cbGender.Text ||
                                    reader["age"].ToString() != txtAge.Text ||
                                    reader["sport"].ToString() != cbSport.Text ||
                                    reader["team"].ToString() != txtTeam.Text ||
                                    reader["nickname"].ToString() != txtNickname.Text ||
                                    reader["position"].ToString() != cbPosition.Text;

                                if (!hasChanges)
                                {
                                    MessageBox.Show("No changes!");
                                    return;
                                }
                            }
                        }
                    }

                    string updateQuery = @"UPDATE users SET
                        lastname=@lastname, firstname=@firstname, address=@address,
                        gender=@gender, age=@age, sport=@sport,
                        team=@team, nickname=@nickname, position=@position
                        WHERE id=@id";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@firstname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@gender", cbGender.Text);
                        cmd.Parameters.AddWithValue("@age", txtAge.Text);
                        cmd.Parameters.AddWithValue("@sport", cbSport.Text);
                        cmd.Parameters.AddWithValue("@team", txtTeam.Text);
                        cmd.Parameters.AddWithValue("@nickname", txtNickname.Text);
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
    }
}