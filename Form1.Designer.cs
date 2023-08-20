namespace EVE_AutomatiX
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.createBotButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.botSettingsBtn1 = new System.Windows.Forms.Button();
            this.startBot1 = new System.Windows.Forms.Button();
            this.changeNickNameBtn = new System.Windows.Forms.Button();
            this.removeBtn = new System.Windows.Forms.Button();
            this.nickNameLabel1 = new System.Windows.Forms.Label();
            this.LOGGER = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 304);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 33);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start all bots";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Start_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 304);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 33);
            this.button2.TabIndex = 1;
            this.button2.Text = "Launcher settings";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Settings_Click);
            // 
            // createBotButton
            // 
            this.createBotButton.Location = new System.Drawing.Point(12, 86);
            this.createBotButton.Name = "createBotButton";
            this.createBotButton.Size = new System.Drawing.Size(113, 33);
            this.createBotButton.TabIndex = 3;
            this.createBotButton.Text = "create new bot";
            this.createBotButton.UseVisualStyleBackColor = true;
            this.createBotButton.Click += new System.EventHandler(this.CreateNewBot_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "NickName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(469, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(469, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "disabled";
            // 
            // botSettingsBtn1
            // 
            this.botSettingsBtn1.Location = new System.Drawing.Point(287, 45);
            this.botSettingsBtn1.Name = "botSettingsBtn1";
            this.botSettingsBtn1.Size = new System.Drawing.Size(24, 20);
            this.botSettingsBtn1.TabIndex = 7;
            this.botSettingsBtn1.Text = "*";
            this.botSettingsBtn1.UseVisualStyleBackColor = true;
            this.botSettingsBtn1.Click += new System.EventHandler(this.botSettingsBtn1_Click);
            // 
            // startBot1
            // 
            this.startBot1.Location = new System.Drawing.Point(334, 44);
            this.startBot1.Name = "startBot1";
            this.startBot1.Size = new System.Drawing.Size(111, 23);
            this.startBot1.TabIndex = 8;
            this.startBot1.Text = "Start";
            this.startBot1.UseVisualStyleBackColor = true;
            this.startBot1.Click += new System.EventHandler(this.startBot1_Click);
            // 
            // changeNickNameBtn
            // 
            this.changeNickNameBtn.Location = new System.Drawing.Point(152, 44);
            this.changeNickNameBtn.Name = "changeNickNameBtn";
            this.changeNickNameBtn.Size = new System.Drawing.Size(111, 23);
            this.changeNickNameBtn.TabIndex = 9;
            this.changeNickNameBtn.Text = "change nickname";
            this.changeNickNameBtn.UseVisualStyleBackColor = true;
            this.changeNickNameBtn.Click += new System.EventHandler(this.changeNickNameBtn_Click);
            // 
            // removeBtn
            // 
            this.removeBtn.Location = new System.Drawing.Point(589, 44);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(75, 23);
            this.removeBtn.TabIndex = 10;
            this.removeBtn.Text = "remove";
            this.removeBtn.UseVisualStyleBackColor = true;
            // 
            // nickNameLabel1
            // 
            this.nickNameLabel1.AutoSize = true;
            this.nickNameLabel1.Location = new System.Drawing.Point(12, 49);
            this.nickNameLabel1.Name = "nickNameLabel1";
            this.nickNameLabel1.Size = new System.Drawing.Size(53, 13);
            this.nickNameLabel1.TabIndex = 11;
            this.nickNameLabel1.Text = "nickname";
            // 
            // LOGGER
            // 
            this.LOGGER.AutoSize = true;
            this.LOGGER.Location = new System.Drawing.Point(420, 170);
            this.LOGGER.Name = "LOGGER";
            this.LOGGER.Size = new System.Drawing.Size(35, 13);
            this.LOGGER.TabIndex = 12;
            this.LOGGER.Text = "label4";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 349);
            this.Controls.Add(this.LOGGER);
            this.Controls.Add(this.nickNameLabel1);
            this.Controls.Add(this.removeBtn);
            this.Controls.Add(this.changeNickNameBtn);
            this.Controls.Add(this.startBot1);
            this.Controls.Add(this.botSettingsBtn1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.createBotButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "EVE AutomatiX";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button createBotButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button botSettingsBtn1;
        private System.Windows.Forms.Button startBot1;
        private System.Windows.Forms.Button changeNickNameBtn;
        private System.Windows.Forms.Button removeBtn;
        private System.Windows.Forms.Label nickNameLabel1;
        private System.Windows.Forms.Label LOGGER;
    }
}

