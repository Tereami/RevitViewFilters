namespace RevitViewFilters
{
    partial class FormSelectParameterForFilters
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
            this.comboBoxParameters = new System.Windows.Forms.ComboBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSymbols = new System.Windows.Forms.Label();
            this.numericStartSymbols = new System.Windows.Forms.NumericUpDown();
            this.radioButtonStartsWith = new System.Windows.Forms.RadioButton();
            this.radioButtonEquals = new System.Windows.Forms.RadioButton();
            this.radioButtonUserParameter = new System.Windows.Forms.RadioButton();
            this.radioButtonCheckHostMark = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartSymbols)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxParameters
            // 
            this.comboBoxParameters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParameters.FormattingEnabled = true;
            this.comboBoxParameters.Location = new System.Drawing.Point(6, 18);
            this.comboBoxParameters.Name = "comboBoxParameters";
            this.comboBoxParameters.Size = new System.Drawing.Size(221, 21);
            this.comboBoxParameters.TabIndex = 1;
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.Location = new System.Drawing.Point(183, 170);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "Далее>>";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(12, 170);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSymbols);
            this.groupBox1.Controls.Add(this.numericStartSymbols);
            this.groupBox1.Controls.Add(this.comboBoxParameters);
            this.groupBox1.Controls.Add(this.radioButtonStartsWith);
            this.groupBox1.Controls.Add(this.radioButtonEquals);
            this.groupBox1.Location = new System.Drawing.Point(23, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 96);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Критерий фильтрации";
            // 
            // labelSymbols
            // 
            this.labelSymbols.AutoSize = true;
            this.labelSymbols.Enabled = false;
            this.labelSymbols.Location = new System.Drawing.Point(125, 71);
            this.labelSymbols.Name = "labelSymbols";
            this.labelSymbols.Size = new System.Drawing.Size(57, 13);
            this.labelSymbols.TabIndex = 3;
            this.labelSymbols.Text = "символов";
            // 
            // numericStartSymbols
            // 
            this.numericStartSymbols.Enabled = false;
            this.numericStartSymbols.Location = new System.Drawing.Point(77, 69);
            this.numericStartSymbols.Name = "numericStartSymbols";
            this.numericStartSymbols.Size = new System.Drawing.Size(42, 20);
            this.numericStartSymbols.TabIndex = 2;
            this.numericStartSymbols.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // radioButtonStartsWith
            // 
            this.radioButtonStartsWith.AutoSize = true;
            this.radioButtonStartsWith.Location = new System.Drawing.Point(6, 69);
            this.radioButtonStartsWith.Name = "radioButtonStartsWith";
            this.radioButtonStartsWith.Size = new System.Drawing.Size(65, 17);
            this.radioButtonStartsWith.TabIndex = 1;
            this.radioButtonStartsWith.Text = "Первые";
            this.radioButtonStartsWith.UseVisualStyleBackColor = true;
            this.radioButtonStartsWith.CheckedChanged += new System.EventHandler(this.radioButtonStartsWith_CheckedChanged);
            // 
            // radioButtonEquals
            // 
            this.radioButtonEquals.AutoSize = true;
            this.radioButtonEquals.Checked = true;
            this.radioButtonEquals.Location = new System.Drawing.Point(6, 45);
            this.radioButtonEquals.Name = "radioButtonEquals";
            this.radioButtonEquals.Size = new System.Drawing.Size(94, 17);
            this.radioButtonEquals.TabIndex = 0;
            this.radioButtonEquals.TabStop = true;
            this.radioButtonEquals.Text = "Всё значение";
            this.radioButtonEquals.UseVisualStyleBackColor = true;
            this.radioButtonEquals.CheckedChanged += new System.EventHandler(this.radioButtonEquals_CheckedChanged);
            // 
            // radioButtonUserParameter
            // 
            this.radioButtonUserParameter.AutoSize = true;
            this.radioButtonUserParameter.Checked = true;
            this.radioButtonUserParameter.Location = new System.Drawing.Point(12, 12);
            this.radioButtonUserParameter.Name = "radioButtonUserParameter";
            this.radioButtonUserParameter.Size = new System.Drawing.Size(124, 17);
            this.radioButtonUserParameter.TabIndex = 2;
            this.radioButtonUserParameter.TabStop = true;
            this.radioButtonUserParameter.Text = "Выбрать параметр:";
            this.radioButtonUserParameter.UseVisualStyleBackColor = true;
            this.radioButtonUserParameter.CheckedChanged += new System.EventHandler(this.radioButtonUserParameter_CheckedChanged);
            // 
            // radioButtonCheckHostMark
            // 
            this.radioButtonCheckHostMark.AutoSize = true;
            this.radioButtonCheckHostMark.Location = new System.Drawing.Point(12, 137);
            this.radioButtonCheckHostMark.Name = "radioButtonCheckHostMark";
            this.radioButtonCheckHostMark.Size = new System.Drawing.Size(204, 17);
            this.radioButtonCheckHostMark.TabIndex = 5;
            this.radioButtonCheckHostMark.TabStop = true;
            this.radioButtonCheckHostMark.Text = "Проверка Метки основы арматуры";
            this.radioButtonCheckHostMark.UseVisualStyleBackColor = true;
            this.radioButtonCheckHostMark.CheckedChanged += new System.EventHandler(this.radioButtonCheckHostMark_CheckedChanged);
            // 
            // FormSelectParameterForFilters
            // 
            this.AcceptButton = this.buttonNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(269, 205);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radioButtonUserParameter);
            this.Controls.Add(this.radioButtonCheckHostMark);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSelectParameterForFilters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Шаг 1";
            this.Load += new System.EventHandler(this.FormSelectParameterForFilters_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartSymbols)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxParameters;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelSymbols;
        private System.Windows.Forms.NumericUpDown numericStartSymbols;
        private System.Windows.Forms.RadioButton radioButtonStartsWith;
        private System.Windows.Forms.RadioButton radioButtonEquals;
        private System.Windows.Forms.RadioButton radioButtonUserParameter;
        private System.Windows.Forms.RadioButton radioButtonCheckHostMark;
    }
}