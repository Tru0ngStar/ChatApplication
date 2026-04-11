namespace ChatClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            grpAuth = new GroupBox();
            btnRegister = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            txtUsername = new TextBox();
            grpChat = new GroupBox();
            btnSend = new Button();
            txtInput = new TextBox();
            rtbChat = new RichTextBox();
            grpAuth.SuspendLayout();
            grpChat.SuspendLayout();
            SuspendLayout();
            // 
            // grpAuth
            // 
            grpAuth.Controls.Add(btnRegister);
            grpAuth.Controls.Add(btnLogin);
            grpAuth.Controls.Add(txtPassword);
            grpAuth.Controls.Add(txtUsername);
            grpAuth.Location = new Point(12, 12);
            grpAuth.Name = "grpAuth";
            grpAuth.Size = new Size(300, 150);
            grpAuth.TabIndex = 0;
            grpAuth.TabStop = false;
            grpAuth.Text = "Đăng nhập / Đăng ký";
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(150, 110);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(120, 34);
            btnRegister.TabIndex = 0;
            btnRegister.Text = "Đăng ký";
            btnRegister.Click += btnRegister_Click;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(20, 110);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(120, 34);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Đăng nhập";
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(20, 70);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.PlaceholderText = "Mật khẩu";
            txtPassword.Size = new Size(250, 31);
            txtPassword.TabIndex = 2;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(20, 30);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Tên đăng nhập";
            txtUsername.Size = new Size(250, 31);
            txtUsername.TabIndex = 3;
            // 
            // grpChat
            // 
            grpChat.Controls.Add(btnSend);
            grpChat.Controls.Add(txtInput);
            grpChat.Controls.Add(rtbChat);
            grpChat.Enabled = false;
            grpChat.Location = new Point(330, 12);
            grpChat.Name = "grpChat";
            grpChat.Size = new Size(500, 400);
            grpChat.TabIndex = 1;
            grpChat.TabStop = false;
            grpChat.Text = "Phòng Chat";
            // 
            // btnSend
            // 
            btnSend.Location = new Point(380, 343);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(100, 34);
            btnSend.TabIndex = 0;
            btnSend.Text = "Gửi";
            btnSend.Click += btnSend_Click;
            // 
            // txtInput
            // 
            txtInput.Location = new Point(20, 345);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(350, 31);
            txtInput.TabIndex = 1;
            // 
            // rtbChat
            // 
            rtbChat.Location = new Point(20, 30);
            rtbChat.Name = "rtbChat";
            rtbChat.Size = new Size(460, 300);
            rtbChat.TabIndex = 2;
            rtbChat.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(850, 430);
            Controls.Add(grpChat);
            Controls.Add(grpAuth);
            Name = "Form1";
            Text = "Chat Client";
            grpAuth.ResumeLayout(false);
            grpAuth.PerformLayout();
            grpChat.ResumeLayout(false);
            grpChat.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpAuth;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private GroupBox grpChat;
        private RichTextBox rtbChat;
        private TextBox txtInput;
        private Button btnSend;
    }
}