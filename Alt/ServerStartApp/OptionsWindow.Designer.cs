namespace ServerStartApp
{
    partial class OptionsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsWindow));
            this.preLabel = new System.Windows.Forms.Label();
            this.postLabel = new System.Windows.Forms.Label();
            this.preTextBox = new System.Windows.Forms.RichTextBox();
            this.postTextBox = new System.Windows.Forms.RichTextBox();
            this.postButton = new System.Windows.Forms.Button();
            this.preStartButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // preLabel
            // 
            this.preLabel.AutoSize = true;
            this.preLabel.Location = new System.Drawing.Point(12, 17);
            this.preLabel.Name = "preLabel";
            this.preLabel.Size = new System.Drawing.Size(35, 13);
            this.preLabel.TabIndex = 0;
            this.preLabel.Text = "label1";
            // 
            // postLabel
            // 
            this.postLabel.AutoSize = true;
            this.postLabel.Location = new System.Drawing.Point(12, 241);
            this.postLabel.Name = "postLabel";
            this.postLabel.Size = new System.Drawing.Size(35, 13);
            this.postLabel.TabIndex = 1;
            this.postLabel.Text = "label2";
            // 
            // preTextBox
            // 
            this.preTextBox.Location = new System.Drawing.Point(15, 41);
            this.preTextBox.Name = "preTextBox";
            this.preTextBox.Size = new System.Drawing.Size(773, 189);
            this.preTextBox.TabIndex = 2;
            this.preTextBox.Text = "";
            this.preTextBox.WordWrap = false;
            // 
            // postTextBox
            // 
            this.postTextBox.Location = new System.Drawing.Point(12, 265);
            this.postTextBox.Name = "postTextBox";
            this.postTextBox.Size = new System.Drawing.Size(773, 189);
            this.postTextBox.TabIndex = 3;
            this.postTextBox.Text = "";
            // 
            // postButton
            // 
            this.postButton.Location = new System.Drawing.Point(195, 236);
            this.postButton.Name = "postButton";
            this.postButton.Size = new System.Drawing.Size(75, 23);
            this.postButton.TabIndex = 4;
            this.postButton.Text = "button1";
            this.postButton.UseVisualStyleBackColor = true;
            this.postButton.Click += new System.EventHandler(this.postButton_Click);
            // 
            // preStartButton
            // 
            this.preStartButton.Location = new System.Drawing.Point(195, 12);
            this.preStartButton.Name = "preStartButton";
            this.preStartButton.Size = new System.Drawing.Size(75, 23);
            this.preStartButton.TabIndex = 5;
            this.preStartButton.Text = "button2";
            this.preStartButton.UseVisualStyleBackColor = true;
            this.preStartButton.Click += new System.EventHandler(this.preStartButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(677, 460);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "button2";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 495);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.preStartButton);
            this.Controls.Add(this.postButton);
            this.Controls.Add(this.postTextBox);
            this.Controls.Add(this.preTextBox);
            this.Controls.Add(this.postLabel);
            this.Controls.Add(this.preLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OptionsWindow";
            this.Text = "OptionsWindow";
            this.Load += new System.EventHandler(this.OptionsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label preLabel;
        private System.Windows.Forms.Label postLabel;
        private System.Windows.Forms.RichTextBox preTextBox;
        private System.Windows.Forms.RichTextBox postTextBox;
        private System.Windows.Forms.Button postButton;
        private System.Windows.Forms.Button preStartButton;
        private System.Windows.Forms.Button saveButton;
    }
}