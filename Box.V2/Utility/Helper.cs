﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Box.V2.Utility
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Convert DateTime to unix timestamp.
        /// </summary>
        /// <param name="date">DateTime object.</param>
        /// <returns>unix timestamp.</returns>
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Encode string to base64
        /// </summary>
        /// <param name="plainText"> the string to be encoded.</param>
        /// <returns>base64 encoded string.</returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Parses a URL and returns query string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1); // +1 for skipping '?'
            var pairs = query.Split('&');
            return pairs
                .Select(o => o.Split('='))
                .Where(items => items.Count() == 2)
                .ToDictionary(pair => Uri.UnescapeDataString(pair[ 0 ]),
                    pair => Uri.UnescapeDataString(pair[ 1 ]));
        }

        /// <summary>
        /// Retuns the number of parts to be uploaded in Session
        /// </summary>
        /// <param name="totalSize"></param>
        /// <param name="partSize"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetNumberOfParts(long totalSize, long partSize)
        {
            if (partSize == 0)
                throw new Exception("Part Size cannot be 0");
            int numberOfParts = 1;
            if (partSize != totalSize)
            {
                numberOfParts = Convert.ToInt32(totalSize / partSize);
                numberOfParts += 1;
            }
            return numberOfParts;
        }

        /// <summary>
        /// Return a Random (probably unique) string.
        /// </summary>
        /// <param name="length">Lenght of the string returned.</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            Random random = new Random();
            byte[] buffer = new byte[length / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (length % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        /// <summary>
        /// Returns part of the file starting at offset.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="partSize"></param>
        /// <param name="partOffset"></param>
        /// <returns></returns>
        public static Stream GetFilePart(Stream stream, long partSize, long partOffset)
        {
            MemoryStream partStream = new MemoryStream();
            int byteRead;
            stream.Position = partOffset;
            for (int i = 0; i < partSize && (byteRead = stream.ReadByte()) != -1; i++)
            {
                partStream.WriteByte((byte)byteRead);
            }
            return partStream;
        }
    }
}