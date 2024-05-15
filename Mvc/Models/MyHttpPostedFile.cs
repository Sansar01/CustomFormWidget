using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sitefinity_Web.Mvc.Models
{
    public class MyHttpPostedFile : HttpPostedFileBase
    {
        private readonly Stream _stream;
        private readonly string _contentType;
        private readonly string _fileName;

        public MyHttpPostedFile(Stream stream, string contentType, string fileName)
        {
            _stream = stream;
            _contentType = contentType;
            _fileName = fileName;
        }

        public override int ContentLength => (int)_stream.Length;

        public override string ContentType => _contentType;

        public override string FileName => _fileName;

        public override Stream InputStream => _stream;

        public override void SaveAs(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            {
                _stream.CopyTo(fileStream);
            }
        }
    }
}