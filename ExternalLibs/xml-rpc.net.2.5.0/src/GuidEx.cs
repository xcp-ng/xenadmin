//==========================================================================================
//
//		OpenNETCF.GuidEx
//		Copyright (C) 2003-2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//		!!! A HUGE thank-you goes out to Casey Chesnut for supplying parts of this code !!!
//      !!! You can contact Casey at http://www.brains-n-brawn.com   
//
//==========================================================================================

#if(COMPACT_FRAMEWORK)

using System;
using System.Runtime.InteropServices;
//using OpenNETCF.Security.Cryptography;

// New for v1.3 - "The Guid to end all Guids" - Peter Foot
namespace OpenNETCF
{
	/// <summary>
	/// Helper class for generating a globally unique identifier (GUID).
	/// <para><b>Revised in v1.3</b></para>
	/// </summary>
	/// <seealso cref="System.Guid"/>
	public sealed class GuidEx
	{
		private GuidEx(){}

        #region New Guid
		/// <summary>
		/// Initializes a new instance of the <see cref="System.Guid"/> class.
		/// </summary>
		/// <returns>A new <see cref="System.Guid"/> object.</returns>
		/// <remarks>On CE.NET 4.1 and higher this method uses the CoCreateGuid API call.
		/// On CE 3.0 based Pocket PCs it uses the OpenNETCF.Security.Cryptography classes to generate a random Guid.</remarks>
		public static System.Guid NewGuid()
		{
			//cocreateguid supported on CE.NET 4.1 and above (4.0 not supported by .NETCF)
			if(System.Environment.OSVersion.Version.Major > 3)
			{
				return NewOleGuid();
			}
			else
			{
				//check if target has crypto API support
//				if(OpenNETCF.Security.Cryptography.NativeMethods.Context.IsCryptoApi)
//				{
//					return NewCryptoGuid();
//				}
//				else
				{
					//if not use random generator
					return NewRandomGuid();
				}
			}
		}
		#endregion


		#region Constants
		// constants that are used in the class
		private class Const
		{
			// guid variant types
			public enum GuidVariant
			{
				ReservedNCS = 0x00,
				Standard = 0x02,
				ReservedMicrosoft = 0x06,
				ReservedFuture = 0x07
			}

			// guid version types
			public enum GuidVersion
			{
				TimeBased = 0x01,
				Reserved = 0x02,
				NameBased = 0x03,
				Random = 0x04
			}

			// multiplex variant info
			public const int VariantByte = 8;
			public const int VariantByteMask = 0x3f;
			public const int VariantByteShift = 6;

			// multiplex version info
			public const int VersionByte = 7;
			public const int VersionByteMask = 0x0f;
			public const int VersionByteShift = 4;
		}
		#endregion

		#region Crypto
//		/// <summary>
//		/// Create a new Random Guid using Crypto APIs
//		/// </summary>
//		/// <returns></returns>
//		public static System.Guid NewCryptoGuid()
//		{
//			//create guid manually
//			byte[] guidbytes = new byte[16];
//
//			//use crypto apis to generate random bytes
//			OpenNETCF.Security.Cryptography.RNGCryptoServiceProvider rng = new OpenNETCF.Security.Cryptography.RNGCryptoServiceProvider();
//			rng.GetBytes(guidbytes);
//			
//			//set version etc	
//			MakeValidRandomGuid(guidbytes);
//
//			// create the new System.Guid object
//			return new System.Guid(guidbytes);
//		}
		#endregion

		#region Random
		/// <summary>
		/// Create a new Random Guid (For platforms without Crypto support).
		/// </summary>
		/// <returns></returns>
		public static System.Guid NewRandomGuid()
		{
			byte[] guidbytes = new byte[16];
      (new Random()).NextBytes(guidbytes);

			//set version etc
			MakeValidRandomGuid(guidbytes);
			
         
			// create the new System.Guid object
			return new System.Guid(guidbytes);
		}
		#endregion

		#region Helper Methods
		private static void MakeValidRandomGuid(byte[] guidbytes)
		{
			// set the variant
			guidbytes[Const.VariantByte] &= Const.VariantByteMask;
			guidbytes[Const.VariantByte] |= 
				((int)Const.GuidVariant.Standard << Const.VariantByteShift);

			// set the version
			guidbytes[Const.VersionByte] &= Const.VersionByteMask;
			guidbytes[Const.VersionByte] |= 
				((int)Const.GuidVersion.Random << Const.VersionByteShift);
		}
		#endregion

		#region Ticks
		/// <summary>
		/// Create a new <see cref="Guid"/> only using TickCount and bit shifting.
		/// </summary>
		public static System.Guid NewGuidTicks()
		{
			// Create a unique GUID
			long fileTime = DateTime.Now.ToUniversalTime().ToFileTime();
			UInt32 high32 = (UInt32)(fileTime >> 32)+0x146BF4;
			int tick = System.Environment.TickCount;
			byte[] guidBytes = new byte[8];

			// load the byte array with random bits
			Random rand = new Random((int)fileTime);
			rand.NextBytes(guidBytes);

			// use tick info in the middle of the array
			guidBytes[2] = (byte)(tick >> 24);
			guidBytes[3] = (byte)(tick >> 16);
			guidBytes[4] = (byte)(tick >> 8);
			guidBytes[5] = (byte)tick;

			// Construct a Guid with our data
			System.Guid guid = new System.Guid((int)fileTime, (short)high32, (short)((high32 | 0x10000000) >> 16), guidBytes);
			return guid;
		}
		#endregion

		#region Ole
		/// <summary>
		/// Create a new <see cref="Guid"/> using COM APIs
		/// </summary>
		/// <returns></returns>
		/// <remarks>Requires Windows CE.NET 4.1 or higher.</remarks>
		public static System.Guid NewOleGuid()
		{
			System.Guid val = System.Guid.Empty;

			int hresult = 0;
			hresult = CoCreateGuid(ref val);

			if(hresult != 0)
			{
				throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "Error creating new Guid");
			}

			return val;
		}
		[DllImport("ole32.dll", SetLastError=true)]
		private static extern int CoCreateGuid(ref System.Guid pguid);
		#endregion
	}

}

#endif