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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.gameTypeGroup = new System.Windows.Forms.GroupBox();
            this.gameType_Handicap = new System.Windows.Forms.CheckBox();
            this.gameType_VsAI = new System.Windows.Forms.RadioButton();
            this.gameType_TwoPlayer = new System.Windows.Forms.RadioButton();
            this.playersInformationGroup = new System.Windows.Forms.GroupBox();
            this.playersInformation_Player2AI = new System.Windows.Forms.RadioButton();
            this.playersInformation_Player1AI = new System.Windows.Forms.RadioButton();
            this.playersInformation_Player2Name = new System.Windows.Forms.TextBox();
            this.playersInformation_Player2Name_Label = new System.Windows.Forms.Label();
            this.playersInformation_Player1Name = new System.Windows.Forms.TextBox();
            this.playersInformation_Player1Name_Label = new System.Windows.Forms.Label();
            this.gameTypeGroup.SuspendLayout();
            this.playersInformationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(304, 171);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 30);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(385, 171);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // gameTypeGroup
            // 
            this.gameTypeGroup.Controls.Add(this.gameType_Handicap);
            this.gameTypeGroup.Controls.Add(this.gameType_VsAI);
            this.gameTypeGroup.Controls.Add(this.gameType_TwoPlayer);
            this.gameTypeGroup.Location = new System.Drawing.Point(12, 12);
            this.gameTypeGroup.Name = "gameTypeGroup";
            this.gameTypeGroup.Size = new System.Drawing.Size(140, 153);
            this.gameTypeGroup.TabIndex = 0;
            this.gameTypeGroup.TabStop = false;
            this.gameTypeGroup.Text = "Game type";
            // 
            // gameType_Handicap
            // 
            this.gameType_Handicap.AutoSize = true;
            this.gameType_Handicap.Location = new System.Drawing.Point(6, 115);
            this.gameType_Handicap.Name = "gameType_Handicap";
            this.gameType_Handicap.Size = new System.Drawing.Size(90, 21);
            this.gameType_Handicap.TabIndex = 2;
            this.gameType_Handicap.Text = "Handicap";
            this.gameType_Handicap.UseVisualStyleBackColor = true;
            // 
            // gameType_VsAI
            // 
            this.gameType_VsAI.AutoSize = true;
            this.gameType_VsAI.Location = new System.Drawing.Point(6, 57);
            this.gameType_VsAI.Name = "gameType_VsAI";
            this.gameType_VsAI.Size = new System.Drawing.Size(59, 21);
            this.gameType_VsAI.TabIndex = 1;
            this.gameType_VsAI.Text = "vs AI";
            this.gameType_VsAI.UseVisualStyleBackColor = true;
            // 
            // gameType_TwoPlayer
            // 
            this.gameType_TwoPlayer.AutoSize = true;
            this.gameType_TwoPlayer.Checked = true;
            this.gameType_TwoPlayer.Location = new System.Drawing.Point(6, 29);
            this.gameType_TwoPlayer.Name = "gameType_TwoPlayer";
            this.gameType_TwoPlayer.Size = new System.Drawing.Size(87, 21);
            this.gameType_TwoPlayer.TabIndex = 0;
            this.gameType_TwoPlayer.TabStop = true;
            this.gameType_TwoPlayer.Text = "2 players";
            this.gameType_TwoPlayer.UseVisualStyleBackColor = true;
            this.gameType_TwoPlayer.CheckedChanged += new System.EventHandler(this.gameType_CheckedChanged);
            // 
            // playersInformationGroup
            // 
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player2AI);
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player1AI);
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player2Name);
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player2Name_Label);
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player1Name);
            this.playersInformationGroup.Controls.Add(this.playersInformation_Player1Name_Label);
            this.playersInformationGroup.Location = new System.Drawing.Point(158, 15);
            this.playersInformationGroup.Name = "playersInformationGroup";
            this.playersInformationGroup.Size = new System.Drawing.Size(302, 150);
            this.playersInformationGroup.TabIndex = 1;
            this.playersInformationGroup.TabStop = false;
            this.playersInformationGroup.Text = "Players information";
            // 
            // playersInformation_Player2AI
            // 
            this.playersInformation_Player2AI.AutoSize = true;
            this.playersInformation_Player2AI.Checked = true;
            this.playersInformation_Player2AI.Enabled = false;
            this.playersInformation_Player2AI.ForeColor = System.Drawing.Color.Blue;
            this.playersInformation_Player2AI.Location = new System.Drawing.Point(255, 54);
            this.playersInformation_Player2AI.Name = "playersInformation_Player2AI";
            this.playersInformation_Player2AI.Size = new System.Drawing.Size(41, 21);
            this.playersInformation_Player2AI.TabIndex = 3;
            this.playersInformation_Player2AI.TabStop = true;
            this.playersInformation_Player2AI.Text = "AI";
            this.playersInformation_Player2AI.UseVisualStyleBackColor = true;
            // 
            // playersInformation_Player1AI
            // 
            this.playersInformation_Player1AI.AutoSize = true;
            this.playersInformation_Player1AI.Enabled = false;
            this.playersInformation_Player1AI.ForeColor = System.Drawing.Color.Red;
            this.playersInformation_Player1AI.Location = new System.Drawing.Point(255, 26);
            this.playersInformation_Player1AI.Name = "playersInformation_Player1AI";
            this.playersInformation_Player1AI.Size = new System.Drawing.Size(41, 21);
            this.playersInformation_Player1AI.TabIndex = 1;
            this.playersInformation_Player1AI.Text = "AI";
            this.playersInformation_Player1AI.UseVisualStyleBackColor = true;
            // 
            // playersInformation_Player2Name
            // 
            this.playersInformation_Player2Name.ForeColor = System.Drawing.Color.Blue;
            this.playersInformation_Player2Name.Location = new System.Drawing.Point(72, 53);
            this.playersInformation_Player2Name.MaxLength = 20;
            this.playersInformation_Player2Name.Name = "playersInformation_Player2Name";
            this.playersInformation_Player2Name.Size = new System.Drawing.Size(177, 22);
            this.playersInformation_Player2Name.TabIndex = 2;
            // 
            // playersInformation_Player2Name_Label
            // 
            this.playersInformation_Player2Name_Label.AutoSize = true;
            this.playersInformation_Player2Name_Label.ForeColor = System.Drawing.Color.Blue;
            this.playersInformation_Player2Name_Label.Location = new System.Drawing.Point(6, 56);
            this.playersInformation_Player2Name_Label.Name = "playersInformation_Player2Name_Label";
            this.playersInformation_Player2Name_Label.Size = new System.Drawing.Size(60, 17);
            this.playersInformation_Player2Name_Label.TabIndex = 0;
            this.playersInformation_Player2Name_Label.Text = "Player 2";
            // 
            // playersInformation_Player1Name
            // 
            this.playersInformation_Player1Name.ForeColor = System.Drawing.Color.Red;
            this.playersInformation_Player1Name.Location = new System.Drawing.Point(72, 25);
            this.playersInformation_Player1Name.MaxLength = 20;
            this.playersInformation_Player1Name.Name = "playersInformation_Player1Name";
            this.playersInformation_Player1Name.Size = new System.Drawing.Size(177, 22);
            this.playersInformation_Player1Name.TabIndex = 0;
            // 
            // playersInformation_Player1Name_Label
            // 
            this.playersInformation_Player1Name_Label.AutoSize = true;
            this.playersInformation_Player1Name_Label.ForeColor = System.Drawing.Color.Red;
            this.playersInformation_Player1Name_Label.Location = new System.Drawing.Point(6, 28);
            this.playersInformation_Player1Name_Label.Name = "playersInformation_Player1Name_Label";
            this.playersInformation_Player1Name_Label.Size = new System.Drawing.Size(60, 17);
            this.playersInformation_Player1Name_Label.TabIndex = 0;
            this.playersInformation_Player1Name_Label.Text = "Player 1";
            // 
            // NewGameDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(472, 213);
            this.Controls.Add(this.playersInformationGroup);
            this.Controls.Add(this.gameTypeGroup);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewGameDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Game";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewGameDialog_FormClosed);
            this.gameTypeGroup.ResumeLayout(false);
            this.gameTypeGroup.PerformLayout();
            this.playersInformationGroup.ResumeLayout(false);
            this.playersInformationGroup.PerformLayout();
            this.ResumeLayout(false);

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
    }
}