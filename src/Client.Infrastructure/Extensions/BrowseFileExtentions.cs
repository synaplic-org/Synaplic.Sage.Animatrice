using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Client.Infrastructure.Extensions
{
    public static class BrowseFileExtentions
    {
        public static async Task<byte[]> GetAllBytes(this IBrowserFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.OpenReadStream(10485761).CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<Stream> GetStream(this IBrowserFile formFile)
        {
            var memoryStream = new MemoryStream();
            await formFile.OpenReadStream(10485761).CopyToAsync(memoryStream);
            return memoryStream ;
        
        }

        public static async Task<List<string>> GetAllLines(this IBrowserFile formFile, Encoding encoding )
        {
            List<string> rows = new List<string>();
            using (var memoryStream = new MemoryStream())
            {
                await formFile.OpenReadStream(10485761).CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Rewind!
                using (var reader = new StreamReader(memoryStream, encoding))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        rows.Add(line);
                    }
                }
            }
            return rows;
        }

    }
}
