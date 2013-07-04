using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PillReminder.Views.Account
{
    public partial class Register : PhoneApplicationPage
    {
        public string RegisterURL = "http://api.wikilife.org/1/account/add.json";
        public RegisterRequestModel RequestModel { get; set; }

        public Register()
        {
            InitializeComponent();
        }
        
        private void StartRequest()
        {
            Dispatcher.BeginInvoke(() =>
            {
                BtnSend.IsEnabled = false;
            });
        }

        private void EndRequest()
        {
            Dispatcher.BeginInvoke(() =>
            {
                BtnSend.IsEnabled = true;
            });
        }

        private void Register_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartRequest();

            RequestModel = new RegisterRequestModel
                {
                    birthdate = TxtBirthDate.Value.Value.ToShortDateString(),
                    country = "USA",
                    device_id = "43fcf5703df2588c9860eb7a1d039c6f58729897",
                    gender = "Male",//TODO
                    height = float.Parse(TxtHeight.Text),
                    pin = TxtPIN.Text,
                    timezone = "GMT",
                    user_id = "",
                    user_name = TxtUserName.Text,
                    weight = float.Parse(TxtWeight.Text),
                };

            var webRequest = WebRequest.CreateHttp(RegisterURL);
            webRequest.Method = "POST";
            webRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), webRequest);
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var postStream = webRequest.EndGetRequestStream(asynchronousResult);

            string postData = GetRegisterData();

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            webRequest.BeginGetResponse(ResponseCallback, webRequest);
        }

        private string GetRegisterData()
        {
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(RegisterRequestModel));
            ser.WriteObject(ms, RequestModel);
            var json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            var httpRequest = (HttpWebRequest)asyncResult.AsyncState;
            var response = httpRequest.EndGetResponse(asyncResult);
            var stream = response.GetResponseStream();

            var serializer = new DataContractJsonSerializer(typeof(RegisterResponseModel));
            var result = (RegisterResponseModel)serializer.ReadObject(stream);

            ProcessResult(result);
            EndRequest();
        }

        private void ProcessResult(RegisterResponseModel result)
        {
            if ("OK".Equals(result.status))
            {
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)));
            }
            else
            {
                Dispatcher.BeginInvoke(() => Dispatcher.BeginInvoke(() => MessageBox.Show("Ocurrió un error.")));
            }
        }
    }

    public class RegisterRequestModel
    {
        public string timezone { get; set; }

        public float weight { get; set; }

        public string device_id { get; set; }

        public string birthdate { get; set; }

        public string user_id { get; set; }

        public string gender { get; set; }

        public string pin { get; set; }

        public float height { get; set; }

        public string country { get; set; }

        public string user_name { get; set; }
    }

    public class DateModel
    {
        public string date { get; set; }
    }

    public class RegisterResponseModel
    {
        public string status { get; set; }

        public DateModel create_utc { get; set; }

        public string user_id { get; set; }

        public string pin { get; set; }

        public bool auto { get; set; }

        public OAuthModel _id { get; set; }
    }

    public class OAuthModel
    {
        public string oid { get; set; }

        public string user_name { get; set; }
    }
}