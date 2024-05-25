namespace ChineseChess
{
    partial class NewGameDialog
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
            okButton = new Button();
            cancelButton = new Button();
            gameTypeGroup = new GroupBox();
            gameType_AIDifficulty = new ComboBox();
            gameType_Handicap = new CheckBox();
            gameType_VsAI = new RadioButton();
            gameType_TwoPlayer = new RadioButton();
            playersInformationGroup = new GroupBox();
            playersInformation_Player2AI = new RadioButton();
            playersInformation_Player1AI = new RadioButton();
            playersInformation_Player2Name = new TextBox();
            playersInformation_Player2Name_Label = new Label();
            playersInformation_Player1Name = new TextBox();
            playersInformation_Player1Name_Label = new Label();
            gameTypeGroup.SuspendLayout();
            playersInformationGroup.SuspendLayout();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(265, 181);
            okButton.Name = "okButton";
            okButton.Size = new Size(66, 28);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(336, 181);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(66, 28);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // gameTypeGroup
            // 
            gameTypeGroup.Controls.Add(gameType_AIDifficulty);
            gameTypeGroup.Controls.Add(gameType_Handicap);
            gameTypeGroup.Controls.Add(gameType_VsAI);
            gameTypeGroup.Controls.Add(gameType_TwoPlayer);
            gameTypeGroup.Location = new Point(10, 11);
            gameTypeGroup.Name = "gameTypeGroup";
            gameTypeGroup.Size = new Size(122, 164);
            gameTypeGroup.TabIndex = 0;
            gameTypeGroup.TabStop = false;
            gameTypeGroup.Text = "Game type";
            // 
            // gameType_AIDifficulty
            // 
            gameType_AIDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            gameType_AIDifficulty.Enabled = false;
            gameType_AIDifficulty.FormattingEnabled = true;
            gameType_AIDifficulty.Items.AddRange(new object[] { "Easy", "Medium", "Hard", "Extreme" });
            gameType_AIDifficulty.Location = new Point(6, 79);
            gameType_AIDifficulty.Name = "gameType_AIDifficulty";
            gameType_AIDifficulty.Size = new Size(110, 23);
            gameType_AIDifficulty.TabIndex = 3;
            // 
            // gameType_Handicap
            // 
            gameType_Handicap.AutoSize = true;
            gameType_Handicap.Location = new Point(5, 139);
            gameType_Handicap.Name = "gameType_Handicap";
            gameType_Handicap.Size = new Size(77, 19);
            gameType_Handicap.TabIndex = 2;
            gameType_Handicap.Text = "Handicap";
            gameType_Handicap.UseVisualStyleBackColor = true;
            // 
            // gameType_VsAI
            // 
            gameType_VsAI.AutoSize = true;
            gameType_VsAI.Location = new Point(5, 53);
            gameType_VsAI.Name = "gameType_VsAI";
            gameType_VsAI.Size = new Size(50, 19);
            gameType_VsAI.TabIndex = 1;
            gameType_VsAI.Text = "vs AI";
            gameType_VsAI.UseVisualStyleBackColor = true;
            // 
            // gameType_TwoPlayer
            // 
            gameType_TwoPlayer.AutoSize = true;
            gameType_TwoPlayer.Checked = true;
            gameType_TwoPlayer.Location = new Point(5, 27);
            gameType_TwoPlayer.Name = "gameType_TwoPlayer";
            gameType_TwoPlayer.Size = new Size(71, 19);
            gameType_TwoPlayer.TabIndex = 0;
            gameType_TwoPlayer.TabStop = true;
            gameType_TwoPlayer.Text = "2 players";
            gameType_TwoPlayer.UseVisualStyleBackColor = true;
            gameType_TwoPlayer.CheckedChanged += GameType_CheckedChanged;
            // 
            // playersInformationGroup
            // 
            playersInformationGroup.Controls.Add(playersInformation_Player2AI);
            playersInformationGroup.Controls.Add(playersInformation_Player1AI);
            playersInformationGroup.Controls.Add(playersInformation_Player2Name);
            playersInformationGroup.Controls.Add(playersInformation_Player2Name_Label);
            playersInformationGroup.Controls.Add(playersInformation_Player1Name);
            playersInformationGroup.Controls.Add(playersInformation_Player1Name_Label);
            playersInformationGroup.Location = new Point(138, 14);
            playersInformationGroup.Name = "playersInformationGroup";
            playersInformationGroup.Size = new Size(264, 161);
            playersInformationGroup.TabIndex = 1;
            playersInformationGroup.TabStop = false;
            playersInformationGroup.Text = "Players information";
            // 
            // playersInformation_Player2AI
            // 
            playersInformation_Player2AI.AutoSize = true;
            playersInformation_Player2AI.Checked = true;
            playersInformation_Player2AI.Enabled = false;
            playersInformation_Player2AI.ForeColor = Color.Blue;
            playersInformation_Player2AI.Location = new Point(223, 51);
            playersInformation_Player2AI.Name = "playersInformation_Player2AI";
            playersInformation_Player2AI.Size = new Size(36, 19);
            playersInformation_Player2AI.TabIndex = 3;
            playersInformation_Player2AI.TabStop = true;
            playersInformation_Player2AI.Text = "AI";
            playersInformation_Player2AI.UseVisualStyleBackColor = true;
            // 
            // playersInformation_Player1AI
            // 
            playersInformation_Player1AI.AutoSize = true;
            playersInformation_Player1AI.Enabled = false;
            playersInformation_Player1AI.ForeColor = Color.Red;
            playersInformation_Player1AI.Location = new Point(223, 24);
            playersInformation_Player1AI.Name = "playersInformation_Player1AI";
            playersInformation_Player1AI.Size = new Size(36, 19);
            playersInformation_Player1AI.TabIndex = 1;
            playersInformation_Player1AI.Text = "AI";
            playersInformation_Player1AI.UseVisualStyleBackColor = true;
            // 
            // playersInformation_Player2Name
            // 
            playersInformation_Player2Name.ForeColor = Color.Blue;
            playersInformation_Player2Name.Location = new Point(63, 50);
            playersInformation_Player2Name.MaxLength = 20;
            playersInformation_Player2Name.Name = "playersInformation_Player2Name";
            playersInformation_Player2Name.Size = new Size(155, 23);
            playersInformation_Player2Name.TabIndex = 2;
            // 
            // playersInformation_Player2Name_Label
            // 
            playersInformation_Player2Name_Label.AutoSize = true;
            playersInformation_Player2Name_Label.ForeColor = Color.Blue;
            playersInformation_Player2Name_Label.Location = new Point(5, 52);
            playersInformation_Player2Name_Label.Name = "playersInformation_Player2Name_Label";
            playersInformation_Player2Name_Label.Size = new Size(48, 15);
            playersInformation_Player2Name_Label.TabIndex = 0;
            playersInformation_Player2Name_Label.Text = "Player 2";
            // 
            // playersInformation_Player1Name
            // 
            playersInformation_Player1Name.ForeColor = Color.Red;
            playersInformation_Player1Name.Location = new Point(63, 23);
            playersInformation_Player1Name.MaxLength = 20;
            playersInformation_Player1Name.Name = "playersInformation_Player1Name";
            playersInformation_Player1Name.Size = new Size(155, 23);
            playersInformation_Player1Name.TabIndex = 0;
            // 
            // playersInformation_Player1Name_Label
            // 
            playersInformation_Player1Name_Label.AutoSize = true;
            playersInformation_Player1Name_Label.ForeColor = Color.Red;
            playersInformation_Player1Name_Label.Location = new Point(5, 26);
            playersInformation_Player1Name_Label.Name = "playersInformation_Player1Name_Label";
            playersInformation_Player1Name_Label.Size = new Size(48, 15);
            playersInformation_Player1Name_Label.TabIndex = 0;
            playersInformation_Player1Name_Label.Text = "Player 1";
            // 
            // NewGameDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(413, 221);
            Controls.Add(playersInformationGroup);
            Controls.Add(gameTypeGroup);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewGameDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "New Game";
            FormClosed += NewGameDialog_FormClosed;
            gameTypeGroup.ResumeLayout(false);
            gameTypeGroup.PerformLayout();
            playersInformationGroup.ResumeLayout(false);
            playersInformationGroup.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox gameTypeGroup;
        private System.Windows.Forms.GroupBox playersInformationGroup;
        private System.Windows.Forms.CheckBox gameType_Handicap;
        private System.Windows.Forms.RadioButton gameType_VsAI;
        private System.Windows.Forms.RadioButton gameType_TwoPlayer;
        private System.Windows.Forms.TextBox playersInformation_Player2Name;
        private System.Windows.Forms.Label playersInformation_Player2Name_Label;
        private System.Windows.Forms.TextBox playersInformation_Player1Name;
        private System.Windows.Forms.Label playersInformation_Player1Name_Label;
        private System.Windows.Forms.RadioButton playersInformation_Player2AI;
        private System.Windows.Forms.RadioButton playersInformation_Player1AI;
        private ComboBox gameType_AIDifficulty;
    }
}