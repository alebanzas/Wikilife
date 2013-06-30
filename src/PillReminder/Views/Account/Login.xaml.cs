using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace PillReminder.Views.Account
{
    public partial class Login : PhoneApplicationPage
    {
        public string LoginURL = "http://api.wikilife.org/1/account/validate.json";

        public string UserName { get; set; }
        public string PIN { get; set; }

        public Login()
        {
            InitializeComponent();
        }

        private void StartRequest()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    LblResult.Text = string.Empty;
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

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartRequest();

            UserName = TxtUsename.Text;
            PIN = TxtPIN.Text;

            var webRequest = WebRequest.CreateHttp(LoginURL);
            webRequest.Method = "POST"; 
            webRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), webRequest);
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var postStream = webRequest.EndGetRequestStream(asynchronousResult);

            string postData = GetLoginData();

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            webRequest.BeginGetResponse(ResponseCallback, webRequest);
        }

        private string GetLoginData()
        {
            var user = new LoginRequestModel
                {
                    user_name = UserName,
                    pin = PIN,
                };

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(LoginRequestModel));
            ser.WriteObject(ms, user);
            var json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            var httpRequest = (HttpWebRequest)asyncResult.AsyncState;
            var response = httpRequest.EndGetResponse(asyncResult);
            var stream = response.GetResponseStream();

            var serializer = new DataContractJsonSerializer(typeof(LoginResponseModel));
            var result = (LoginResponseModel)serializer.ReadObject(stream);

            ProcessResult(result);
            EndRequest();
        }

        private void ProcessResult(LoginResponseModel result)
        {
            if ("OK".Equals(result.status))
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        LblResult.Text = result.message;
                    });
            }
        }
    }

    public class LoginRequestModel
    {
        public string pin { get; set; }

        public string user_name { get; set; }
    }

    public class LoginResponseModel
    {
        public string status { get; set; }

        public string message { get; set; }

        public string user_id { get; set; }
    }
}