using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gtk;
using MySplunkApp;

public partial class MainWindow : Gtk.Window
{
	static string user;
    static string pass;
	static string token;
    public static string respOut;
    public static Stream streamOut;
    
	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnEntry1Changed(object sender, EventArgs e)
	{
		user = entry1.Text;
	}

	protected void OnEntry3Changed(object sender, EventArgs e)
	{
		pass = entry3.Text;
	}
    
	protected void OnButton5Clicked(object sender, EventArgs e)
	{
		//var a = BasicAuth(user, pass).Result;
		var a = login(user, pass).Result;
        if(a == true)
        //if (a != null)
        {
            textview1.Buffer.Text = "Logging in.";
            this.Destroy();
            Window2 win2 = new Window2();
            win2.Show();
        }
        else
        {
            textview1.Buffer.Text = "Failed.";
        }
	      
		//var a = login(user,pass).Result;
		//if (a != null)
  //      {
		//	textview1.Buffer.Text = "Logging in.";
  //          this.Destroy();
  //          Window2 win2 = new Window2();
  //          win2.Show();
  //      }
		//else{
		//	textview1.Buffer.Text = "Failed.";
		//}
	}

	protected void OnButton3Clicked(object sender, EventArgs e)
	{
	}
	static string retstring(){
		string s = string.Format("user {0} pass {1}", user, pass);
		return s;
	}


	static async Task<bool> login(string username, string password)
	{
		ServicePointManager.ServerCertificateValidationCallback =
(System.Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;

		using (HttpClient client = new HttpClient())
		{
			client.BaseAddress = new Uri("https://10.0.0.62:8089/services/auth/login");
			client.DefaultRequestHeaders.Add("Authorization", "Basic");
			var postData = new List<KeyValuePair<string, string>>();
			postData.Add(new KeyValuePair<string, string>("username", username));
			postData.Add(new KeyValuePair<string, string>("password", password));

			HttpContent body = new FormUrlEncodedContent(postData);
			HttpResponseMessage response = await client.PostAsync(client.BaseAddress, body);
			HttpContent content = response.Content;

			bool x = response.IsSuccessStatusCode;
			// lambda here
			string result = await content.ReadAsStringAsync();
			return x;

		}
	}





	public static async Task<bool> BasicAuth(string username, string password)
	{
		bool boolAuth = false;
		using (HttpClient client = new HttpClient())
		{
			client.BaseAddress = new Uri("https://10.0.0.62:8089/services/auth/login");
			client.DefaultRequestHeaders.Add("Authorization", "Basic");
			var postData = new List<KeyValuePair<string, string>>();
			postData.Add(new KeyValuePair<string, string>("username", username));
			postData.Add(new KeyValuePair<string, string>("password", password));

			HttpContent body = new FormUrlEncodedContent(postData);
			HttpResponseMessage response = await client.PostAsync(client.BaseAddress, body);
			HttpContent content = response.Content;
			if (response.IsSuccessStatusCode)
			{
				boolAuth = true;
				DecodeResponse();
			}
			// lambda here
			string result = await content.ReadAsStringAsync();
			respOut = result;
			streamOut = await content.ReadAsStreamAsync();
			// token => decode()
		}
		return boolAuth;
	}
	[XmlType("response")]
	public partial class tokenClass
	{
		[XmlElement("sessionKey")]
		public string sessionKey { get; set; }
	}

	public static void DecodeResponse()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(tokenClass));
		var obj = (tokenClass)serializer.Deserialize(streamOut);
		//return obj.sessionKey;
		token = obj.sessionKey;

	}
}

        

