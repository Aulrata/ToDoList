using CsvHelper;
using CsvHelper.Configuration;
using System.Collections;
using System.Globalization;
using System.Text;

namespace ToDoList.Domain.Utils
{
    public class CsvBaseService<T>
    {
        private readonly CsvConfiguration _csvConfiguration;

        public CsvBaseService()
        {
            _csvConfiguration = GetConfiguration(); 
        }

        public byte[] UploadFiles(IEnumerable data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream)) 
            using (var csvWriter = new CsvWriter(streamWriter, _csvConfiguration))
            {
                csvWriter.WriteRecords(data);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }


        }

        private CsvConfiguration GetConfiguration()
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Encoding = Encoding.UTF8,
                NewLine = "\r\n",
            };
        }
    }
}
