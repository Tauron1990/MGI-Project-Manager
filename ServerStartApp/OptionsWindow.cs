using System;
using System.IO;
using System.Windows.Forms;
using ServerStartApp.Properties;
using ServerStartApp.Resources;

namespace ServerStartApp
{
    public partial class OptionsWindow : Form
    {
        private Settings _settings = Settings.Default;

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void OptionsWindow_Load(object sender, System.EventArgs e)
        {
            Text = Local.OptionsWindow_Form_Title;
            preLabel.Text = Local.OptionsWindow_Label_PreStart;
            preStartButton.Text = Local.OptionsWindow_Button_PreStart;
            postButton.Text = Local.OptionsWindow_Button_PostStart;
            postLabel.Text = Local.OptionsWindow_Label_PostStart;
            saveButton.Text = Local.OptionsWindow_Button_Save;

            preTextBox.Text = _settings.Prestart;
            postTextBox.Text = _settings.PostStart;
        }

        private void preStartButton_Click(object sender, System.EventArgs e) => AddText(preTextBox, GetSoftware());
        private void AddText(RichTextBox box, string soft)
        {
            if(string.IsNullOrWhiteSpace(soft) || !File.Exists(soft)) return;

            if (string.IsNullOrWhiteSpace(box.Text))
                box.Text = soft;
            else
                box.Text = box.Text + Environment.NewLine + soft;
        }
        private string GetSoftware()
        {
            FileDialog diag = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true
            };

            if (diag.ShowDialog(this) == DialogResult.OK)
                return diag.FileName;
            return null;
        }
        private void postButton_Click(object sender, EventArgs e) => AddText(postTextBox, GetSoftware());
        private void saveButton_Click(object sender, EventArgs e)
        {
            _settings.Save();
            DialogResult = DialogResult.OK;
        }
    }
}
