namespace Audio2MinecraftScore
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.GO = new System.Windows.Forms.Button();
            this.MIDI = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.WAV = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // GO
            // 
            this.GO.Location = new System.Drawing.Point(38, 23);
            this.GO.Name = "GO";
            this.GO.Size = new System.Drawing.Size(77, 26);
            this.GO.TabIndex = 0;
            this.GO.Text = "GO";
            this.GO.UseVisualStyleBackColor = true;
            this.GO.Click += new System.EventHandler(this.GO_Click);
            // 
            // MIDI
            // 
            this.MIDI.Location = new System.Drawing.Point(285, 23);
            this.MIDI.Name = "MIDI";
            this.MIDI.Size = new System.Drawing.Size(247, 21);
            this.MIDI.TabIndex = 1;
            this.MIDI.Click += new System.EventHandler(this.MIDI_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(38, 111);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(322, 198);
            this.textBox1.TabIndex = 2;
            // 
            // WAV
            // 
            this.WAV.Location = new System.Drawing.Point(285, 50);
            this.WAV.Name = "WAV";
            this.WAV.Size = new System.Drawing.Size(247, 21);
            this.WAV.TabIndex = 3;
            this.WAV.Click += new System.EventHandler(this.WAV_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 321);
            this.Controls.Add(this.WAV);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.MIDI);
            this.Controls.Add(this.GO);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GO;
        private System.Windows.Forms.TextBox MIDI;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox WAV;
    }
}

