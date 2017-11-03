using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace MagneticSuspension
{
    public partial class mainForm : Form
    {
        serialPortClass serialPortC;
        SerialPort serialPort = new SerialPort();  //
        byte[] sendBuf = new byte[6] { 0x0, 0x0, 0x0, 0x0, 0xAA, 0xAA };
        byte[] rcvBuf = new byte[42];
        bool sendEnable = true;
        bool motorIsEnable = false;
        bool magneticIsEnable = false;
        bool isWriteText = false;
        bool isShowI = false;
        double motorI, motorSumI;
        double motorV, motorSumV;
        int Count = 0, k;
        U_sendData sendData = new U_sendData();

        polarForm pForm = new polarForm();


       bool isBJControl = false;
 
       uint motorDevIndex = 0;
       uint motorCANIndex = 0;

       uint magnDevIndex = 0;
       uint magnCANIndex = 0;
       byte sendMagnHead=  0x0;
       byte sendMagnDataH = 0x0;
       byte sendMagnDataL = 0x0;



        private float X;
        private float Y;


        public mainForm()
        {
            

            InitializeComponent();
            serialPortC = new serialPortClass(serialPort);

            for (int j = 0; j < serialPortC.getCount(); j++)
            {
                comboBoxCom.Items.Add(serialPortC.getPorts()[j]);
            }

            this.Resize += new EventHandler(mianForm_Resize);

            X = this.Width;
            Y = this.Height;


            setTag(this);
            mianForm_Resize(new object(), new EventArgs());//x,y可在实例化时赋值,最后这句是新加的，在MDI时有用


        }




        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }

        }

        void mianForm_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
           // this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }





        /// <summary>
        /// 双击关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            serialPort.Close();
            Application.Exit();
        }

        private void comboBoxCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                serialPort.PortName = comboBoxCom.Text;
                serialPortC.Open(serialPort);

                if (serialPort.IsOpen)
                {
                    pictureBoxPort.BackColor = Color.Green;
                    magneticIsEnable = true;
                    writeText(serialPort.PortName + " Open");
                }
                else
                {
                    pictureBoxPort.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxPort_Click(object sender, EventArgs e)
        {
            magneticIsEnable = false;
            serialPort.Close();
            pictureBoxPort.BackColor = Color.Red;
            writeText(serialPort.PortName + " Closed");
        }



        /// <summary>
        /// 总控制模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode.SelectedIndex.ToString()))
                return;
            SendParam(0x50, comboBoxMode.SelectedIndex);
            writeText("Mode=" + comboBoxMode.Text);
        }

        /// <summary>
        /// KP设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void numericUpDownKP1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKP1.Value.ToString()))
                return;
            SendParam(0x01, numericUpDownKP1.Value);
            writeText("KP1=" + numericUpDownKP1.Value);
        }


        private void numericUpDownKP2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKP2.Value.ToString()))
                return;
            SendParam(0x02, numericUpDownKP2.Value);
            writeText("KP2=" + numericUpDownKP2.Value);
        }

        private void numericUpDownKP3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKP3.Value.ToString()))
                return;
            SendParam(0x03, numericUpDownKP3.Value);
            writeText("KP3=" + numericUpDownKP3.Value);
        }

        private void numericUpDownKP4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKP4.Value.ToString()))
                return;
            SendParam(0x04, numericUpDownKP4.Value);
            writeText("KP4=" + numericUpDownKP4.Value);
        }

        private void numericUpDownKP5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKP5.Value.ToString()))
                return;
            SendParam(0x05, numericUpDownKP5.Value);
            writeText("KP5=" + numericUpDownKP5.Value);
        }





        /// <summary>
        /// KD设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void numericUpDownKD1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKD1.Value.ToString()))
                return;
            if(isBJControl)
                SendParam(0x07, numericUpDownKD1.Value*10);
            else
                SendParam(0x07, numericUpDownKD1.Value);
            writeText("KD1=" + numericUpDownKD1.Value);
        }
        private void numericUpDownKD2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKD2.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x08, numericUpDownKD2.Value * 10);
            else
                SendParam(0x08, numericUpDownKD2.Value);
            writeText("KD2=" + numericUpDownKD2.Value);
        }

        private void numericUpDownKD3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKD3.ToString()))
                return;
            if (isBJControl)
                SendParam(0x09, numericUpDownKD3.Value * 10);
            else
                SendParam(0x09, numericUpDownKD3.Value);
            writeText("KD3=" + numericUpDownKD3.Value);
        }

        private void numericUpDownKD4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKD4.ToString()))
                return;
            if (isBJControl)
                SendParam(0x0A, numericUpDownKD4.Value * 10);
            else
                SendParam(0x0A, numericUpDownKD4.Value);
            writeText("KD4=" + numericUpDownKD4.Value);
        }


        private void numericUpDownKD5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKD5.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x0B, numericUpDownKD5.Value * 10);
            else
                SendParam(0x0B, numericUpDownKD5.Value);
            writeText("KD5=" + numericUpDownKD5.Value);
        }




        /// <summary>
        /// KI设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownKI1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKI1.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0xD, numericUpDownKI1.Value * 100);
            else
                SendParam(0xD, numericUpDownKI1.Value / 100);
            writeText("KI1=" + numericUpDownKI1.Value);
        }

        private void numericUpDownKI2_ValueChanged(object sender, EventArgs e)
        {

            if (!ValidityCheck(numericUpDownKI2.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0xE, numericUpDownKI2.Value * 100);
            else
                SendParam(0xE, numericUpDownKI2.Value / 100);
            writeText("KI2=" + numericUpDownKI2.Value);
        }

        private void numericUpDownKI3_ValueChanged(object sender, EventArgs e)
        {

            if (!ValidityCheck(numericUpDownKI3.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0xF, numericUpDownKI3.Value * 100);
            else
                SendParam(0xF, numericUpDownKI3.Value / 100);
            writeText("KI3=" + numericUpDownKI3.Value);
        }

        private void numericUpDownKI4_ValueChanged(object sender, EventArgs e)
        {

            if (!ValidityCheck(numericUpDownKI4.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x10, numericUpDownKI4.Value * 100);
            else
                SendParam(0x10, numericUpDownKI4.Value / 100);
            writeText("KI4=" + numericUpDownKI4.Value);
        }
        private void numericUpDownKI5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownKI5.Value.ToString()))
                return;
            SendParam(0x11, numericUpDownKI5.Value / 100);
            writeText("KI5=" + numericUpDownKI5.Value);
        }




        /// <summary>
        /// 设置DAT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownDTA1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownDTA1.Value.ToString()))
                return;
            SendParam(0x13, numericUpDownDTA1.Value);
            writeText("DTA1=" + numericUpDownDTA1.Value);
        }

        private void numericUpDownDTA2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownDTA2.Value.ToString()))
                return;
            SendParam(0x14, numericUpDownDTA2.Value);
            writeText("DTA2=" + numericUpDownDTA2.Value);
        }

        private void numericUpDownDTA3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownDTA3.Value.ToString()))
                return;
            SendParam(0x15, numericUpDownDTA3.Value);
            writeText("DTA3=" + numericUpDownDTA3.Value);
        }

        private void numericUpDownDTA4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownDTA4.Value.ToString()))
                return;
            SendParam(0x16, numericUpDownDTA4.Value);
            writeText("DTA4=" + numericUpDownDTA4.Value);
        }
        private void numericUpDownDTA5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownDTA5.Value.ToString()))
                return;
            SendParam(0x17, numericUpDownDTA5.Value);
            writeText("DTA5=" + numericUpDownDTA5.Value);
        }




        /// <summary>
        /// SGM设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownSGM1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownSGM1.Value.ToString()))
                return;

            if(isBJControl)
                SendParam(0x19, numericUpDownSGM1.Value+1);
            else
                SendParam(0x19, numericUpDownSGM1.Value);
            writeText("SGM1=" + numericUpDownSGM1.Value);
        }

        private void numericUpDownSGM2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownSGM2.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x1A, numericUpDownSGM2.Value + 1);
            else
                SendParam(0x1A, numericUpDownSGM2.Value);
            writeText("SGM2=" + numericUpDownSGM2.Value);
        }

        private void numericUpDownSGM3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownSGM3.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x1B, numericUpDownSGM3.Value + 1);
            else
                SendParam(0x1B, numericUpDownSGM3.Value);
            writeText("SGM3=" + numericUpDownSGM3.Value);
        }

        private void numericUpDownSGM4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownSGM4.Value.ToString()))
                return;
            if (isBJControl)
                SendParam(0x1C, numericUpDownSGM4.Value + 1);
            else
                SendParam(0x1C, numericUpDownSGM4.Value);
            writeText("SGM4=" + numericUpDownSGM4.Value);
        }

        private void numericUpDownSGM5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownSGM5.Value.ToString()))
                return;
            SendParam(0x1D, numericUpDownSGM5.Value);
            writeText("SGM5=" + numericUpDownSGM5.Value);
        }




        /// <summary>
        /// PI设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownPI1_ValueChanged(object sender, EventArgs e)
        {
            
            if (!ValidityCheck(numericUpDownPI1.Value.ToString()))
                return;
            SendParam(0x21, numericUpDownPI1.Value);
            writeText("PI1=" + numericUpDownPI1.Value);
        }
        private void numericUpDownPI2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownPI2.Value.ToString()))
                return;
            SendParam(0x22, numericUpDownPI2.Value);
            writeText("PI2=" + numericUpDownPI2.Value);
        }

        private void numericUpDownPI3_ValueChanged(object sender, EventArgs e)
        {

            if (!ValidityCheck(numericUpDownPI3.Value.ToString()))
                return;
            SendParam(0x23, numericUpDownPI3.Value);
            writeText("PI3=" + numericUpDownPI3.Value);
        }

        private void numericUpDownPI4_ValueChanged(object sender, EventArgs e)
        {

            if (!ValidityCheck(numericUpDownPI4.Value.ToString()))
                return;
            SendParam(0x24, numericUpDownPI4.Value);
            writeText("PI4=" + numericUpDownPI4.Value);
        }
        private void numericUpDownPI5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownPI5.Value.ToString()))
                return;
            SendParam(0x25, numericUpDownPI5.Value);
            writeText("PI5=" + numericUpDownPI5.Value);
        }



        /// <summary>
        /// MODE设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMode1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode1.SelectedIndex.ToString()))
                return;
            SendParam(0x2D, comboBoxMode1.SelectedIndex);
            writeText("Mode1=" + comboBoxMode1.Text);
        }
        private void comboBoxMode2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode2.SelectedIndex.ToString()))
                return;
            SendParam(0x2E, comboBoxMode2.SelectedIndex);
            writeText("Mode2=" + comboBoxMode2.Text);
        }

        private void comboBoxMode3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode3.SelectedIndex.ToString()))
                return;
            SendParam(0x2F, comboBoxMode3.SelectedIndex);
            writeText("Mode3=" + comboBoxMode3.Text);
        }

        private void comboBoxMode4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode4.SelectedIndex.ToString()))
                return;
            SendParam(0x30, comboBoxMode4.SelectedIndex);
            writeText("Mode4=" + comboBoxMode4.Text);
        }
        private void comboBoxMode5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(comboBoxMode5.SelectedIndex.ToString()))
                return;
            SendParam(0x31, comboBoxMode5.SelectedIndex);
            writeText("Mode5=" + comboBoxMode5.Text);
        }












        /// <summary>
        /// Base设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownBS1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownBS1.Value.ToString()))
                return;
            SendParam(0x3A, numericUpDownBS1.Value/100+10);
            writeText("BS1=" + numericUpDownBS1.Value);
        }

        private void numericUpDownBS2_ValueChanged_1(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownBS2.Value.ToString()))
                return;
            SendParam(0x3B, numericUpDownBS2.Value/1000);
            writeText("BS2=" + numericUpDownBS2.Value);
        }
        private void numericUpDownBS3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownBS3.Value.ToString()))
                return;
            SendParam(0x3C, numericUpDownBS3.Value/1000);
            writeText("BS3=" + numericUpDownBS3.Value);
        }

        private void numericUpDownBS4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownBS4.Value.ToString()))
                return;
            SendParam(0x3D, numericUpDownBS4.Value/1000);
            writeText("BS4=" + numericUpDownBS4.Value);
        }
        private void numericUpDownBS5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownBS5.Value.ToString()))
                return;
            SendParam(0x3E, numericUpDownBS5.Value);
            writeText("BS5=" + numericUpDownBS5.Value);
        }

        /// <summary>
        /// 交叉PID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDowncKP1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKP1.Value.ToString()))
                return;
            SendParam(0x51, numericUpDowncKP1.Value);
            writeText("cKP1=" + numericUpDowncKP1.Value);
        }

        private void numericUpDowncKp2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKp2.Value.ToString()))
                return;
            SendParam(0x52, numericUpDowncKp2.Value);
            writeText("cKP2=" + numericUpDowncKp2.Value);
        }

        private void numericUpDowncKP3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKP3.Value.ToString()))
                return;
            SendParam(0x53, numericUpDowncKP3.Value);
            writeText("cKP3=" + numericUpDowncKP3.Value);
        }


        private void numericUpDowncKP4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKP4.Value.ToString()))
                return;
            SendParam(0x54, numericUpDowncKP4.Value);
            writeText("cKP4=" + numericUpDowncKP4.Value);
        }
        private void numericUpDowncKP5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKP5.Value.ToString()))
                return;
            SendParam(0x55, numericUpDowncKP5.Value);
            writeText("cKP5=" + numericUpDowncKP5.Value);
        }



        private void numericUpDowncKD1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKD1.Value.ToString()))
                return;
            SendParam(0x57, numericUpDowncKD1.Value);
            writeText("cKD1=" + numericUpDowncKD1.Value);
        }
        private void numericUpDowncKD2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKD2.Value.ToString()))
                return;
            SendParam(0x58, numericUpDowncKD2.Value);
            writeText("cKD2=" + numericUpDowncKD2.Value);
        }

        private void numericUpDowncKD3_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKD3.Value.ToString()))
                return;
            SendParam(0x59, numericUpDowncKD3.Value);
            writeText("cKD3=" + numericUpDowncKD3.Value);
        }

        private void numericUpDowncKD4_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKD4.Value.ToString()))
                return;
            SendParam(0x5A, numericUpDowncKD4.Value);
            writeText("cKD4=" + numericUpDowncKD4.Value);
        }

        private void numericUpDowncKD5_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDowncKD5.Value.ToString()))
                return;
            SendParam(0x5B, numericUpDowncKD5.Value);
            writeText("cKD5=" + numericUpDowncKD5.Value);
        }




        /// <summary>
        /// 陷波器设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDownLA_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownLA.Value.ToString()))
                return;
            SendParam(0x60, numericUpDownLA.Value);
            writeText("A=" + numericUpDownLA.Value);
        }
        private void numericUpDownLseta1_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownLseta1.Value.ToString()))
                return;
            SendParam(0x61, numericUpDownLseta1.Value);
            writeText("θ1=" + numericUpDownLseta1.Value);
        }

        private void numericUpDownLB_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownLB.Value.ToString()))
                return;
            SendParam(0x62, numericUpDownLB.Value);
            writeText("B=" + numericUpDownLB.Value);
        }

        private void numericUpDownLseta2_ValueChanged(object sender, EventArgs e)
        {
            if (!ValidityCheck(numericUpDownLseta2.Value.ToString()))
                return;
            SendParam(0x63, numericUpDownLseta2.Value);
            writeText("θ2=" + numericUpDownLseta2.Value);
        }

        private void checkBox_notch_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_notch.Checked)
            {
                SendParam(0x64, 1);
                writeText("启动陷波器");
            }
            else
            {
                SendParam(0x64, 0);
                writeText("关闭陷波器");
            }
        }



        /// <summary>
        /// 模式显示设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void labelSIGMA5_Click(object sender, EventArgs e)
        {
            groupBox5.Visible = true;
            groupBox7.Visible = false;
        }

        private void labelSIGMA4_Click(object sender, EventArgs e)
        {
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }

        private void TSM_MonitorW4_Click(object sender, EventArgs e)
        {
            isShowI = false;
            groupBox5.Visible = true;
            groupBox7.Visible = false;
            SendParam(0x48, 1);

        }

        private void TSM_MonitorW6_Click(object sender, EventArgs e)
        {
            isShowI = false;
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }

        private void TSM_MonitorI4_Click(object sender, EventArgs e)
        {
            isShowI = true;
            groupBox5.Visible = true;
            groupBox7.Visible = false;
            SendParam(0x48, 0);
        }

        private void TSM_MonitorI6_Click(object sender, EventArgs e)
        {
            isShowI = true;
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }




        /// <summary>
        /// 记录数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        private void TSM_StartRecordData_Click(object sender, EventArgs e)
        {
            isWriteText = true;
        }

        private void TSM_EndRecordData_Click(object sender, EventArgs e)
        {
            isWriteText = false;
        }
        private void TSM_MonitorTrail_Click(object sender, EventArgs e)
        {
            if (pForm.IsDisposed)
            {
                pForm = new polarForm();
            }         
                pForm.Show();
 
        }



        /// <summary>
        /// 电机设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void buttonOpenCAN_Click(object sender, EventArgs e)
        {
            if (buttonOpenCAN.Text == "打开")
            {
                if (CANClass.initCAN(motorDevIndex,motorCANIndex) == 0)
                {
                    buttonOpenCAN.Text = "关闭";
                    buttonOpenCAN.ForeColor = Color.Green;
                    motorIsEnable = true;
                }
            }
            else
            {
                CANClass.closeCAN(motorDevIndex);
                motorIsEnable = false;
                buttonOpenCAN.Text = "打开";
                buttonOpenCAN.ForeColor = Color.Red;
            }
        }
        private void radioButtonOpenLoop_Click(object sender, EventArgs e)
        {
            if (1 == CANClass.sendMotorBuf(motorDevIndex, motorCANIndex, 0x1B, 0, 0))
            {
                writeText("开环设定成功");
                labelDuty.Text = "占空比";
            }
            else
            {
                writeText("开环设定失败");

            }
        }

        private void radioButtonCloseLoop_Click(object sender, EventArgs e)
        {


            if (1 == CANClass.sendMotorBuf(motorDevIndex, motorCANIndex, 0x1A, 0, 0))
            {
                writeText("闭环设定成功");
                labelDuty.Text = "速度";
            }
            else
            {
                writeText("闭环设定失败");
            }
        }


        private void numericUpDownLDT_ValueChanged(object sender, EventArgs e)
        {

            UInt32 iref_int = Convert.ToUInt32(numericUpDownLDT.Value);
            if (radioButtonCloseLoop.Checked)
                iref_int += 5000;

            byte iref_int88 = (byte)((iref_int) / 256);
            byte iref_int77 = (byte)(iref_int - iref_int88 * 256);

            if (1 == CANClass.sendMotorBuf(motorDevIndex,motorCANIndex, 0xd, iref_int77, iref_int88))
            {
                writeText("刷新成功，PWM=" + numericUpDownLDT.Value);
            }
            else
            {
                writeText("刷新失败");
            }
        }









        private void SendParam(byte head, decimal data)
        {
            if (isBJControl)
            {
                sendMagnHead = head;

                UInt32 iref_int = Convert.ToUInt32((float)data*100+0.01);

                sendMagnDataH= (byte)((iref_int) / 256);
                sendMagnDataL = (byte)(iref_int - sendMagnDataH * 256);

            }
            else
            {
                sendEnable = false;
                sendData.f = (float)data;
                sendBuf[0] = head;
                sendBuf[4] = sendData.b0;
                sendBuf[3] = sendData.b1;
                sendBuf[2] = sendData.b2;
                sendBuf[1] = sendData.b3;
                sendEnable = true;
            }
        }




        private bool ValidityCheck(string textInput)
        {
            if (pictureBoxPort.BackColor == Color.Red)
            {
                MessageBox.Show("Please Connect Serial Port Or CAN!");
                return false;
            }

            if (textInput == string.Empty)
            {
                MessageBox.Show("Please select param!");
                return false;
            }
            if (!Regex.IsMatch(textInput.Trim(), @"^-|[0-9]|\.$"))
            {
                MessageBox.Show("Please input right format value!");
                return false;
            }
            return true;
        }

        private void TMS_NUDuniversity_Click(object sender, EventArgs e)
        {
            isBJControl = false;
            comboBoxCom.Visible = true;
            buttonCanSet.Visible = false;
        }

        private void TMS_BJuniversity_Click(object sender, EventArgs e)
        {
            isBJControl = true;
            comboBoxCom.Visible = false;
            buttonCanSet.Visible = true;
        }

        private void TSM_device0_Click(object sender, EventArgs e)
        {
            magnDevIndex = 0;
        }

        private void TSM_device1_Click(object sender, EventArgs e)
        {
            magnDevIndex = 1;
        }

        private void STM_CANInde0_Click(object sender, EventArgs e)
        {
            magnCANIndex = 0;
        }

        private void STM_CANInde1_Click(object sender, EventArgs e)
        {
            magnCANIndex = 1;
        }

        private void TSM_device2_Click(object sender, EventArgs e)
        {
            motorDevIndex = 0;
        }

        private void TSM_device3_Click(object sender, EventArgs e)
        {
            motorDevIndex = 1;
        }

        private void STM_CANInde2_Click(object sender, EventArgs e)
        {
            motorCANIndex = 0;
        }

        private void STM_CANInde3_Click(object sender, EventArgs e)
        {
            motorCANIndex = 1;
        }

        private void buttonCanSet_Click(object sender, EventArgs e)
        {
            if (buttonCanSet.Text == "打开CAN")
            {
                if (CANClass.initCAN(magnDevIndex ,magnCANIndex) == 0)
                {
                    buttonCanSet.Text = "关闭CAN";
                    pictureBoxPort.BackColor = Color.Green;
                    magneticIsEnable = true;
                }
            }
            else
            {
                CANClass.closeCAN(magnDevIndex);
                magneticIsEnable = false;
                buttonCanSet.Text = "打开CAN";
                pictureBoxPort.BackColor = Color.Red;
            }
        }

        private void writeText(string txt)
        {
            textBoxLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + txt + "\t\n\t\n");
            // LogClass.writelog("mainForm", txt);
        }


        double speed = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (motorIsEnable)
            {
                //电机信息显示
                PVCI_CAN_OBJ[] pCanObj = new PVCI_CAN_OBJ[300];

                int NumValue = CANClass.rcvBuf(motorDevIndex, motorCANIndex,out pCanObj);

                for (int num = 0; num < NumValue; num++)
                {
                    byte[] Rx_recieve = new byte[pCanObj[num].DataLen];

                    for (int i = 0; i < (pCanObj[num].DataLen); i++)                   //数据信息
                    {
                        Rx_recieve[i] = pCanObj[num].Data[i];
                    }


                    if (Rx_recieve[0] == 2)
                    {
                        if (Rx_recieve[6] == 9)
                        {
                            motorI = (double)((Rx_recieve[3] * 256 + Rx_recieve[2]) * 0.003662109375 - 6);
                            motorV = (double)(((Rx_recieve[5] * 256 + Rx_recieve[4])));
                            axTChartLI.Series(0).AddXY(Count, speed, Count.ToString(), (uint)(1000));
                            axTChartLV.Series(0).AddXY(Count, motorV, Count.ToString(), (uint)(1000));



                            if (axTChartLV.Series(0).Count > 100)
                            {
                                axTChartLI.Series(0).Delete(0);
                                axTChartLV.Series(0).Delete(0);
                            }

                            motorSumI += motorI;
                            motorSumV += motorV;
                            k++;
                            if (k > 10)
                            {
                                textBoxLI.Text = (motorSumI / k).ToString("f2");
                                textBoxLV.Text = (motorSumV / k).ToString("f0");
                                motorSumI = 0;
                                motorSumV = 0;
                                k = 0;
                            }

                            if (isWriteText)
                            {
                                //  LogClass.writelog("mainForm", "I= " + motorI + " " + " V=  " + motorV);
                            }
                        }
                    }
                }
            }


            Count++;

            if (magneticIsEnable)
            {
                U_rcvData rcvData = new U_rcvData(); //接收数据


                //发送磁悬浮指令
                if (isBJControl)
                {
                    CANClass.sendMagnBuf(magnDevIndex, magnCANIndex, sendMagnHead,sendMagnDataL,sendMagnDataH);
                    Thread.Sleep(5);

                    PVCI_CAN_OBJ[] pCanObj = new PVCI_CAN_OBJ[300];
                    CANClass.rcvBuf(motorDevIndex, motorCANIndex, out pCanObj);

                    for (int i = 0; i < (pCanObj[0].DataLen); i++)                   //数据信息
                    {
                        unsafe
                        {
                            if (isShowI)
                            {
                                if(pCanObj[0].Data[i+1]>=128)
                                    rcvData.I[i / 2] =(short)(pCanObj[0].Data[i]+ pCanObj[0].Data[i+1]*256-65536);
                                else
                                    rcvData.I[i / 2] = (short)(pCanObj[0].Data[i] + pCanObj[0].Data[i+1] * 256);
                            }
                            else
                            {
                                if (pCanObj[0].Data[i + 1] >= 128)
                                    rcvData.W[i / 2] = (short)(pCanObj[0].Data[i] + pCanObj[0].Data[i+1] * 256 - 65536);
                                else
                                    rcvData.W[i / 2] = (short)(pCanObj[0].Data[i] + pCanObj[0].Data[i+1] * 256);
                            }
                            i++;
                        }
                    }

                }
                else
                {
                    if (sendEnable)
                    {
                        serialPortC.SendBuf(serialPort, sendBuf);
                    }
                    Thread.Sleep(25);
                    serialPortC.ReadBuf(serialPort, out rcvBuf);

                    unsafe
                    {
                        for (int i = 0; i < rcvBuf.Length; i++)
                        {
                            rcvData.rcvBuf[i] = rcvBuf[i];
                        }
                    }
                }







           

                unsafe
                {

                    if ((rcvBuf.Length == 42 && rcvBuf[0] == 0xe4 && rcvBuf[1] == 0xe1)||isBJControl)
                    {

                        if (isShowI)
                        {
                            axTChart1.Series(0).AddXY(Count, rcvData.I[0], Count.ToString(), (uint)(50000));
                            axTChart2.Series(0).AddXY(Count, rcvData.I[1], Count.ToString(), (uint)(50000));
                            axTChart3.Series(0).AddXY(Count, rcvData.I[2], Count.ToString(), (uint)(50000));
                            axTChart4.Series(0).AddXY(Count, rcvData.I[3], Count.ToString(), (uint)(50000));
                            axTChart5.Series(0).AddXY(Count, rcvData.I[4], Count.ToString(), (uint)(50000));
                        }
                        else
                        {
                            axTChart1.Series(0).AddXY(Count, rcvData.W[0], Count.ToString(), (uint)(1000));
                            axTChart2.Series(0).AddXY(Count, rcvData.W[1], Count.ToString(), (uint)(1000));
                            axTChart3.Series(0).AddXY(Count, rcvData.W[2], Count.ToString(), (uint)(1000));
                            axTChart4.Series(0).AddXY(Count, rcvData.W[3], Count.ToString(), (uint)(1000));
                            axTChart5.Series(0).AddXY(Count, rcvData.W[4], Count.ToString(), (uint)(1000));

                        }
                        if (pForm != null && !pForm.IsDisposed)
                        {
                           
                            double x = rcvData.W[0] - (double)numericUpDownBS1.Value;
                            double y = rcvData.W[1] - (double)numericUpDownBS2.Value;
                            double A = Math.Atan2(y, x) * 180 / 3.14;
                            double R = Math.Sqrt(x * x + y * y);
                            //pForm.axTChartUpolar.Series(0).AddXY(Math.Atan2(y, x) * 180 / 3.14, Math.Sqrt(x * x + y * y), Count.ToString(), (uint)(1000));

                            pForm.axTChartUpolar.Series(0).AddXY(A, R, Count.ToString(), (uint)(1000));
              

                            x = rcvData.W[2] - (double)numericUpDownBS3.Value;
                            y = rcvData.W[3] - (double)numericUpDownBS4.Value;
                            A = Math.Atan2(y, x) * 180 / 3.14;
                            R = Math.Sqrt(x * x + y * y);

                            pForm.axTChartDpolar.Series(0).AddXY(A, R, Count.ToString(), (uint)(1000));
                   
                            // pForm.axTChartDpolar.Series(0).AddXY(Math.Atan2(y, x) * 180 / 3.14, Math.Sqrt(x * x + y * y), Count.ToString(), (uint)(1000));
                        }
                    }





                    if (axTChart1.Series(0).Count > 50)
                    {
                        axTChart1.Series(0).Delete(0);
                        axTChart2.Series(0).Delete(0);
                        axTChart3.Series(0).Delete(0);
                        axTChart4.Series(0).Delete(0);
                        axTChart5.Series(0).Delete(0);

                    }

                    if (pForm != null && !pForm.IsDisposed)
                    {
                        if (pForm.axTChartUpolar.Series(0).Count > 500)
                        {
                            pForm.axTChartUpolar.Series(0).Clear();
                            pForm.axTChartDpolar.Series(0).Clear();
                        }
                    }

                    //电流
                    textBoxI1.Text = rcvData.I[0].ToString();
                    textBoxI2.Text = rcvData.I[1].ToString();
                    textBoxI3.Text = rcvData.I[2].ToString();
                    textBoxI4.Text = rcvData.I[3].ToString();
                    textBoxI5.Text = rcvData.I[3].ToString();

                    //占空比
                    textBoxDT1.Text = rcvData.duty[0].ToString();
                    textBoxDT2.Text = rcvData.duty[1].ToString();
                    textBoxDT3.Text = rcvData.duty[2].ToString();
                    textBoxDT4.Text = rcvData.duty[3].ToString();
                    textBoxDT5.Text = rcvData.duty[3].ToString();

                    //电机转速
                    // textBoxMotov.Text = rcvData.speed.ToString();
                    if (rcvData.speed != 0)
                    {
                        speed = (int)(6 * Math.Pow(10, 7) / rcvData.speed);
                        // speed = Math.Sin(rcvData.speed*3.14/180);
                    }

                    textBoxMotov.Text = speed.ToString();


                    if (isWriteText)
                    {
                        // LogClass.writelog("mainForm", "W=" + rcvData.W[0] + " " + rcvData.W[1] + " " + rcvData.W[2] + " " + rcvData.W[3] + " " + rcvData.W[4]);
                        // LogClass.writelog("mainForm", "I=" + rcvData.I[0] + " " + rcvData.I[1] + " " + rcvData.I[2] + " " + rcvData.I[3] + " " + rcvData.I[4]);
                        // LogClass.writelog("mainForm", "duty=" + rcvData.duty[0] + " " + rcvData.duty[1] + " " + rcvData.duty[2] + " " + rcvData.duty[3] + " " + rcvData.duty[4]);
                        LogClass.writelog("", rcvData.speed + "");
                    }
                }

            }
        }

    }

}
