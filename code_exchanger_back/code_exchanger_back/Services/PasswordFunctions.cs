using System.Security.Cryptography;

namespace code_exchanger_back.Services
{
    public static class PasswordFunctions
    {
        public static bool CheckString(string s)
        {
            if (s == null) return true;
            bool ok = true;
            foreach (char c in s)
                if (!(c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9'))
                {
                    ok = false;
                    break;
                }
            return ok;
        }

        public static string GetHash(string s)
        {
            if (s == null) return null;
            SHA512 sha512 = SHA512.Create();
            byte[] input = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] hash = sha512.ComputeHash(input);
            System.Text.StringBuilder sb = new System.Text.StringBuilder("");
            foreach (byte b in hash) sb.Append(b.ToString());
            return sb.ToString();
        }

        public static bool CheckPasswords(string correct, string possible)
        {
            if (correct == "") correct = null;
            if (possible == "") possible = null;
            return (correct == possible);
        }
    }
}
