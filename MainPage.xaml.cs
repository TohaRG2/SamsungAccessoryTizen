using System;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using Samsung.Sap;
using System.Linq;
using System.Diagnostics;
using Tizen.Wearable.CircularUI.Forms;

using Xamarin.Forms.Xaml;

namespace DisplayText
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private int count = 0;

        private Agent agent;
        private Connection connection;
        private string TAG = "DisplayTextWear";
        string receivedMessage = "recivedMessage";
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnTopButtonPress(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            count = count + 1;

            this.textLabel.Text = count.ToString();

        }



        private void OnBottomButtonPress(object sender, EventArgs e)
        {
            Connect();
        }

        private async void Connect()
        {
            try
            {
                agent = await Agent.GetAgent("/tohaman/display_text");
                var peers = await agent.FindPeers();
                if (peers.Count() > 0)
                {
                    var peer = peers.First();
                    connection = peer.Connection;
                    connection.DataReceived -= Connection_DataReceived;
                    connection.DataReceived += Connection_DataReceived;
                    await connection.Open();
                    ShowMessage("Connected");
                }
                else
                {
                    ShowMessage("Any peer not found");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, ex.ToString());
            }
        }

        //
        private void Connection_DataReceived(object sender, Samsung.Sap.DataReceivedEventArgs e)
        {
            ShowMessage("Something received from phone");
            string s = Encoding.UTF8.GetString(e.Data);
            ReceivedMessage = s;
            Tizen.Log.Debug(TAG, s);
        }

        private void ShowMessage(string message, string debugLog = null)
        {
            Toast.DisplayText(message, 3000);
            if (debugLog != null)
            {
                Console.WriteLine(TAG + debugLog);
            }
            Console.WriteLine(TAG + message);
        }

        public class BgReading
        {
            public double value { get; set; }
            public string time { get; set; }
            public string direction { get; set; }
        }

        double bgValue = 0;
        public double BgValue
        {
            get => bgValue;
            set
            {
                bgValue = value;
                var args = new PropertyChangedEventArgs(nameof(BgValue));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public string ReceivedMessage
        {
            get => receivedMessage;
            set
            {
                receivedMessage = value;
                var args = new PropertyChangedEventArgs(nameof(ReceivedMessage));
                PropertyChanged?.Invoke(this, args);
            }
        }
    }

}
