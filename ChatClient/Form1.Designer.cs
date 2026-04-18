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
            btnDownloadFile = new Button();
            lstFileList = new ListBox();
            btnSendFile = new Button();
            btnSend = new Button();
            txtInput = new TextBox();
            rtbChat = new RichTextBox();
            lstOnlineUsers = new ListBox();
            lblOnlineUsers = new Label();
            grpGroup = new GroupBox();
            btnLeaveGroup = new Button();
            btnJoinGroup = new Button();
            lstGroups = new ListBox();
            lblGroupList = new Label();
            grpAuth.SuspendLayout();
            grpChat.SuspendLayout();
            grpGroup.SuspendLayout();
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
            grpChat.Controls.Add(btnDownloadFile);
            grpChat.Controls.Add(lstFileList);
            grpChat.Controls.Add(btnSendFile);
            grpChat.Controls.Add(btnSend);
            grpChat.Controls.Add(txtInput);
            grpChat.Controls.Add(rtbChat);
            grpChat.Enabled = false;
            grpChat.Location = new Point(330, 12);
            grpChat.Name = "grpChat";
            grpChat.Size = new Size(600, 618);
            grpChat.TabIndex = 1;
            grpChat.TabStop = false;
            grpChat.Text = "Phòng Chat";
            // 
            // btnDownloadFile
            // 
            btnDownloadFile.Location = new Point(500, 565);
            btnDownloadFile.Name = "btnDownloadFile";
            btnDownloadFile.Size = new Size(80, 34);
            btnDownloadFile.TabIndex = 5;
            btnDownloadFile.Text = "Tải File";
            btnDownloadFile.Visible = false;
            btnDownloadFile.Click += btnDownloadFile_Click;
            // 
            // lstFileList
            // 
            lstFileList.ItemHeight = 25;
            lstFileList.Location = new Point(20, 30);
            lstFileList.Name = "lstFileList";
            lstFileList.Size = new Size(560, 454);
            lstFileList.TabIndex = 3;
            lstFileList.Visible = false;
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(500, 565);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(90, 34);
            btnSendFile.TabIndex = 4;
            btnSendFile.Text = "Gửi File";
            btnSendFile.Click += btnSendFile_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(400, 565);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(90, 34);
            btnSend.TabIndex = 0;
            btnSend.Text = "Gửi";
            btnSend.Click += btnSend_Click;
            // 
            // txtInput
            // 
            txtInput.Location = new Point(20, 565);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(370, 31);
            txtInput.TabIndex = 1;
            // 
            // rtbChat
            // 
            rtbChat.Location = new Point(20, 30);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(560, 520);
            rtbChat.TabIndex = 2;
            rtbChat.Text = "";
            // 
            // lstOnlineUsers
            // 
            lstOnlineUsers.ItemHeight = 25;
            lstOnlineUsers.Location = new Point(12, 200);
            lstOnlineUsers.Name = "lstOnlineUsers";
            lstOnlineUsers.Size = new Size(300, 179);
            lstOnlineUsers.TabIndex = 3;
            // 
            // lblOnlineUsers
            // 
            lblOnlineUsers.AutoSize = true;
            lblOnlineUsers.Location = new Point(12, 175);
            lblOnlineUsers.Name = "lblOnlineUsers";
            lblOnlineUsers.Size = new Size(144, 25);
            lblOnlineUsers.TabIndex = 2;
            lblOnlineUsers.Text = "Đang trực tuyến:";
            // 
            // grpGroup
            // 
            grpGroup.Controls.Add(btnLeaveGroup);
            grpGroup.Controls.Add(btnJoinGroup);
            grpGroup.Controls.Add(lstGroups);
            grpGroup.Controls.Add(lblGroupList);
            grpGroup.Location = new Point(12, 410);
            grpGroup.Name = "grpGroup";
            grpGroup.Size = new Size(300, 220);
            grpGroup.TabIndex = 4;
            grpGroup.TabStop = false;
            grpGroup.Text = "Nhóm Chat";
            // 
            // btnLeaveGroup
            // 
            btnLeaveGroup.Enabled = false;
            btnLeaveGroup.Location = new Point(160, 160);
            btnLeaveGroup.Name = "btnLeaveGroup";
            btnLeaveGroup.Size = new Size(120, 34);
            btnLeaveGroup.TabIndex = 3;
            btnLeaveGroup.Text = "Rời Nhóm";
            btnLeaveGroup.Click += btnLeaveGroup_Click;
            // 
            // btnJoinGroup
            // 
            btnJoinGroup.Enabled = false;
            btnJoinGroup.Location = new Point(20, 160);
            btnJoinGroup.Name = "btnJoinGroup";
            btnJoinGroup.Size = new Size(130, 34);
            btnJoinGroup.TabIndex = 2;
            btnJoinGroup.Text = "Tham gia Nhóm";
            btnJoinGroup.Click += btnJoinGroup_Click;
            // 
            // lstGroups
            // 
            lstGroups.ItemHeight = 25;
            lstGroups.Items.AddRange(new object[] { "Group 1", "Group 2", "Group 3" });
            lstGroups.Location = new Point(20, 50);
            lstGroups.Name = "lstGroups";
            lstGroups.Size = new Size(260, 79);
            lstGroups.TabIndex = 1;
            lstGroups.SelectedIndexChanged += lstGroups_SelectedIndexChanged;
            // 
            // lblGroupList
            // 
            lblGroupList.AutoSize = true;
            lblGroupList.Location = new Point(20, 25);
            lblGroupList.Name = "lblGroupList";
            lblGroupList.Size = new Size(110, 25);
            lblGroupList.TabIndex = 0;
            lblGroupList.Text = "Chọn nhóm:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(950, 640);
            Controls.Add(grpGroup);
            Controls.Add(lstOnlineUsers);
            Controls.Add(lblOnlineUsers);
            Controls.Add(grpChat);
            Controls.Add(grpAuth);
            Name = "Form1";
            Text = "Chat Client";
            grpAuth.ResumeLayout(false);
            grpAuth.PerformLayout();
            grpChat.ResumeLayout(false);
            grpChat.PerformLayout();
            grpGroup.ResumeLayout(false);
            grpGroup.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private Button btnSendFile;
        private ListBox lstOnlineUsers;
        private Label lblOnlineUsers;
        private GroupBox grpGroup;
        private ListBox lstGroups;
        private Label lblGroupList;
        private Button btnJoinGroup;
        private Button btnLeaveGroup;
        private ListBox lstFileList;
        private Button btnDownloadFile;
    }
}