namespace ChineseChess
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            boardPanel = new Panel();
            newGameButton = new Button();
            undoButton = new Button();
            controlGroup = new GroupBox();
            statusGroup = new GroupBox();
            statusLabel = new Label();
            startButton = new Button();
            turnLabel = new Label();
            currentPlayerLabel = new Label();
            controlGroup.SuspendLayout();
            statusGroup.SuspendLayout();
            SuspendLayout();
            // 
            // boardPanel
            // 
            boardPanel.BackgroundImage = Properties.Resources.Board;
            boardPanel.BackgroundImageLayout = ImageLayout.Stretch;
            boardPanel.Location = new Point(12, 12);
            boardPanel.Name = "boardPanel";
            boardPanel.Size = new Size(486, 449);
            boardPanel.TabIndex = 0;
            // 
            // newGameButton
            // 
            newGameButton.Location = new Point(17, 29);
            newGameButton.Name = "newGameButton";
            newGameButton.Size = new Size(100, 28);
            newGameButton.TabIndex = 0;
            newGameButton.Text = "New game";
            newGameButton.UseVisualStyleBackColor = true;
            newGameButton.Click += NewGameButton_Click;
            // 
            // undoButton
            // 
            undoButton.Enabled = false;
            undoButton.Location = new Point(123, 29);
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(100, 28);
            undoButton.TabIndex = 1;
            undoButton.Text = "Undo";
            undoButton.UseVisualStyleBackColor = true;
            undoButton.Click += UndoButton_Click;
            // 
            // controlGroup
            // 
            controlGroup.Controls.Add(undoButton);
            controlGroup.Controls.Add(newGameButton);
            controlGroup.Location = new Point(504, 12);
            controlGroup.Name = "controlGroup";
            controlGroup.Size = new Size(236, 75);
            controlGroup.TabIndex = 1;
            controlGroup.TabStop = false;
            controlGroup.Text = "Control";
            // 
            // statusGroup
            // 
            statusGroup.Controls.Add(statusLabel);
            statusGroup.Controls.Add(startButton);
            statusGroup.Controls.Add(turnLabel);
            statusGroup.Controls.Add(currentPlayerLabel);
            statusGroup.Location = new Point(504, 93);
            statusGroup.Name = "statusGroup";
            statusGroup.Size = new Size(236, 141);
            statusGroup.TabIndex = 2;
            statusGroup.TabStop = false;
            statusGroup.Text = "Status";
            // 
            // statusLabel
            // 
            statusLabel.Location = new Point(5, 61);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(193, 38);
            statusLabel.TabIndex = 0;
            // 
            // startButton
            // 
            startButton.Location = new Point(17, 102);
            startButton.Name = "startButton";
            startButton.Size = new Size(100, 28);
            startButton.TabIndex = 0;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = true;
            startButton.Visible = false;
            startButton.Click += StartButton_Click;
            // 
            // turnLabel
            // 
            turnLabel.AutoSize = true;
            turnLabel.Location = new Point(5, 27);
            turnLabel.Name = "turnLabel";
            turnLabel.Size = new Size(34, 15);
            turnLabel.TabIndex = 0;
            turnLabel.Text = "Turn:";
            // 
            // currentPlayerLabel
            // 
            currentPlayerLabel.AutoSize = true;
            currentPlayerLabel.ForeColor = Color.Red;
            currentPlayerLabel.Location = new Point(47, 27);
            currentPlayerLabel.Name = "currentPlayerLabel";
            currentPlayerLabel.Size = new Size(0, 15);
            currentPlayerLabel.TabIndex = 0;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(752, 473);
            Controls.Add(statusGroup);
            Controls.Add(controlGroup);
            Controls.Add(boardPanel);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Chiness Chess";
            Load += Main_Load;
            controlGroup.ResumeLayout(false);
            statusGroup.ResumeLayout(false);
            statusGroup.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button newGameButton;
        private Button undoButton;
        private Panel boardPanel;
        private GroupBox controlGroup;
        private GroupBox statusGroup;
        private Label currentPlayerLabel;
        private Label statusLabel;
        private Button startButton;
        private Label turnLabel;
    }
}

