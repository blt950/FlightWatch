using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LockheedMartin.Prepar3D.SimConnect;
using System.Runtime.InteropServices;

namespace FlightWatch
{
    public partial class MainForm : Form
    {

        /*
         * ===============================
         *        SET DEFAULT VARS
         * ===============================
        */
        const int WM_USER_SIMCONNECT = 0x0402;
        SimConnect simconnect = null;

        private string alarmPath = "beep.wav";
        private bool alarmTriggered = false;

        private bool WARN_annunFIRE_WARN, WARN_annunMASTER_CAUTION = false;
        private bool WARN_annunANTI_ICE, WARN_annunHYD, WARN_annunDOORS, WARN_annunENG, WARN_annunOVERHEAD, WARN_annunAIR_COND = false;


        /*
         * ===============================
         *      FORM INIT AND CLOSE
         * ===============================
        */
        public MainForm()
        {
            InitializeComponent();
            setButtons(true, false, false, false, false);

            try
            {
                System.IO.File.ReadAllLines(@"EULA.txt");
            }
            catch
            {
                EULAForm ef = new EULAForm();
                ef.ShowDialog();
            }

        }


        /*
         * ===============================
         *           STRUCTURE
         * ===============================
        */

        enum CLIENT_DATA_IDS
        {
            PMDG_NGX_DATA_ID = 0x4E477831,
            PMDG_NGX_DATA_DEFINITION = 0x4E477832,
            PMDG_NGX_CONTROL_ID = 0x4E477833,
            PMDG_NGX_CONTROL_DEFINITION = 0x4E477834,
        };
        enum DATA_REQUEST_ID
        {
            DATA_REQUEST,
            CONTROL_REQUEST,
            AIR_PATH_REQUEST
        };

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (simconnect != null)
                {
                    simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        /*
         * ===============================
         *          SIMCONNECT
         * ===============================
        */

        private void closeConnection()
        {
            if (simconnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close() 
                simconnect.Dispose();
                simconnect = null;

                displayText("Disconnected");
            }
        }

        private void initDataRequest()
        {
            try
            {

                simconnect.MapClientDataNameToID("PMDG_NGX_Data", CLIENT_DATA_IDS.PMDG_NGX_DATA_ID);
                simconnect.MapClientDataNameToID("PMDG_NGX_Control", CLIENT_DATA_IDS.PMDG_NGX_CONTROL_ID);

                simconnect.AddToClientDataDefinition(CLIENT_DATA_IDS.PMDG_NGX_DATA_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(SDK.PMDG_NGX_Data)),
                    0, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToClientDataDefinition(CLIENT_DATA_IDS.PMDG_NGX_CONTROL_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(SDK.PMDG_NGX_Control)),
                    0, SimConnect.SIMCONNECT_UNUSED);

                simconnect.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, SDK.PMDG_NGX_Data>(CLIENT_DATA_IDS.PMDG_NGX_DATA_DEFINITION);
                simconnect.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, SDK.PMDG_NGX_Control>(CLIENT_DATA_IDS.PMDG_NGX_CONTROL_DEFINITION);

                simconnect.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(simconnect_ClientData);

                simconnect.RequestClientData(CLIENT_DATA_IDS.PMDG_NGX_DATA_ID, DATA_REQUEST_ID.DATA_REQUEST, CLIENT_DATA_IDS.PMDG_NGX_DATA_DEFINITION,
                    SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                simconnect.RequestClientData(CLIENT_DATA_IDS.PMDG_NGX_CONTROL_ID, DATA_REQUEST_ID.CONTROL_REQUEST, CLIENT_DATA_IDS.PMDG_NGX_CONTROL_DEFINITION,
                    SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);

            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message, "COM Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            displayText("Connected to Prepar3D");
        }

        // The case where the user closes Prepar3D 
        void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            displayText("Prepar3D has exited. Disconnected.");
            closeConnection();
        }

        void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            MessageBox.Show("Number: " + data.dwException, "Exception recieved", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        void simconnect_ClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            switch ((DATA_REQUEST_ID)data.dwRequestID)
            {
                case DATA_REQUEST_ID.DATA_REQUEST:
                    SDK.PMDG_NGX_Data sData = (SDK.PMDG_NGX_Data)data.dwData[0];
                    this.Invoke((MethodInvoker)delegate
                    {
                        /*mcp_altitude
                        mcp_altitude.Text = Convert.ToString(sData.MCP_Altitude);
                        mcp_heading.Text = Convert.ToString(sData.MCP_Heading);
                        mcp_fd.Checked = (sData.MCP_FDSw[0] == 1 ? true : false);*/
                        //displayText(Convert.ToString(sData.WARN_annunFIRE_WARN[1]));

                        if (WARN_annunFIRE_WARN != Convert.ToBoolean(sData.WARN_annunFIRE_WARN[1]))
                        {
                            WARN_annunFIRE_WARN = Convert.ToBoolean(sData.WARN_annunFIRE_WARN[1]);
                            if (Convert.ToBoolean(sData.WARN_annunFIRE_WARN[1])) Notify("Fire Warning!");
                        }

                        if (WARN_annunMASTER_CAUTION != Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[1]))
                        {
                            WARN_annunMASTER_CAUTION = Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[1]);
                            if (Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[1]))
                            {
                                string msg = null;

                                if (Convert.ToBoolean(sData.WARN_annunFLT_CONT)) msg += "FLT-CONT ";
                                if (Convert.ToBoolean(sData.WARN_annunIRS)) msg += "IRS ";
                                if (Convert.ToBoolean(sData.WARN_annunFUEL)) msg += "FUEL ";
                                if (Convert.ToBoolean(sData.WARN_annunELEC)) msg += "ELEC ";
                                if (Convert.ToBoolean(sData.WARN_annunAPU)) msg += "APU ";
                                if (Convert.ToBoolean(sData.WARN_annunOVHT_DET)) msg += "OVHT/DET ";

                                if (msg != null) Notify("Master Caution!", msg); else Notify("Master Caution!");
                            }
                        }

                        if (WARN_annunANTI_ICE != Convert.ToBoolean(sData.WARN_annunANTI_ICE))
                        {
                            WARN_annunANTI_ICE = Convert.ToBoolean(sData.WARN_annunANTI_ICE);
                            if (Convert.ToBoolean(sData.WARN_annunANTI_ICE)) Notify("Anti-Ice Warning!");
                        }

                        if (WARN_annunHYD != Convert.ToBoolean(sData.WARN_annunHYD))
                        {
                            WARN_annunHYD = Convert.ToBoolean(sData.WARN_annunHYD);
                            if (Convert.ToBoolean(sData.WARN_annunHYD)) Notify("Hydraulics Warning!");
                        }

                        if (WARN_annunDOORS != Convert.ToBoolean(sData.WARN_annunDOORS))
                        {
                            WARN_annunDOORS = Convert.ToBoolean(sData.WARN_annunDOORS);
                            if (Convert.ToBoolean(sData.WARN_annunDOORS)) Notify("Doors Warning!");
                        }

                        if (WARN_annunENG != Convert.ToBoolean(sData.WARN_annunENG))
                        {
                            WARN_annunENG = Convert.ToBoolean(sData.WARN_annunENG);
                            if (Convert.ToBoolean(sData.WARN_annunENG)) Notify("Engine Warning!");
                        }

                        if (WARN_annunOVERHEAD != Convert.ToBoolean(sData.WARN_annunOVERHEAD))
                        {
                            WARN_annunOVERHEAD = Convert.ToBoolean(sData.WARN_annunOVERHEAD);
                            if (Convert.ToBoolean(sData.WARN_annunOVERHEAD)) Notify("Overhead Warning!");
                        }

                        if (WARN_annunAIR_COND != Convert.ToBoolean(sData.WARN_annunAIR_COND))
                        {
                            WARN_annunAIR_COND = Convert.ToBoolean(sData.WARN_annunAIR_COND);
                            if (Convert.ToBoolean(sData.WARN_annunAIR_COND)) Notify("Air-condition Warning!");
                        }

                    });
                    break;
                case DATA_REQUEST_ID.CONTROL_REQUEST:
                    SDK.PMDG_NGX_Control cData = (SDK.PMDG_NGX_Control)data.dwData[0];

                    break;
            }
        }


        /*
         * ===============================
         *         FORM FUNCTIONS
         * ===============================
        */

        private void setButtons(bool bConnect, bool bDisconnect, bool b737, bool b777, bool b747)
        {
            buttonConnect.Enabled = bConnect;
            buttonDisconnect.Enabled = bDisconnect;
            buttonB737.Enabled = b737;
            buttonB777.Enabled = b777;
            buttonB747.Enabled = b747;
        }

        private void displayText(string s)
        {
            DateTime dt = DateTime.Now;
            String time = "[" + dt.ToString("HH:mm:ss") + "] ";
            richResponse.Text = time + (s + "\n") + richResponse.Text;
        }

        private void fetchTimer_Tick(object sender, EventArgs e)
        {
            //simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.PMDG_NGX_Data, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            // Useless, remove?
        }

        private void Notify(string s, string msg = "You should check your plane.")
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = Icon,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning,
                BalloonTipTitle = s,
                BalloonTipText = msg,
            };
            notification.ShowBalloonTip(0);
            notification.Click += new EventHandler(notificationClick);

            FlashWindow.Flash(this);

            displayText(s);

            beepTimer.Start();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@alarmPath);
            player.Play();
            alarmTriggered = true;
        }

        private void notificationClick(object sender, EventArgs e)
        {
            // Not working
            this.Show();
        }

        private void beepTimer_Tick(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@alarmPath);
            player.Play();
        }

        public void Form_GetsFocused(Object sender, EventArgs e)
        {
            if (alarmTriggered) alarmTriggered = false; beepTimer.Stop();
        }


        /*
         * ===============================
         *          FORM BUTTONS
         * ===============================
        */

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (simconnect == null)
            {
                try
                {

                    simconnect = new SimConnect("Managed", this.Handle, WM_USER_SIMCONNECT, null, 0);
                    simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                    simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                    simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                   
                    setButtons(false, true, false, false, false);
                    initDataRequest();

                }
                catch (COMException ex)
                {
                    MessageBox.Show("Unable to connect to Prepar3D:\n\n" + ex.Message, "Exception recieved", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Connection error, please try again.", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                closeConnection();

                setButtons(true, false, false, false, false);
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            setButtons(true, false, false, false, false);
        }

        private void buttonB737_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonB777_Click(object sender, EventArgs e)
        {

        }

        private void buttonB747_Click(object sender, EventArgs e)
        {

        }
    }
}
