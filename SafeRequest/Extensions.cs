using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SafeRequest {

    public static class Extensions {

        // https://codereview.stackexchange.com/a/107864
        public static  byte[] ToByteArray(this SecureString secureString, Encoding encoding = null) {
            if (secureString == null) 
                throw new ArgumentNullException(nameof(secureString));
            encoding = encoding ?? Encoding.UTF8;
            IntPtr unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return encoding.GetBytes(Marshal.PtrToStringUni(unmanagedString));
            } finally {
                if (unmanagedString != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(unmanagedString);
            }
        }

    }

}
