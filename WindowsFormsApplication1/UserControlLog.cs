using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApplication1
{
    public partial class UserControlLog : UserControl
    {
        private SerialPort port;
        private PMS pms;

        public UserControlLog(SerialPort port)
        {
            InitializeComponent();
            this.port = port;
            this.pms = new PMS(this.port);
            this.pms.DataEvent += Pms_DataEvent;
            this.pms.FeedbackEvent += Pms_RawDataEvent;

            try
            {
                Font font = new Font(FontFamily.GenericMonospace, 7.5f);
                textBoxLog.Font = font;
            }
            catch { }
        }

        private void Pms_RawDataEvent(object sender, PMS.RawDataEventArgs e)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                textBoxLog.AppendText(e.Data + Environment.NewLine);
            }));
        }

        private void Pms_DataEvent(object sender, PMS.DataEventArgs e)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                textBoxCF_1_0.Text = e.PM_FE_UG_1_0.ToString();
                textBoxCF_2_5.Text = e.PM_FE_UG_2_5.ToString();
                textBoxCF_10_0.Text = e.PM_FE_UG_10_0.ToString();

                textBox1_0.Text = e.PM_AE_UG_1_0.ToString();
                textBox2_5.Text = e.PM_AE_UG_2_5.ToString();
                textBox10_0.Text = e.PM_AE_UG_10_0.ToString();
            }));
        }

        private void checkBoxActiveMode_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                pms.ActiveMode();
                buttonRead.Enabled = false;
            }
            else
            {
                pms.PassiveMode();
                buttonRead.Enabled = true;
            }
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            pms.Read();
        }

        private void checkBoxSleepMode_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                pms.Sleep();
            }
            else
            {
                pms.WakeUp();
            }
        }

        private void timerReport_Tick(object sender, EventArgs e)
        {
            if (checkBoxReport.Checked)
            {
                textBoxLog.AppendText("Lewei:"
                                      + Report.SensorUpdate(textBox2_5.Text,
                                          UserControlSettings.GatewayId,
                                          UserControlSettings.UserKey)
                                      + Environment.NewLine);
            }
        }

        private void timerWake_Tick(object sender, EventArgs e)
        {
            checkBoxSleepMode.Checked = false;
            timerCollectSleep.Enabled = true;
            progressBar.Value = 3600000;
        }

        private void timerCollectSleep_Tick(object sender, EventArgs e)
        {
            checkBoxSleepMode.Checked = true;
            timerCollectSleep.Enabled = false;
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            progressBar.Value -= 100;
        }

        private void AutoSleep_CheckedChanged(object sender, EventArgs e)
        {
            timerWake.Enabled = checkBoxAutoSleep.Checked;
            timerUI.Enabled = checkBoxAutoSleep.Checked;
        }
    }
}
