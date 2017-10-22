using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSFeatureProject.Extensions
{
    public static class FunctionExtensions
    {
        public static string AppendTimeStamp(this string fileName)
        {
            return string.Concat(
                DateTime.Now.ToString("ddMMyyyyHHmmssfff_"),
                Path.GetFileNameWithoutExtension(fileName),
                Path.GetExtension(fileName)
                );
        }
    }
}
