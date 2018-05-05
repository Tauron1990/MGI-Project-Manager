using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog.Config;
using NLog.Layouts;
using NLog.Windows.Forms;
using ServerStartApp.Core;
using ServerStartApp.Properties;
using ServerStartApp.Resources;
using Tauron.Application.ProjectManager;
using Tauron.Application.ProjectManager.ApplicationServer;
using LogLevel = NLog.LogLevel;

namespace ServerStartApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startButton.Text         = Local.MainForm_Button_Start;
            optionsButton.Text       = Local.MainForm_Button_Options;
            closeButton.Text         = Local.MainForm_Button_Close;
            shutdownButton.Text      = Local.MainForm_Button_Shutdown;
            hiberateCheckBox.Text    = Local.MainForm_CheckBox_Hiberate;
            hiberateCheckBox.Checked = Settings.Default.SetHiberate;

            Bootstrapper.ConfigurateLogging += BootstrapperOnConfigurateLogging;
            MessageHelper.Initialize(LogConsole);
        }

        private void BootstrapperOnConfigurateLogging(LoggingConfiguration obj)
        {
            var target = new RichTextBoxTarget
                         {
                             Name                       = "UI-Target",
                             AllowAccessoryFormCreation = false,
                             AutoScroll                 = true,
                             TargetForm                 = this,
                             TargetRichTextBox          = LogConsole,
                             MaxLines                   = 300,
                             UseDefaultRowColoringRules = true,
                             Layout                     = new SimpleLayout("${longdate}|${level:uppercase=false}|${logger:shortName=true}|${message}")
                         };
            var rule = new LoggingRule("*", LogLevel.Trace, target);
            obj.AddTarget(target);
            obj.LoggingRules.Add(rule);
        }

        private void hiberateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var set = Settings.Default;
            set.SetHiberate = hiberateCheckBox.Checked;

            set.Save();
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            var options = new OptionsWindow();

            options.ShowDialog(this);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            LogConsole.Text = string.Empty;
            startButton.Enabled = false;

            Task.Run(() =>
                     {
                         SofwareManager.Start(StartType.PreStart);
                         Bootstrapper.Start(false, IpSettings.CreateDefault());
                         SofwareManager.Start(StartType.PostStart);
                     }).ContinueWith(t => Invoke(new Action(() =>
                                                  {
                                                      shutdownButton.Enabled = true;
                                                      closeButton.Enabled = true;
                                                  })));
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            closeButton.Enabled = false;
            shutdownButton.Enabled = false;

            Task.Run(() => Bootstrapper.Stop()).ContinueWith(t => Invoke(new Action(() => startButton.Enabled = true)));
        }

        private void shutdownButton_Click(object sender, EventArgs e)
        {
            closeButton.Enabled    = false;
            shutdownButton.Enabled = false;

            Task.Run(() =>
                     {
                         Bootstrapper.Stop();
                         Process.Start("shutdown", Settings.Default.SetHiberate ? "/h /f" : "/s /t 0");
                         Invoke(new Action(Close));
                     });
        }
    }
}
