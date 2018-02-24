namespace Common.Extensions
{
    using System.IO;

    public static class StringExtensions
    {
        /// <summary>
        /// Obtains stream from string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stream ToStream(this string value)
        {
            var stream = new MemoryStream();

            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            return stream;
        }
    }
}