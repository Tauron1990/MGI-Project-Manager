namespace ServerStartApp
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LogConsole = new System.Windows.Forms.RichTextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.optionsButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.shutdownButton = new System.Windows.Forms.Button();
            this.hiberateCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LogConsole
            // 
            resources.ApplyResources(this.LogConsole, "LogConsole");
            this.LogConsole.Name = "LogConsole";
            this.LogConsole.ReadOnly = true;
            // 
            // startButton
            // 
            resources.ApplyResources(this.startButton, "startButton");
            this.startButton.Name = "startButton";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // optionsButton
            // 
            resources.ApplyResources(this.optionsButton, "optionsButton");
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.UseVisualStyleBackColor = true;
            this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // shutdownButton
            // 
            resources.ApplyResources(this.shutdownButton, "shutdownButton");
            this.shutdownButton.Name = "shutdownButton";
            this.shutdownButton.UseVisualStyleBackColor = true;
            this.shutdownButton.Click += new System.EventHandler(this.shutdownButton_Click);
            // 
            // hiberateCheckBox
            // 
            resources.ApplyResources(this.hiberateCheckBox, "hiberateCheckBox");
            this.hiberateCheckBox.Name = "hiberateCheckBox";
            this.hiberateCheckBox.UseVisualStyleBackColor = true;
            this.hiberateCheckBox.CheckedChanged += new System.EventHandler(this.hiberateCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hiberateCheckBox);
            this.Controls.Add(this.shutdownButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.optionsButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.LogConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LogConsole;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button optionsButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button shutdownButton;
        private System.Windows.Forms.CheckBox hiberateCheckBox;
    }
}

