using System;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.Collections.Generic;

namespace Byu.Utilities.LDAP
{
	/// <summary>
	/// The NetIDDetail class contains the properties that are filled with user
	/// information at the end of a successful authentication or LDAP query.
	/// </summary>
	public class NetIDDetail
	{
		#region Private members
		private DirectoryEntry de;
		#endregion

		#region Construction
		internal NetIDDetail(DirectoryEntry de)
		{
			if( de == null ) throw new ArgumentNullException("de");
			this.de = de;
		}

		#endregion
       
		#region Attributes
		/// <summary>
		/// Looks up a string value in the LDAP directory entry.
		/// </summary>
		/// <param name="fieldName">
		/// The specific field to look up.
		/// </param>
		/// <returns>
		/// The string value of the queried property.
		/// </returns>
		protected string getString(string fieldName)
		{
			return de.Properties[fieldName].Value.ToString();
		}

		protected bool IsInRole(string p)
		{
			var employeeTypes = (object[])de.Properties["employeeType"].Value;
			return employeeTypes.Contains(p);
		}

		/// <summary>
		/// The NetID of the queried or authenticated user.
		/// </summary>
		public string NetID { get { return getString("uid"); } }
		/// <summary>
		/// The full name of the queried or authenticated user.
		/// </summary>
		public string FullName { get { return getString("cn"); } }
		/// <summary>
		/// The first name of the queried or authenticated user.
		/// </summary>
		/// <remarks>
		/// The given names field is queried, and the first word is returned.
		/// </remarks>
		public string FirstName 
		{ 
			get 
			{ 
				string givenNames = getString("givenname");
				int idxFirstSpace;
				if( (idxFirstSpace = givenNames.IndexOf(" ")) > 0 )
					return givenNames.Substring(0, idxFirstSpace);
				else
					return givenNames;
			} 
		}
		/// <summary>
		/// The middle name of the queried or authenticated user.  
		/// </summary>
		/// <remarks>
		/// The given names field is queried, and all but the first word is returned.
		/// If no more than one given name is listed, string.Empty is returned.
		/// </remarks>
		public string MiddleName
		{
			get
			{
				string givenNames = getString("givenname");
				int idxFirstSpace;
				if( (idxFirstSpace = givenNames.IndexOf(" ")) <= 0 ) return string.Empty;
				return givenNames.Substring(idxFirstSpace+1);
			}
		}
		/// <summary>
		/// The last name of the queried or authenticated user.
		/// </summary>
		public string LastName { get { return getString("sn"); } }
		/// <summary>
		/// The primary email address of the queried or authenticated user.
		/// </summary>
		public string Email { get { return getString("mail"); } }
		/// <summary>
		/// The home telephone number of the queried or authenticated user.
		/// </summary>
		public string HomeTelephone { get { return getString("homephone"); } }
		/// <summary>
		/// The home address of the queried or authenticated user.
		/// </summary>
		public string HomeAddress { get { return getString("homepostaladdress"); } }
		/// <summary>
		/// The professional title of the queried or authenticated user.
		/// </summary>
		public string Title { get { return TitleCase(getString("title")); } }
		/// <summary>
		/// The organizational unit the queried or authenticated user belongs to.
		/// </summary>
		public string Department { get { return TitleCase(getString("ou")); } }
		/// <summary>
		/// The on campus room number and building of the queried or authenticated user.
		/// </summary>
		public string CampusAddress { get { return getString("roomnumber"); } }
		/// <summary>
		/// The on campus telephone number for the queried or authenticated user.
		/// </summary>
		public string CampusTelephone { get { return getString("telephonenumber"); } }

		public bool IsStudent { get { return IsInRole("Active Eligible to Register Student"); } }
		public bool IsAlumni { get { return IsInRole("Alumni"); } }
		public bool IsPartTimeEmployee { get { return IsInRole("Active Part-time Employee"); } }
		public bool IsFullTimeEmployee { get { return IsInRole("Active Full-time Employee"); } }
		public bool IsPartTimeFaculty { get { return IsInRole("Active Part-time Instructor"); } }
		public bool IsFullTimeFaculty { get { return IsInRole("Active Full-time Instructor"); } }
		public bool IsEmployee { get { return IsPartTimeEmployee || IsFullTimeEmployee || IsPartTimeFaculty || IsFullTimeFaculty; } }

		/// <summary>
		/// Returns user roles:
		/// "Alumni"
		/// "Active Eligible to Register Student"
		/// "Active Part-time Instructor"
		/// "Active Full-time Instructor"
		/// "Active Part-time Employee"
		/// "Active Full-time Employee"
		/// </summary>
		public string[] Roles { get { return (string[])((object[])de.Properties["employeeType"].Value).Cast<string>(); } }
		#endregion

		#region Operations
		/// <summary>
		/// Called to send the photo ID of the person to the browser.
		/// </summary>
		/// <remarks>
		/// Called during an image request from the browser for the 
		/// LDAP member's photo ID.  
		/// </remarks>
		/*public void StreamPhotoToBrowser()
		{
			if( HttpContext.Current == null ) throw new InvalidOperationException("No ASP.NET web request could be detected.");

			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.ContentType = "image/jpeg";
			// TODO: code here
			throw new NotImplementedException();

			//Response.End();
		}*/
		/// <summary>
		/// Converts the details of a user to a textual description
		/// with only one attribute per line.
		/// </summary>
		/// <returns>
		/// The text string description.
		/// </returns>
		public override string ToString()
		{
			return string.Format("NetID: {0}\nName: {1}\nMail: {2}", NetID, FullName, Email);
		}

		#endregion

		#region Helper methods
		/// <summary>
		/// Capitalizes the first letter of every word, if the input string is all caps or no caps.
		/// </summary>
		/// <param name="str">The string to perform the filter on.</param>
		/// <returns>The modified string.</returns>
		/// <example>
		/// The following string: 
		/// ANDREW ARNOTT
		/// would be modified to 
		/// Andrew Arnott
		/// 
		/// But this string:
		/// Andrea McManus
		/// would be left alone because it is not all caps or all lower caps.
		/// </example>
		protected static string TitleCase(string str)
		{
			// do not touch string unless it is all upper- or lower- case.
			if( !( str.ToUpper() == str || str.ToLower() == str ) ) return str;

			str = str.ToLower(); // start with everything lowercase			
			int idx = -1;
			while( true )
			{
				idx++; // move to the character after the space
				if( char.IsLetter(str,idx) )
					str = str.Substring(0, idx) + char.ToUpper(str[idx]) + str.Substring(idx+1);
				if( (idx = str.IndexOfAny(new char[] {' ', '\t', '\n', '\r'}, idx)) == -1 ) break;
			}
			return str;
		}
		#endregion
	}
}
