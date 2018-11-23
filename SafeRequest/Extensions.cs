using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SafeRequest {

    public static class Extensions {

        // https://stackoverflow.com/a/25190648/5699643
        public static T Process<T>(this SecureString src, Func<byte[], T> func) {
            IntPtr bstr = IntPtr.Zero;
            byte[] workArray = null;
            GCHandle handle = GCHandle.Alloc(workArray, GCHandleType.Pinned);
            try {
                bstr = Marshal.SecureStringToBSTR(src);
                unsafe {
                    byte* bstrBytes = (byte*)bstr;
                    workArray = new byte[src.Length * 2];
                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = *bstrBytes++;
                }
                return func(workArray);
            } finally {
                if (workArray != null)
                    for (int i = 0; i < workArray.Length; i++)
                        workArray[i] = 0;
                handle.Free();
                if (bstr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bstr);
            }
        } 

        public static SecureString ToSecureString(this string str) {
            SecureString secure = new SecureString();
            foreach (char c in str)
                secure.AppendChar(c);
            return secure;
        }

    }

}
