﻿using System;
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
        SimConnect simconnectPMDG = null;


        private int trackingType = 0; // 1 = NGX, 2 = 777, 3 = 747

        private string alarmPath = "beep.wav";
        private bool alarmTriggered = false;

        private bool WARN_annunFIRE_WARN, WARN_annunMASTER_CAUTION, WARN_annunMASTER_WARNING = false;
        private bool WARN_annunANTI_ICE, WARN_annunHYD, WARN_annunDOORS, WARN_annunENG, WARN_annunOVERHEAD, WARN_annunAIR_COND, MAIN_annunAP, MAIN_annunAT = false;

        private bool WARN_stall, WARN_overspeed = false;


        /*
         * ===============================
         *      FORM INIT AND CLOSE
         * ===============================
        */
        public MainForm()
        {
            InitializeComponent();
            setButtons(true, false, false, false);

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct MyStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string title;
            public bool stall;
            public bool overspeed;
        };

        enum DEFINITIONS : uint
        {
            MyStruct,
        }

        enum CLIENT_DATA_IDS_NGX
        {
            PMDG_NGX_DATA_ID = 0x4E477831,
            PMDG_NGX_DATA_DEFINITION = 0x4E477832,
            PMDG_NGX_CONTROL_ID = 0x4E477833,
            PMDG_NGX_CONTROL_DEFINITION = 0x4E477834,
        };
        enum CLIENT_DATA_IDS_777X
        {
            PMDG_777X_DATA_ID = 0x4E477831,
            PMDG_777X_DATA_DEFINITION = 0x4E477832,
            PMDG_777X_CONTROL_ID = 0x4E477833,
            PMDG_777X_CONTROL_DEFINITION = 0x4E477834,
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

                if (simconnectPMDG != null)
                {
                    simconnectPMDG.ReceiveMessage();
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
            if (simconnect != null && simconnectPMDG != null)
            {
                // Dispose serves the same purpose as SimConnect_Close() 
                simconnect.Dispose();
                simconnect = null;

                simconnectPMDG.Dispose();
                simconnectPMDG = null;

                displayText("Disconnected");

            } else {
                MessageBox.Show("Connection closing error. This should not happen, please contact author.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void initDataRequest()
        {
            try
            {
                if(trackingType == 1)
                {
                    // PMDG NGX Vars
                    simconnectPMDG.MapClientDataNameToID("PMDG_NGX_Data", CLIENT_DATA_IDS_NGX.PMDG_NGX_DATA_ID);
                    simconnectPMDG.MapClientDataNameToID("PMDG_NGX_Control", CLIENT_DATA_IDS_NGX.PMDG_NGX_CONTROL_ID);

                    simconnectPMDG.AddToClientDataDefinition(CLIENT_DATA_IDS_NGX.PMDG_NGX_DATA_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(NGX_SDK.PMDG_NGX_Data)),
                        0, SimConnect.SIMCONNECT_UNUSED);
                    simconnectPMDG.AddToClientDataDefinition(CLIENT_DATA_IDS_NGX.PMDG_NGX_CONTROL_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(NGX_SDK.PMDG_NGX_Control)),
                        0, SimConnect.SIMCONNECT_UNUSED);

                    simconnectPMDG.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, NGX_SDK.PMDG_NGX_Data>(CLIENT_DATA_IDS_NGX.PMDG_NGX_DATA_DEFINITION);
                    simconnectPMDG.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, NGX_SDK.PMDG_NGX_Control>(CLIENT_DATA_IDS_NGX.PMDG_NGX_CONTROL_DEFINITION);

                    simconnectPMDG.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(simconnect_ClientData);

                    simconnectPMDG.RequestClientData(CLIENT_DATA_IDS_NGX.PMDG_NGX_DATA_ID, DATA_REQUEST_ID.DATA_REQUEST, CLIENT_DATA_IDS_NGX.PMDG_NGX_DATA_DEFINITION,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                    simconnectPMDG.RequestClientData(CLIENT_DATA_IDS_NGX.PMDG_NGX_CONTROL_ID, DATA_REQUEST_ID.CONTROL_REQUEST, CLIENT_DATA_IDS_NGX.PMDG_NGX_CONTROL_DEFINITION,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                } else if(trackingType == 2) {
                    // PMDG B777 Vars
                    simconnectPMDG.MapClientDataNameToID("PMDG_777X_Data", CLIENT_DATA_IDS_777X.PMDG_777X_DATA_ID);
                    simconnectPMDG.MapClientDataNameToID("PMDG_777X_Control", CLIENT_DATA_IDS_777X.PMDG_777X_CONTROL_ID);

                    simconnectPMDG.AddToClientDataDefinition(CLIENT_DATA_IDS_777X.PMDG_777X_DATA_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(B777_SDK.PMDG_777X_Data)),
                        0, SimConnect.SIMCONNECT_UNUSED);
                    simconnectPMDG.AddToClientDataDefinition(CLIENT_DATA_IDS_777X.PMDG_777X_CONTROL_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(B777_SDK.PMDG_777X_Control)),
                        0, SimConnect.SIMCONNECT_UNUSED);

                    simconnectPMDG.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, B777_SDK.PMDG_777X_Data>(CLIENT_DATA_IDS_777X.PMDG_777X_DATA_DEFINITION);
                    simconnectPMDG.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, B777_SDK.PMDG_777X_Control>(CLIENT_DATA_IDS_777X.PMDG_777X_CONTROL_DEFINITION);

                    simconnectPMDG.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(simconnect_ClientData);

                    simconnectPMDG.RequestClientData(CLIENT_DATA_IDS_777X.PMDG_777X_DATA_ID, DATA_REQUEST_ID.DATA_REQUEST, CLIENT_DATA_IDS_777X.PMDG_777X_DATA_DEFINITION,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                    simconnectPMDG.RequestClientData(CLIENT_DATA_IDS_777X.PMDG_777X_CONTROL_ID, DATA_REQUEST_ID.CONTROL_REQUEST, CLIENT_DATA_IDS_777X.PMDG_777X_CONTROL_DEFINITION,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);

                }
                

                // P3D Default vars
                simconnect.AddToDataDefinition(DEFINITIONS.MyStruct, "title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.MyStruct, "STALL WARNING", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.MyStruct, "OVERSPEED WARNING", null, SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.RegisterDataDefineStruct<MyStruct>(DEFINITIONS.MyStruct);
                simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimobjectData);
                simconnect.RequestDataOnSimObject(DEFINITIONS.MyStruct, DEFINITIONS.MyStruct, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);


            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message, "COM Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                    if (trackingType == 1) // *** PMDG NGX ***
                    {
                        NGX_SDK.PMDG_NGX_Data sData = (NGX_SDK.PMDG_NGX_Data)data.dwData[0];
                        this.Invoke((MethodInvoker)delegate
                        {

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

                            // The light flashes, so we block it spamming notifications if alarm already on.
                            if (!alarmTriggered)
                            {
                                if (MAIN_annunAP != Convert.ToBoolean(sData.MAIN_annunAP[0]))
                                {
                                    MAIN_annunAP = Convert.ToBoolean(sData.MAIN_annunAP[0]);
                                    if (Convert.ToBoolean(sData.MAIN_annunAP[0])) Notify("Auto Pilot Warning!");
                                }

                                if (MAIN_annunAT != Convert.ToBoolean(sData.MAIN_annunAT[0]))
                                {
                                    MAIN_annunAT = Convert.ToBoolean(sData.MAIN_annunAT[0]);
                                    if (Convert.ToBoolean(sData.MAIN_annunAT[0])) Notify("Auto Throttle Warning!");
                                }
                            }

                        });
                    } else if (trackingType == 2) { // *** PMDG B777 ***

                        B777_SDK.PMDG_777X_Data sData = (B777_SDK.PMDG_777X_Data)data.dwData[0];
                        this.Invoke((MethodInvoker)delegate
                        {

                            if (WARN_annunMASTER_WARNING != Convert.ToBoolean(sData.WARN_annunMASTER_WARNING[0]))
                            {
                                WARN_annunMASTER_WARNING = Convert.ToBoolean(sData.WARN_annunMASTER_WARNING[0]);
                                if (Convert.ToBoolean(sData.WARN_annunMASTER_WARNING[0])) Notify("Master Warning!");
                            }

                            if (WARN_annunMASTER_CAUTION != Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[0]))
                            {
                                WARN_annunMASTER_CAUTION = Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[0]);
                                if (Convert.ToBoolean(sData.WARN_annunMASTER_CAUTION[0])) Notify("Master Caution!");
                            }

                        });

                    }

                    
                    break;
                case DATA_REQUEST_ID.CONTROL_REQUEST:

                    if(trackingType == 1)
                    {
                        NGX_SDK.PMDG_NGX_Control cData = (NGX_SDK.PMDG_NGX_Control)data.dwData[0];
                    } else if (trackingType == 2)
                    {
                        B777_SDK.PMDG_777X_Control cData = (B777_SDK.PMDG_777X_Control)data.dwData[0];
                    }
                    
                    break;
            }
        }

        void simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((DEFINITIONS)data.dwRequestID)
            {
                case DEFINITIONS.MyStruct:
                    MyStruct sData = (MyStruct)data.dwData[0];

                    if (WARN_stall != sData.stall)
                    {
                        WARN_stall = sData.stall;
                        if (sData.stall) Notify("Stall!");
                    }

                    if (WARN_overspeed != sData.overspeed)
                    {
                        WARN_overspeed = sData.overspeed;
                        if (sData.overspeed) Notify("Overspeed!");
                    }

                    break;
            }
        }

        private void labelLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://forum.vatsim-scandinavia.org/topic/2090-pilotwatch-fly-safely-when-afk/");
        }


        /*
         * ===============================
         *         FORM FUNCTIONS
         * ===============================
        */

        private void setButtons(bool bConnect, bool bDisconnect, bool b737, bool b777)
        {
            buttonConnect.Enabled = bConnect;
            buttonDisconnect.Enabled = bDisconnect;
            buttonB737.Enabled = b737;
            buttonB777.Enabled = b777;
        }

        private void displayText(string s)
        {
            DateTime dt = DateTime.Now;
            String time = "[" + dt.ToString("HH:mm:ss") + "] ";
            richResponse.Text = time + (s + "\n") + richResponse.Text;
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
            if (simconnect == null && simconnectPMDG == null)
            {
                try
                {
                    
                    simconnect = new SimConnect("Managed", this.Handle, WM_USER_SIMCONNECT, null, 0);
                    simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                    simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                    
                    simconnectPMDG = new SimConnect("Managed", this.Handle, WM_USER_SIMCONNECT, null, 0);
                    simconnectPMDG.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                    simconnectPMDG.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

                    displayText("Connected to Prepar3D");
                    setButtons(false, true, true, true);

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

                setButtons(true, false, false, false);
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            setButtons(true, false, false, false);

            buttonB737.Text = "Track PMDG 737";
            buttonB777.Text = "Track PMDG 777";

            trackingType = 0;
        }

        private void buttonB737_Click(object sender, EventArgs e)
        {
            trackingType = 1;

            initDataRequest();
            displayText("Flight tracking started.");

            setButtons(false, true, false, false);
            buttonB737.Text = "Tracking PMDG 737...";
        }

        private void buttonB777_Click(object sender, EventArgs e)
        {
            trackingType = 2;

            initDataRequest();
            displayText("Flight tracking started.");

            setButtons(false, true, false, false);
            buttonB777.Text = "Tracking PMDG 777...";
        }
    }
}
