using System;
using System.DirectoryServices;

namespace Byu.Utilities.LDAP
{
	/// <summary>
	/// A class for looking up information from the LDAP.
	/// </summary>
	public class Lookup
	{
		private const string ByuLDAP = "LDAP://ldap.byu.edu/o=byu.edu/ou=people";
		/// <summary>
		/// Gets whether a particular NetID exists in the LDAP.
		/// </summary>
		/// <param name="netID">
		/// The NetID of the person being queried.
		/// </param>
		/// <returns>
		/// True if the NetID exists in the LDAP.  False otherwise.
		/// </returns>
		public static bool Contains(string netID)
		{
			if( netID == null || netID.Length == 0 ) throw new ArgumentNullException("netID");

			DirectoryEntry de = Lookup.getAnonymousBind();
			DirectorySearcher ds = Lookup.getDirectorySearcher(de, netID);
			SearchResult src = ds.FindOne();

			return src != null;
		}
		/// <summary>
		/// Gets the meta data that is publicly available regarding a specific
		/// person in the database.
		/// </summary>
		/// <param name="netID">
		/// The NetID of the person being queried.
		/// </param>
		/// <returns>
		/// A structure containing the fields that were able 
		/// to be looked up.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the NetID specified does not exist in the LDAP.
		/// </exception>
		public static NetIDDetail getDetails(string netID)
		{
			NetIDDetail details = getDetailsIfExists(netID);
			if( details == null ) throw new ArgumentOutOfRangeException("netID", netID, "Could not find in LDAP.");
			return details;
		}

		/// <summary>
		/// Gets the meta data that is publicly available regarding a specific
		/// person in the database.
		/// </summary>
		/// <param name="netID">
		/// The NetID of the person being queried.
		/// </param>
		/// <returns>
		/// A structure containing the fields that were able 
		/// to be looked up.
		/// </returns>
		public static NetIDDetail getDetailsIfExists(string netID)
		{
			if( netID == null || netID.Length == 0 ) throw new ArgumentNullException("netID");

			DirectoryEntry de = Lookup.getAnonymousBind();
			DirectorySearcher ds = Lookup.getDirectorySearcher(de, netID);
			SearchResult src = ds.FindOne();
			if( src == null ) return null; // not found
			de = src.GetDirectoryEntry();

			return new NetIDDetail(de);
		}
		internal static DirectoryEntry getAnonymousBind()
		{
			DirectoryEntry de = new DirectoryEntry(ByuLDAP);
			de.AuthenticationType = AuthenticationTypes.None;
			return de;
		}

		internal static DirectoryEntry getBind(string netID, string password)
		{
			if( netID == null ) throw new ArgumentNullException("netID");
			if( password == null ) throw new ArgumentNullException("password");

			return new DirectoryEntry(ByuLDAP, "uid=" + netID + ",ou=people,o=byu.edu", password, AuthenticationTypes.None);
		}

		internal static DirectorySearcher getDirectorySearcher(DirectoryEntry de)
		{
			return getDirectorySearcher(de, null);
		}
		internal static DirectorySearcher getDirectorySearcher(DirectoryEntry de, string netID)
		{
			if( de == null ) throw new ArgumentNullException("de");
			if( netID != null && netID.Length == 0 ) netID = null;

			DirectorySearcher ds = new DirectorySearcher(de);
			ds.SearchScope = SearchScope.Subtree;
			ds.Filter = "(|" +
							"(employeeType=active part-time employee)" +
							"(employeeType=active part-time instructor)" +
							"(employeeType=active full-time employee)" +
							"(employeeType=active full-time instructor)" +
							"(employeeType=active eligible to register student)" +
				")";
			if( netID != null ) ds.Filter = string.Format("(&({0})(uid={1}))", ds.Filter, netID);
			ds.CacheResults = false;
			ds.SizeLimit = 10;
			ds.PropertiesToLoad.Add("uid");
			ds.PropertiesToLoad.Add("cn"); // combined name
			ds.PropertiesToLoad.Add("givenname");
			ds.PropertiesToLoad.Add("sn"); // surname
			ds.PropertiesToLoad.Add("employeeType");
			ds.PropertiesToLoad.Add("mail");
			ds.PropertiesToLoad.Add("homephone");
			ds.PropertiesToLoad.Add("title");
			ds.PropertiesToLoad.Add("ou");
			ds.PropertiesToLoad.Add("roomnumber");
			ds.PropertiesToLoad.Add("telephonenumber");
			ds.PropertiesToLoad.Add("homepostaladdress");

			return ds;
		}
	}
}
