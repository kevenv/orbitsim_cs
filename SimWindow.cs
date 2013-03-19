using System;
using System.Drawing;
using System.Windows.Forms;

namespace OrbitSim
{
    public partial class SimWindow : Form
    {
        private SimPanel simPanel;
        public const float VERSION = 0.1f;

        public SimWindow()
        {
            InitializeComponent();

            simPanel = new SimPanel(this.Width-groupBox1.Width-10, this.Height - menuStrip1.Height);
            simPanel.Location = new System.Drawing.Point(0, menuStrip1.Height);
            simPanel.Size = new System.Drawing.Size(this.Width - groupBox1.Width-10, this.Height - menuStrip1.Height);
            simPanel.BackColor = Color.Black;
            simPanel.Name = "panel1";
            simPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Controls.Add(simPanel);

            checkBox5.Checked = simPanel.drawSpeedInfo;
            checkBox4.Checked = SimPanel.space.collisionMode;
            checkBox3.Checked = SimPanel.space.realisticMode;
            checkBox2.Checked = simPanel.realScaleMode;
            textBox4.Text = "" + SimPanel.space.dt;
            textBox2.Text = "" + simPanel.scale;
            textBox1.Text = "" + simPanel.mass;
            textBox3.Text = "" + simPanel.radius;
            checkBox1.Checked = simPanel.star;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            simPanel.stop();
            simPanel.Dispose();
            Dispose();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            simPanel.drawSpeedInfo = checkBox5.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SimPanel.space.collisionMode = checkBox4.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SimPanel.space.clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SimPanel.space.reset();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            simPanel.recenter();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SimPanel.space.realisticMode = checkBox3.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            simPanel.realScaleMode = checkBox2.Checked;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            long dt;
            if(long.TryParse(textBox4.Text, out dt))
               SimPanel.space.dt = dt;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double scale;
            if (double.TryParse(textBox2.Text, out scale))
                simPanel.scale = scale;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double mass;
            if (double.TryParse(textBox1.Text, out mass))
                simPanel.mass = mass;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int radius;
            if (int.TryParse(textBox3.Text, out radius))
                simPanel.radius = radius;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            simPanel.star = checkBox1.Checked;
        }
    }
}
