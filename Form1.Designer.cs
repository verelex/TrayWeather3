namespace TrayWeather3
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            label1 = new Label();
            label2 = new Label();
            textBoxCNM = new TextBox();
            textBox1 = new TextBox();
            label3 = new Label();
            textBoxRPH = new TextBox();
            label4 = new Label();
            label5 = new Label();
            comboBoxHosts = new ComboBox();
            button1 = new Button();
            checkBoxRegAutorun = new CheckBox();
            label6 = new Label();
            textBox2 = new TextBox();
            label7 = new Label();
            textBox3 = new TextBox();
            label8 = new Label();
            textBox4 = new TextBox();
            panel1 = new Panel();
            comboBoxIcoColors = new ComboBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            labelInfo = new Label();
            label10 = new Label();
            textBox5 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // webView21
            // 
            resources.ApplyResources(webView21, "webView21");
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Name = "webView21";
            webView21.ZoomFactor = 1D;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // textBoxCNM
            // 
            resources.ApplyResources(textBoxCNM, "textBoxCNM");
            textBoxCNM.Name = "textBoxCNM";
            textBoxCNM.TextChanged += textBoxCNM_TextChanged;
            // 
            // textBox1
            // 
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // textBoxRPH
            // 
            resources.ApplyResources(textBoxRPH, "textBoxRPH");
            textBoxRPH.Name = "textBoxRPH";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // comboBoxHosts
            // 
            resources.ApplyResources(comboBoxHosts, "comboBoxHosts");
            comboBoxHosts.FormattingEnabled = true;
            comboBoxHosts.Name = "comboBoxHosts";
            comboBoxHosts.SelectedIndexChanged += comboBoxHosts_SelectedIndexChanged;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // checkBoxRegAutorun
            // 
            resources.ApplyResources(checkBoxRegAutorun, "checkBoxRegAutorun");
            checkBoxRegAutorun.Name = "checkBoxRegAutorun";
            checkBoxRegAutorun.UseVisualStyleBackColor = true;
            checkBoxRegAutorun.CheckedChanged += checkBoxRegAutorun_CheckedChanged;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // textBox2
            // 
            resources.ApplyResources(textBox2, "textBox2");
            textBox2.Name = "textBox2";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // textBox3
            // 
            resources.ApplyResources(textBox3, "textBox3");
            textBox3.Name = "textBox3";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // textBox4
            // 
            resources.ApplyResources(textBox4, "textBox4");
            textBox4.Name = "textBox4";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(comboBoxIcoColors);
            panel1.Controls.Add(radioButton2);
            panel1.Controls.Add(radioButton1);
            panel1.Name = "panel1";
            // 
            // comboBoxIcoColors
            // 
            resources.ApplyResources(comboBoxIcoColors, "comboBoxIcoColors");
            comboBoxIcoColors.FormattingEnabled = true;
            comboBoxIcoColors.Name = "comboBoxIcoColors";
            // 
            // radioButton2
            // 
            resources.ApplyResources(radioButton2, "radioButton2");
            radioButton2.Name = "radioButton2";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            resources.ApplyResources(radioButton1, "radioButton1");
            radioButton1.Checked = true;
            radioButton1.Name = "radioButton1";
            radioButton1.TabStop = true;
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // labelInfo
            // 
            resources.ApplyResources(labelInfo, "labelInfo");
            labelInfo.Name = "labelInfo";
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // textBox5
            // 
            resources.ApplyResources(textBox5, "textBox5");
            textBox5.Name = "textBox5";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox5);
            Controls.Add(label10);
            Controls.Add(labelInfo);
            Controls.Add(panel1);
            Controls.Add(textBox4);
            Controls.Add(label8);
            Controls.Add(textBox3);
            Controls.Add(label7);
            Controls.Add(textBox2);
            Controls.Add(label6);
            Controls.Add(checkBoxRegAutorun);
            Controls.Add(button1);
            Controls.Add(comboBoxHosts);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(textBoxRPH);
            Controls.Add(label3);
            Controls.Add(textBox1);
            Controls.Add(textBoxCNM);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(webView21);
            Name = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Label label1;
        private Label label2;
        private TextBox textBoxCNM;
        private TextBox textBox1;
        private Label label3;
        private TextBox textBoxRPH;
        private Label label4;
        private Label label5;
        private ComboBox comboBoxHosts;
        private Button button1;
        private CheckBox checkBoxRegAutorun;
        private Label label6;
        private TextBox textBox2;
        private Label label7;
        private TextBox textBox3;
        private Label label8;
        private TextBox textBox4;
        private Panel panel1;
        private ComboBox comboBoxIcoColors;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Label labelInfo;
        private Label label10;
        private TextBox textBox5;
    }
}
