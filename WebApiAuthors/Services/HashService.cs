using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using WebApiAuthors.DTOs;
using WebApiAuthors.Migrations;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiAuthors.Services
{
    public class HashService
    {
        public HashResultDTO Hash(string planeText)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }
            return Hash(planeText, sal);
        }

        public HashResultDTO Hash(string planeText, byte[] sal)
        {
            var derivedKey = KeyDerivation.Pbkdf2(
                password: planeText,
                salt: sal,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);
            var hash = Convert.ToBase64String(derivedKey);
            return new HashResultDTO()
            {
                Hash = hash,
                Sal = sal,
            };
        }
    }
}
