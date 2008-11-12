// Base.cs
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
using System.Collections.Generic;
using System.Xml;

namespace lastfm.Services
{
	public abstract class Base
	{
		protected string APIKey {get; private set;}
		protected string Secret {get; private set;}
		protected string SessionKey {get; private set;}
		
		public Base(string apiKey, string secret, string sessionKey)
		{
			this.APIKey = apiKey;
			this.Secret = secret;
			this.SessionKey = sessionKey;
		}
    
    public Base(string[] authData)
    {
      this.APIKey = authData[0];
      this.Secret = authData[1];
      this.SessionKey = authData[2];
    }
		
		protected virtual RequestParameters getParams()
    {
      // OVERRIDE ME
      return new RequestParameters();
    }
    
    protected XmlDocument request(string methodName, RequestParameters parameters)
    {
      return (new Request(methodName, APIKey, parameters, Secret, SessionKey)).execute();
    }
    
    protected XmlDocument request(string methodName)
    {
      return (new Request(methodName, APIKey, getParams(), Secret, SessionKey)).execute();
    }
    
    protected string[] getAuthData()
    {
      return new string[] {this.APIKey, this.Secret, this.SessionKey};
    }
    
    protected string extract(XmlElement element, string name, int index)
    {
      return element.GetElementsByTagName(name)[index].InnerText;
    }
    
    protected string extract(XmlElement element, string name)
    {
      return extract(element, name, 0);
    }
    
    protected string[] extractAll(XmlElement element, string name, int limitCount)
    {
      string[] s = extractAll(element, name);
      List<string> l = new List<string>();
      
      for(int i = 0; i < limitCount; i++)
        l.Add(s[i]);
      
      return l.ToArray();
    }
    
    protected string[] extractAll(XmlElement element, string name)
    {
      List<string> list = new List<string>();
      
      for(int i = 0; i < element.GetElementsByTagName(name).Count; i++)
          list.Add(extract(element, name, i));
      
      return list.ToArray();
    }
	}
}
