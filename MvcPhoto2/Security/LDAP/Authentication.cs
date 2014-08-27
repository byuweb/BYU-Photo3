using System;
using System.Diagnostics;
using System.DirectoryServices;

namespace Byu.Utilities.LDAP
{
	/// <summary>
	/// The LDAP class provides a simple interface with LDAP on BYU campus.
	/// It can hide 30+ lines of access code behind one line of a method call
	/// to make searching or authenticating to the LDAP very easy.
	/// </summary>
	/// <remarks>
	/// The BYU LDAP is a user accounts database that tracks all the NetIDs 
	/// and passwords that are allowed to log into Route Y, among other things.
	/// By accessing the LDAP, one can look up contact information for a user
	/// based on his/her NetID.  One can also authenticate against the LDAP
	/// to verify a user's identity using his Route Y NetID and password.
	/// </remarks>
	public class Authentication
	{
		/// <summary>
		/// Verifies the specified NetID and password combination against the BYU LDAP.
		/// </summary>
		/// <param name="NetID">
		/// The NetID of the user trying to authenticate to the system.  This will be the same
		/// NetID that he/she uses to log into Route Y.
		/// </param>
		/// <param name="Password">
		/// The password of the user trying to authenticate to the system.  This will be the same
		/// password that he/she uses to log into Route Y.
		/// </param>
		/// <returns>
		/// If the authentication was successful, a NetIDDetail object is returned with
		/// attributes that describe the successfully authenticated user, 
		/// using data that came out of the LDAP.  
		/// If the authentication failed, null is returned.
		/// </returns>
		/// <remarks>
		/// Although the LDAP conversation with the server is done in clear text, the password
		/// is encrypted first, and is not vulnerable to sniffing.
		/// </remarks>
		public static NetIDDetail Authenticate(string NetID, string Password)
		{
			if( NetID == null || NetID.Length == 0 ) throw new ArgumentNullException("NetID");
			if( Password == null || Password.Length == 0 ) throw new ArgumentNullException("Password");

			DirectoryEntry de = Lookup.getBind(NetID, Password);
			try 
			{
				DirectorySearcher ds = Lookup.getDirectorySearcher(de, NetID);

				// This next line throws an exception if the credentials are bad
				SearchResult sr = ds.FindOne();
				Debug.Assert( sr != null ); // the user just authenticated, so it better be found

				return new NetIDDetail( sr.GetDirectoryEntry() );
			} 
			catch
			{
				// An exception is thrown when the authentication fails for either a
				// bad username or bad password.
				return null; 
			}
		}
		/// <summary>
		/// Verifies the specified NetID and password combination against the BYU LDAP.
		/// </summary>
		/// <param name="NetID">
		/// The NetID of the user trying to authenticate to the system.  This will be the same
		/// NetID that he/she uses to log into Route Y.
		/// </param>
		/// <param name="Password">
		/// The password of the user trying to authenticate to the system.  This will be the same
		/// password that he/she uses to log into Route Y.
		/// </param>
		public static bool IsValidLogin(string NetID, string Password)
		{
			return Authenticate(NetID, Password) != null;
		}
	}
}
