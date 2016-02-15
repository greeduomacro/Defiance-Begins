using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Server;
using Server.Accounting;

namespace Server.Misc
{
	public class Email
	{
		/* In order to support emailing, fill in EmailServer:
		 * Example:
		 *  public static readonly string EmailServer = "mail.domain.com";
		 *
		 * If you want to add crash reporting emailing, fill in CrashAddresses:
		 * Example:
		 *  public static readonly string CrashAddresses = "first@email.here;second@email.here;third@email.here";
		 *
		 * If you want to add speech log page emailing, fill in SpeechLogPageAddresses:
		 * Example:
		 *  public static readonly string SpeechLogPageAddresses = "first@email.here;second@email.here;third@email.here";
		 */

		public static readonly string EmailServer = "mail.defiancebegins.com";
		public static readonly int Port = 25;
		public static readonly bool EnableSSL = false;

		public static readonly string CrashAddresses = "crashes@defiancebegins.com";
		public static readonly string SpeechLogPageAddresses = "speech@defiancebegins.com";
		private static readonly NetworkCredential m_SmtpCredentials = new NetworkCredential( "webteam+defiancebegins.com", "auth54123" );

		private static Regex _pattern = new Regex( @"^[a-z0-9.+_-]+@([a-z0-9-]+.)+[a-z]+$", RegexOptions.IgnoreCase );

		public static bool IsAvailable( string address )
		{
			address = address.Trim().ToLower();

			foreach ( Account acct in Accounts.GetAccounts() )
				if ( acct.AccessLevel == AccessLevel.Player && acct.Email == address )
					return false;

			return true;
		}

		public static bool IsValid( string address )
		{
			if ( address == null || address.Length > 320 )
				return false;

			return _pattern.IsMatch( address );
		}

		private static SmtpClient _Client;

		public static void Configure()
		{
			if ( !String.IsNullOrEmpty( EmailServer ) )
			{
				_Client = new SmtpClient( EmailServer, Port );
				_Client.UseDefaultCredentials = false;
				_Client.EnableSsl = EnableSSL;
				_Client.Timeout = 30000;

				ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
			}
		}

		public static bool Send( MailMessage message )
		{
			return Send( message, m_SmtpCredentials );
		}

		public static bool Send( MailMessage message, ICredentialsByHost credentials )
		{
			try
			{
				lock ( _Client )
				{
					//If this is not specified, we would like to keep it that way.
					_Client.Credentials = credentials;
					_Client.Send( message );
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "Email Send: Failure: {0}", e );
				return false;
			}

			return true;
		}

		public static void AsyncSend( MailMessage message )
		{
			AsyncSend( message, m_SmtpCredentials );
		}

		public static void AsyncSend( MailMessage message, ICredentialsByHost credentials )
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( SendCallback ), new object[]{ message, credentials } );
		}

		private static void SendCallback( object state )
		{
			object[] states = (object[])state;

			MailMessage message = states[0] as MailMessage;
			ICredentialsByHost credentials = states[1] as ICredentialsByHost;

			if ( !Send( message, credentials ) )
			//	Console.WriteLine( "Sent e-mail '{0}' to '{1}'.", message.Subject, message.To );
			//else
				Console.WriteLine( "Failure sending e-mail '{0}' to '{1}'.", message.Subject, message.To );
		}
	}
}