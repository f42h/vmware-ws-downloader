/*
 * MIT License
 * 
 * Copyright (c) 2025 f42h
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using vmware_downloader;

class EntryPoint()
{
    public static void Main()
    {
        Console.WriteLine("VMWare Workstation Downloader");
        Console.WriteLine();

        int idx = 0;
        var d = new VMwareDownloader();

        List<string> prompts = [
           "\nVersion> ",
           "\nNr> ",
           "\nOS> ",
           "\nApp> ",
           "\nPackage> "
        ];

        Console.CancelKeyPress += (sender, e) => {
            Console.WriteLine("Ctrl+C pressed..");
            Environment.Exit(0); 
        };

        while (true)
        {
            // Reset core URL by overwriting its value with the backup value
            d._coreUrl = d._originalCoreUrl;

            foreach (var prompt in prompts)
            {
                Console.WriteLine("#######################################");
                Console.WriteLine();
               
                // Collect all a:href elements to a list
                var collect = d.CollectHrefHTML();
                // Build output and show menu
                VMwareDownloader.ListHrefValues(collect, ref idx);

                // Read the settings from the user
                int call = VMwareDownloader.AskIndex(prompt, idx);
                d.CoreUrlAddPath(collect[call]); // Extend the current url by setting
                idx = 0; // Reset menu index

                Console.WriteLine();
            }

            // Try to setup and download the target file
            string outputFile = d.DownloadFile();

            if (string.IsNullOrEmpty(outputFile)) 
            {
                // Could not safe file..
                continue;
            }

            Console.WriteLine($"\nFile saved to: {outputFile}");
            Console.WriteLine("\nPress any key to continue..");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
};