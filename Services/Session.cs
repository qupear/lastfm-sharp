// Session.cs
//
//  Copyright (C) 2008 Amr Hassan
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Xml;


namespace Lastfm.Services
{
	/// <summary>
	/// Represents your identity tokens provided by Last.fm.</summary>
	/// <remarks>
	/// A session with only API Key and API Secret is not authenticated, and it wouldn't let you
	/// perform write operations on Last.fm. In order to have it authenticated you could do the following:
	/// <code>
	/// using System;
	/// using Lastfm.Services;
	/// 
	/// string key = "b25b959554ed76058ac220b7b2e0a026";
	/// string secret = "361505f8eeaf61426ef95a4317482251";
	/// 
	/// Session session = new Session(key, secret);
	///  
	/// // one way is to ask the user for his username and password.
	/// string username = Console.ReadLine("Please enter your username: ");
	/// string md5Password = Utilities.md5(Console.ReadLine("Please enter your password: "));
	/// 
	/// // then authenticate.
	/// session.Authenticate(username, md5Password);
	/// 
	/// // another way is to let the user authenticate from the Last.fm website.
	/// string url = session.GetAuthenticationURL();
	/// Console.WriteLine("Please open the following url and follow the procedures, then press Enter: " + url);
	/// 
	/// // wait for it.
	/// Console.ReadLine();
	/// 
	/// // now that he's done, retreive the session key.
	/// session.AuthenticateViaWeb();
	/// 
	/// </code>
	/// </remarks>
	[Serializable]
	public class Session : IEquatable<Session>
	{
		/// <summary>API Key</summary>
		/// <value>
		/// The Last.fm API Key.
		/// </value>
		/// <remarks>
		/// To acquire one, please see http://www.last.fm/api/account
		/// </remarks>
		public string APIKey {get; private set;}
		
		/// <summary>API Secret</summary>
		/// <value>
		/// The Last.fm API Secret.
		/// </value>
		/// <remarks>
		/// To acquire one, please see http://www.last.fm/api/account
		/// </remarks>
		public string APISecret {get; private set;}
		
		/// <summary>Session Key</summary>
		/// <value>
		/// The Session key which represents the user's permission to let you
		/// perform "write" operations on his/her profile.
		/// </value>
		/// <remarks>
		/// To set this value, you have to call either <see cref="Session.Authenticate"/> or
		/// <see cref="Session.GetWebAuthenticationURL"/> and let the user authenticate by theirselves then
		/// call <see cref="Session.AuthenticateViaWeb"/> to complete the process.
		/// </remarks>

		public string SessionKey
		{
			get;
			private set;
		}
		
		public bool Authenticated
		{
			get { return !(SessionKey == null); }
		}
		
		private string token {get; set;}
		
		public Session(string apiKey, string apiSecret, string sessionKey)
		{
			APIKey = apiKey;
			APISecret = apiSecret;
			SessionKey = sessionKey;
		}
		
		public Session(string apiKey, string apiSecret)
		{
			APIKey = apiKey;
			APISecret = apiSecret;
		}
		
		public void Authenticate(string username, string md5Password)
		{
			RequestParameters p = new Lastfm.RequestParameters();
			
			p["username"] = username;
			p["authToken"] = Utilities.md5(username + md5Password);
			
			XmlDocument doc = (new Request("auth.getMobileSession", this, p)).execute();
			
			SessionKey = doc.GetElementsByTagName("key")[0].InnerText;
		}
		
		private string getAuthenticationToken()
		{
			XmlDocument doc = (new Request("auth.getToken", this, new RequestParameters())).execute();
			
			return doc.GetElementsByTagName("token")[0].InnerText;
		}
		
		public string GetWebAuthenticationURL()
		{
			token = getAuthenticationToken();
			
			return "http://www.last.fm/api/auth/?api_key=" + APIKey + "&token=" + token;
		}
		
		public void AuthenticateViaWeb()
		{
			RequestParameters p = new Lastfm.RequestParameters();
			p["token"] = token;
			
			Request r = new Request("auth.getSession", this, p);
			r.signIt();
			
			XmlDocument doc = r.execute();
			
			SessionKey = doc.GetElementsByTagName("key")[0].InnerText;
		}
		
		public bool Equals(Session session)
		{
			return (session.APIKey == this.APIKey &&
			        session.APISecret == this.APISecret &&
			        session.SessionKey == this.SessionKey);
		}
	}
}