using System;
using System.IO;
using System.Linq;
using Docs.Routing;

namespace Docs
{
    public static class AfterPublish
    {
        public static void Run(string publishPath)
        {
            var entryFile = "404.html";
            var sourceFilePath = Path.Combine(publishPath, entryFile);
            Console.WriteLine($"Source file '{sourceFilePath}'");

            foreach (var webPath in DocumentRouteProvider.DocumentRoutes.Select(documentRoute => documentRoute.WebPath))
            {
                var fileName = webPath;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "index";
                }

                var destinationFilePath = Path.Combine(publishPath, fileName + ".html");
                Console.WriteLine($"Destination file '{destinationFilePath}'");
                File.Copy(sourceFilePath, destinationFilePath, true);
            }
        }
    }
}
