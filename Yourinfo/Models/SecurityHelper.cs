using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Yourinfo.Models
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            // Generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Specify the number of iterations for the operation
            int iterations = 10000;

            // Derive a 256-bit subkey (use HMACSHA1 with 256-bit key)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iterations,
                numBytesRequested: 256 / 8));

            // Combine salt and hashed password for storage
            return $"{iterations}.{Convert.ToBase64String(salt)}.{hashed}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Extract the parameters from the hashed password
                string[] parts = hashedPassword.Split('.', 3);
                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hashed = Convert.FromBase64String(parts[2]);

                // Compute the hash of the provided password and compare it with the stored hash
                byte[] testHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations,
                    numBytesRequested: 256 / 8);

                return CryptographicOperations.FixedTimeEquals(hashed, testHash);
            }
            catch (Exception)
            {
                return false; // In case of invalid format or other exception
            }
        }
    }
}
