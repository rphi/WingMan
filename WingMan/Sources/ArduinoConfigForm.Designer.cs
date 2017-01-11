namespace WingMan.Sources
{
    partial class ArduinoConfigForm
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
            this.faderMapDataGrid = new System.Windows.Forms.DataGridView();
            this.faderMapDataGridIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.faderMapDataGridTargetColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonMapDataGrid = new System.Windows.Forms.DataGridView();
            this.fadersGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonsGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.buttonMapDataGridIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonMapDataGridTypeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.buttonMapDataGridTargetColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonMapDataGridDataColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.faderMapDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonMapDataGrid)).BeginInit();
            this.fadersGroupBox.SuspendLayout();
            this.buttonsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // faderMapDataGrid
            // 
            this.faderMapDataGrid.AllowUserToAddRows = false;
            this.faderMapDataGrid.AllowUserToDeleteRows = false;
            this.faderMapDataGrid.AllowUserToResizeRows = false;
            this.faderMapDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.faderMapDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.faderMapDataGridIdColumn,
            this.faderMapDataGridTargetColumn});
            this.faderMapDataGrid.Location = new System.Drawing.Point(6, 19);
            this.faderMapDataGrid.Name = "faderMapDataGrid";
            this.faderMapDataGrid.Size = new System.Drawing.Size(486, 146);
            this.faderMapDataGrid.TabIndex = 0;
            // 
            // faderMapDataGridIdColumn
            // 
            this.faderMapDataGridIdColumn.HeaderText = "Fader ID";
            this.faderMapDataGridIdColumn.Name = "faderMapDataGridIdColumn";
            this.faderMapDataGridIdColumn.ReadOnly = true;
            this.faderMapDataGridIdColumn.Width = 50;
            // 
            // faderMapDataGridTargetColumn
            // 
            this.faderMapDataGridTargetColumn.HeaderText = "Target";
            this.faderMapDataGridTargetColumn.Name = "faderMapDataGridTargetColumn";
            this.faderMapDataGridTargetColumn.Width = 250;
            // 
            // buttonMapDataGrid
            // 
            this.buttonMapDataGrid.AllowUserToAddRows = false;
            this.buttonMapDataGrid.AllowUserToDeleteRows = false;
            this.buttonMapDataGrid.AllowUserToResizeRows = false;
            this.buttonMapDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.buttonMapDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.buttonMapDataGridIdColumn,
            this.buttonMapDataGridTypeColumn,
            this.buttonMapDataGridTargetColumn,
            this.buttonMapDataGridDataColumn});
            this.buttonMapDataGrid.Location = new System.Drawing.Point(6, 19);
            this.buttonMapDataGrid.Name = "buttonMapDataGrid";
            this.buttonMapDataGrid.Size = new System.Drawing.Size(486, 146);
            this.buttonMapDataGrid.TabIndex = 0;
            // 
            // fadersGroupBox
            // 
            this.fadersGroupBox.Controls.Add(this.faderMapDataGrid);
            this.fadersGroupBox.Location = new System.Drawing.Point(12, 29);
            this.fadersGroupBox.Name = "fadersGroupBox";
            this.fadersGroupBox.Size = new System.Drawing.Size(498, 171);
            this.fadersGroupBox.TabIndex = 1;
            this.fadersGroupBox.TabStop = false;
            this.fadersGroupBox.Text = "Fader Map";
            // 
            // buttonsGroupBox
            // 
            this.buttonsGroupBox.Controls.Add(this.buttonMapDataGrid);
            this.buttonsGroupBox.Location = new System.Drawing.Point(12, 206);
            this.buttonsGroupBox.Name = "buttonsGroupBox";
            this.buttonsGroupBox.Size = new System.Drawing.Size(498, 171);
            this.buttonsGroupBox.TabIndex = 2;
            this.buttonsGroupBox.TabStop = false;
            this.buttonsGroupBox.Text = "Button Map";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Configure mapping of inputs to OSC commands";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(12, 383);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(435, 383);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // buttonMapDataGridIdColumn
            // 
            this.buttonMapDataGridIdColumn.HeaderText = "Button ID";
            this.buttonMapDataGridIdColumn.Name = "buttonMapDataGridIdColumn";
            this.buttonMapDataGridIdColumn.ReadOnly = true;
            this.buttonMapDataGridIdColumn.Width = 50;
            // 
            // buttonMapDataGridTypeColumn
            // 
            this.buttonMapDataGridTypeColumn.HeaderText = "Type";
            this.buttonMapDataGridTypeColumn.Name = "buttonMapDataGridTypeColumn";
            // 
            // buttonMapDataGridTargetColumn
            // 
            this.buttonMapDataGridTargetColumn.HeaderText = "Target";
            this.buttonMapDataGridTargetColumn.Name = "buttonMapDataGridTargetColumn";
            this.buttonMapDataGridTargetColumn.Width = 180;
            // 
            // buttonMapDataGridDataColumn
            // 
            this.buttonMapDataGridDataColumn.HeaderText = "Data";
            this.buttonMapDataGridDataColumn.Name = "buttonMapDataGridDataColumn";
            this.buttonMapDataGridDataColumn.Width = 75;
            // 
            // ArduinoConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 414);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonsGroupBox);
            this.Controls.Add(this.fadersGroupBox);
            this.Name = "ArduinoConfigForm";
            this.Text = "Arduino Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.faderMapDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonMapDataGrid)).EndInit();
            this.fadersGroupBox.ResumeLayout(false);
            this.buttonsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView faderMapDataGrid;
        private System.Windows.Forms.DataGridView buttonMapDataGrid;
        private System.Windows.Forms.GroupBox fadersGroupBox;
        private System.Windows.Forms.GroupBox buttonsGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn faderMapDataGridIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn faderMapDataGridTargetColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buttonMapDataGridIdColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn buttonMapDataGridTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buttonMapDataGridTargetColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn buttonMapDataGridDataColumn;
    }
}