using System;
using System.Drawing;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace P1130
{
    public partial class Form1 : Form
    {
        string user_id = "";
        string connStr = "";
        string prog_name = "";

        string org_password = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";

            if (!set_up())      // 取得程式參數,並檢查權限
                this.Close();

            textBox1.Text = user_id;
            this.Text = prog_name;

            // 不允許更改大小
            //this.MinimumSize = new Size(513, 254); //(width, height)
            //this.MaximumSize = new Size(513, 254);

            textBox2.Focus();
            textBox2.Refresh();
        }

        // 取得程式參數,並檢查權限
        private bool set_up()
        {
            bool ret_value = true;

            string[] args = Environment.GetCommandLineArgs();   // 取得程式執行的參數
            if (args.Length != 4)
            {
                MessageBox.Show("請找資訊人員", "執行錯誤");
                ret_value = false;
            }
            else
            {
                user_id = args[1];
                connStr = args[2].Replace("_", " ");
                prog_name = args[3];

                if (!check_authority(user_id))    // 檢查程式執行權限
                {
                    MessageBox.Show("你沒有此程式的權限 - " + user_id, prog_name);
                    ret_value = false;
                }
            }

            return ret_value;
        }

        // 檢查程式執行權限
        private bool check_authority(string user_id)
        {
            bool ret_value = false;

            org_password = "";

            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string SQLcmd = "SELECT a.使用者代號, a.使用者密碼, b.程式名稱 ";
            SQLcmd += "FROM 使用者 a ";
            SQLcmd += "LEFT JOIN 權限管理 b ON a.使用者代號 = b.使用者代號 ";
            SQLcmd += "WHERE a.使用者代號 = '" + user_id + "' AND b.程式名稱 = '" + prog_name + "'";

            SqlCommand cmd = new SqlCommand(SQLcmd, conn);
            SqlDataReader myData = cmd.ExecuteReader();

            if (myData.Read())
            {
                org_password = myData[1].ToString();
                ret_value = true;
            }
            else
            {
                MessageBox.Show("你沒有系統密碼更改權限", prog_name + " " + user_id);
                ret_value = false;
            }

            myData.Close();
            cmd.Dispose();

            conn.Close();
            conn.Dispose();

            return ret_value;
        }

        private void 確定_Click(object sender, EventArgs e)
        {
            string new_password = textBox3.Text.Trim();

            if (org_password == textBox2.Text.Trim())
            {

                update_password(user_id, new_password);

                toolStripStatusLabel1.Text = "密碼更改完成";
                toolStripStatusLabel1.ForeColor = Color.Green;
            }
            else
            {
                toolStripStatusLabel1.Text = "原先密碼錯誤";
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
        }

        private void update_password(string user_id, string new_password)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                string SQL = "UPDATE 使用者 SET 使用者密碼 = '" + new_password + "' ";
                SQL += "WHERE 使用者代號 = '" + user_id + "'";

                SqlCommand cmd = new SqlCommand(SQL, conn);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                conn.Dispose();
                conn.Close();
            }
            catch (Exception err)
            {
                toolStripStatusLabel1.Text = err.Message;
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
        }

        private void 取消_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
