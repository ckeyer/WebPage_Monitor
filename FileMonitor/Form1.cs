using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FileMonitor
{
    public partial class Form1 : Form
    {
        private CJMonitor mainMonitor = new CJMonitor();
        private List<CheckBox> FilterCheck = new List<CheckBox>();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != null)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox1.Text = path;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            save_tsbtn.Visible = false;
            versionInfo_tsbtn.Visible = false;
            button6.Visible = false;
            button7.Visible = false;

            FilterCheck.Add(checkBox1);
            FilterCheck.Add(checkBox2);
            FilterCheck.Add(checkBox3);
            FilterCheck.Add(checkBox4);
            FilterCheck.Add(checkBox5);
            FilterCheck.Add(checkBox6);
            FilterCheck.Add(checkBox7);
            FilterCheck.Add(checkBox8);
            FilterCheck.Add(checkBox9);
            FilterCheck.Add(checkBox10); 
            FilterCheck[0].Text = "*.*";
            FilterCheck[1].Text = "*.txt";
            FilterCheck[2].Text = "*.htm";
            FilterCheck[3].Text = "*.html";
            FilterCheck[4].Text = "*.js";
            FilterCheck[5].Text = "*.css";
            FilterCheck[6].Text = "*.asp";
            FilterCheck[7].Text = "*.aspx";
            FilterCheck[8].Text = "*.jsp";
            FilterCheck[9].Text = "*.php";
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView1.Columns.Add("监控目录", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("监控文件类型", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("标准目录", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("备份目录", 120, HorizontalAlignment.Left);
            listView1.SelectedIndexChanged += new EventHandler(listView1_ItemChecked);

            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.View = View.Details;
            listView2.Scrollable = true;
            listView2.MultiSelect = false;
            listView2.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView2.Columns.Add("时间", 120, HorizontalAlignment.Left);
            listView2.Columns.Add("文件名", 120, HorizontalAlignment.Left);
            listView2.Columns.Add("全路径", 250, HorizontalAlignment.Left);
            listView2.Columns.Add("操作类型", 80, HorizontalAlignment.Left);
            listView2.Columns.Add("其它", 300, HorizontalAlignment.Left);

            button6.Enabled = false;
            button7.Enabled = false;

            stop_tsbtn.Enabled = true;
            start_tsbtn.Enabled = false;
            labMonitorStatus.Text = "\t监控状态：正在监控中...";

            mainMonitor.MsgSend += new MsgSendHandle(ChageControl);

            string filterTmp = "";
            foreach (CJFolder fold in this.mainMonitor.folders)
            {
                foreach (string str in fold.Filter)
                {
                    if (filterTmp == "")
                    {
                        filterTmp = str;
                    }
                    else
                    {
                        filterTmp += ", " + str;
                    }
                }
                string[] strs = { fold.DirPath, filterTmp, fold.StandDir,fold.BackupDir};
                ListViewItem itm = new ListViewItem(strs);
                listView1.Items.Add(itm);
            }
        }
        private void listView1_ItemChecked(object sender, EventArgs e)
        {
            if (1 == listView1.SelectedItems.Count)
            {
                ListView.SelectedIndexCollection selected=
                    listView1.SelectedIndices;
                
                textBox1.Text = listView1.FocusedItem.SubItems[0].Text;
                textBox3.Text = listView1.FocusedItem.SubItems[2].Text;
                textBox4.Text = listView1.FocusedItem.SubItems[3].Text;
                string filtersTmp = listView1.FocusedItem.SubItems[1].Text;
                foreach (string str in filtersTmp.Split(','))
                {
                    foreach (CheckBox ck in FilterCheck)
                    {
                        if (str.Trim() == ck.Text)
                        {
                            ck.Checked = true;
                        }
                    }
                }
                button6.Enabled = true;
                button7.Enabled = true;
            }
        }
        private void ChageControl(object sender,MsgEvent e)
        {
            if (e.Type == "Changed" || e.Type == "Created" || e.Type == "Deleted"||e.Type=="Renamed")
            {
                DLG_Enent dlg = new DLG_Enent(DoChageControl);
                listView2.Invoke(dlg, e);
            }
        }
        public delegate void DLG_Enent(MsgEvent e);
        public void DoChageControl(MsgEvent e)
        {
            if (e.Type == "Changed")
            {
                string[] strs = { DateTime.Now.ToString(), e.Msg[0], e.Msg[1], "内容更改","" };
                ListViewItem itm = new ListViewItem(strs);
                listView2.Items.Add(itm);
            }
            else if (e.Type == "Created")
            {
                string[] strs = { DateTime.Now.ToString(), e.Msg[0], e.Msg[1], "添加文件", "" };
                ListViewItem itm = new ListViewItem(strs);
                listView2.Items.Add(itm);
            }
            else if (e.Type == "Deleted")
            {
                string[] strs = { DateTime.Now.ToString(), e.Msg[0], e.Msg[1], "删除文件", "" };
                ListViewItem itm = new ListViewItem(strs);
                listView2.Items.Add(itm);
            }
            else if (e.Type == "Renamed")
            {
                string[] strs = { DateTime.Now.ToString(), e.Msg[0], e.Msg[1], "重命名", "新文件名："+e.Msg[2] };
                ListViewItem itm = new ListViewItem(strs);
                listView2.Items.Add(itm);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            List<string> filter = new List<string>();
            for (int i = 0; i < FilterCheck.Count; i++)
            {
                if (FilterCheck[i].Checked)
                {
                    filter.Add(FilterCheck[i].Text);
                }
            }
            if (filter.Count==0)
            {
                MessageBox.Show("请勾选需要监控的文件类型","添加监控失败");
                return;
            }
            if (textBox1.Text=="" || !Directory.Exists(textBox1.Text))
            {
                MessageBox.Show("没有找到所选监控目录", "添加监控失败");
                return;
            } 
            if (textBox3.Text==""  || !Directory.Exists(textBox3.Text))
            {
                MessageBox.Show("没有找到所选标准目录", "添加监控失败");
                return;
            }
            if (textBox4.Text == "" || !Directory.Exists(textBox4.Text))
            {
                MessageBox.Show("没有找到所选备份目录", "添加监控失败");
                return;
            }
            mainMonitor.AddFolder(textBox1.Text,filter,textBox3.Text,textBox4.Text);
            string filterTmp = "";
            foreach (string str in filter)
            {
                if (filterTmp == "")
                {
                    filterTmp = str;
                }
                else
                {
                    filterTmp += "|" + str;
                }
            }
            string[] strs = { textBox1.Text, filterTmp, textBox3.Text, textBox4.Text };
            ListViewItem itm = new ListViewItem(strs);
            listView1.Items.Add(itm);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] ss = {"1","D://Tmp","*.txt","234234"};
            ListViewItem itm = new ListViewItem(ss);
            listView1.Items.Add(itm);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowNewFolderButton = false;
            folderBrowserDialog2.ShowDialog();
            if (folderBrowserDialog2.SelectedPath != null)
            {
                string path = folderBrowserDialog2.SelectedPath;
                textBox3.Text = path;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            folderBrowserDialog3.ShowNewFolderButton = false;
            folderBrowserDialog3.ShowDialog();
            if (folderBrowserDialog3.SelectedPath != null)
            {
                string path = folderBrowserDialog3.SelectedPath;
                textBox4.Text = path;
            }
        }

        private void start_tsbtn_Click(object sender, EventArgs e)
        {
            foreach (CJFolder fold in mainMonitor.folders)
            {
                fold.fileWatcher.EnableRaisingEvents = true;
            }
            stop_tsbtn.Enabled = true;
            start_tsbtn.Enabled = false;
            labMonitorStatus.Text = "\t监控状态：正在监控中...";
        }

        private void stop_tsbtn_Click(object sender, EventArgs e)
        {
            foreach (CJFolder fold in mainMonitor.folders)
            {
                fold.fileWatcher.EnableRaisingEvents = false;
            }
            stop_tsbtn.Enabled = false;
            start_tsbtn.Enabled = true;
            labMonitorStatus.Text = "\t监控状态：监控已停止";
        }

        private void clear_tsbtn_Click(object sender, EventArgs e)
        {
            listView2.Clear();
            listView2.Columns.Add("时间", 120, HorizontalAlignment.Left);
            listView2.Columns.Add("文件名", 120, HorizontalAlignment.Left);
            listView2.Columns.Add("全路径", 250, HorizontalAlignment.Left);
            listView2.Columns.Add("操作类型", 80, HorizontalAlignment.Left);
            listView2.Columns.Add("其它", 300, HorizontalAlignment.Left);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(new Point(MousePosition.X,MousePosition.Y));
            }
        }
    }
}
