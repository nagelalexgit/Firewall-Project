using System;
using System.Windows.Forms;
using NetFwTypeLib;
using System.IO;

namespace Frewall_Blocker
{
    public partial class FirewallBlocker : Form
    {
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        public FirewallBlocker()
        {
            InitializeComponent();
        }

        private void btnBlock_Click(object sender, EventArgs e)
        {
            FWRule(@txtPath.Text, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, "1");
        }
        private void btnUnblock_Click(object sender, EventArgs e)
        {
            FWRule(@txtPath.Text, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, "0");
        }

        void Form_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        void Form_DragDrop(object sender, DragEventArgs e)
        {
            string[] path = (string[])e.Data.GetData(DataFormats.FileDrop);
            //if (path != null && path.Length != 0)
            //{
                txtPath.Text = path[0];
            //}
        }

        private void FWRule(string path, NET_FW_RULE_DIRECTION_ d,
        NET_FW_ACTION_ fwaction, string action)
        {
            try
            {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = fwaction;
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.ApplicationName = path;
                firewallRule.Name = "Frewall_Blocker: " + Path.GetFileName(path);
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance
                (Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallRule.Direction = d;
                if (action == "1")
                {
                    firewallPolicy.Rules.Add(firewallRule);
                    MessageBox.Show("Program is blocked");
                }
                else
                {
                    firewallPolicy.Rules.Remove(firewallRule.Name);
                    MessageBox.Show("Program is unblocked");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "ERROR"); }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    string fullPath = openFileDialog1.FileName;
                    txtPath.Text = fullPath;
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
