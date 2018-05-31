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
	static string token = null;
    public static string respOut;
    public static Stream streamOut;
	public static bool xAuth;
    
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
		
		String a = login(user, pass).Result;
		//Decoder(a);

        if(xAuth == true)
      
        {
			//MessageDialog messageDialog = new MessageDialog("login success.");

			textview1.Buffer.Text = string.Format("Logging in.");
            //this.Destroy();
            //Window2 win2 = new Window2();
            //win2.Show();
        }
        else
        {
            textview1.Buffer.Text = "Failed.";
        }
	      

	}

	protected void OnButton3Clicked(object sender, EventArgs e)
	{
	}
	static string retstring(){
		string s = string.Format("user {0} pass {1}", user, pass);
		return s;
	}
    

	public static async Task<string> login(string username, string password)
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

			 xAuth = response.IsSuccessStatusCode;


			//streamOut = await content.ReadAsStreamAsync();
            
			// lambda here
			string result = await content.ReadAsStringAsync();

			var z = await content.ReadAsStreamAsync();


			//if (x == true){
			//	DecodeResponse();

			//}
			//return z;
			return result;

		}
	}

	[XmlType("response")]
	public partial class tokenClass1
	{
		[XmlElement("sessionKey")]
		public string sessionKey { get; set; }
	}

	public static void Decoder(Stream stream)
    {
        XmlSerializer serializer = new
   XmlSerializer(typeof(tokenClass1));
        tokenClass1 i;
        i = (tokenClass1)serializer.Deserialize(stream);
        token = i.sessionKey;
       
    }
}

        

