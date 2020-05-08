namespace KENKENNN
{
    partial class Form1
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
            this.CompleteButton = new System.Windows.Forms.Button();
            this.AutoComleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CompleteButton
            // 
            this.CompleteButton.Location = new System.Drawing.Point(33, 215);
            this.CompleteButton.Name = "CompleteButton";
            this.CompleteButton.Size = new System.Drawing.Size(134, 37);
            this.CompleteButton.TabIndex = 0;
            this.CompleteButton.Text = "Complete";
            this.CompleteButton.UseVisualStyleBackColor = true;
            this.CompleteButton.Click += new System.EventHandler(this.CompleteButton_Click);
            // 
            // AutoComleteButton
            // 
            this.AutoComleteButton.Location = new System.Drawing.Point(33, 258);
            this.AutoComleteButton.Name = "AutoComleteButton";
            this.AutoComleteButton.Size = new System.Drawing.Size(134, 37);
            this.AutoComleteButton.TabIndex = 2;
            this.AutoComleteButton.Text = "AutoComplete";
            this.AutoComleteButton.UseVisualStyleBackColor = true;
            this.AutoComleteButton.Click += new System.EventHandler(this.AutoComleteButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 310);
            this.Controls.Add(this.AutoComleteButton);
            this.Controls.Add(this.CompleteButton);
            this.Name = "Form1";
            this.Text = "KenKen";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CompleteButton;
        private System.Windows.Forms.Button AutoComleteButton;
    }
}

