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

using HtmlAgilityPack;

namespace vmware_downloader;

internal class VMwareDownloader
{
    public string _coreUrl = "https://softwareupdate.vmware.com/cds/vmw-desktop/ws/";
    public string _originalCoreUrl;

    public VMwareDownloader()
    { 
        // Create backup of core URL since _coreUrl will change in setup process
        this._originalCoreUrl = _coreUrl;
    }

    /// <summary>
    /// The AskIndex function returns the index of the list containing the 
    /// a:href elements. This index is used to access the needed directory and append it 
    /// to this._coreUrl by using the wrapper function CoreUrlAddPath
    /// </summary>
    /// <param name="directory">a:href value accessed from elements list</param>
    public void CoreUrlAddPath(string directory)
    {
        if (!string.IsNullOrEmpty(directory) && !this._coreUrl.Contains(directory))
        {
            if (!directory.EndsWith('/'))
            {
                directory += "/";
            }

            this._coreUrl += directory;
        }
    }

    /// <summary>
    /// ListHrefValues prints all elements found by CollectHrefHTML(), format them and build a menu.
    /// </summary>
    /// <param name="elements">a:href elements</param>
    /// <param name="idx">Reference of the current menu index</param>
    public static void ListHrefValues(List<string> elements, ref int idx)
    {
        foreach (var element in elements)
        {
            // Iterate through a:href hits and print with pattern: <idx>) <entry>
            string entry = element.Replace('/', ' ');
            Console.WriteLine($"{idx}) {entry}");
            idx++;
        }
    }

    /// <summary>
    /// Read the settings from stdin and perform basic validation of the user defined input
    /// </summary>
    /// <param name="prompt">Input prompt string</param>
    /// <param name="idx">Menu maximum index to ensure the input is in valid range</param>
    /// <returns>User definde setting by index</returns>
    public static Int32 AskIndex(string prompt, int idx)
    {
        int choice;

        while (true)
        {
            Console.Write(prompt);

            try
            {
                // Get choice by index
                choice = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                continue;
            }

            if (choice < 0 || choice > idx)
            {
                Console.WriteLine("Invalid input!");
                continue;
            }

            break;
        }

        return choice;
    }

    /// <summary>
    /// Load the HTML structure of the target endpoint, filter the a:href 
    /// elements and store them to a list
    /// </summary>
    /// <returns>List containing all found attributes</returns>
    public List<string> CollectHrefHTML()
    {
        var versions = new List<string>();

        var hw = new HtmlWeb();
        HtmlDocument document = hw.Load(this._coreUrl);

        foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
        {
            HtmlAttribute attribute = link.Attributes["href"];

            // Skip first a:href element: ../
            if (!string.IsNullOrWhiteSpace(attribute.Value) && !attribute.Value.Equals("../"))
            {
                versions.Add(attribute.Value);
            }
        }

        return versions;
    }

    /// <summary>
    /// Use the full URL to get the filename after path build process
    /// </summary>
    /// <param name="url">Full download URL</param>
    /// <returns>Filename to download</returns>
    private static string GetFilenameToDownload(string url)
    { 
        var uri = new Uri(url);
        // Remove part before last delim
        return uri.Segments.Last().TrimEnd('/');
    }

    private static string HandleOutputDirectory()
    {
        string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        return outputDirectory;
    }

    /// <summary>
    /// Ensure all settings are correct and the file can be downloaded. If the user want to download, 
    /// A GET request will be send to the endpoint to get the response body containg the needed data for download. The file
    /// will be saved with its original name in the current working directory.
    /// </summary>
    /// <returns>The output file name or string.Empty if an error occured</returns>
    public string DownloadFile()
    {
        string outputDirectory = HandleOutputDirectory();
        string fullPath = this._coreUrl.Remove(this._coreUrl.Length - 1);
        string outputFile = GetFilenameToDownload(fullPath);
        string outputFilePath = Path.Combine(outputDirectory, outputFile);

        if (File.Exists(outputFilePath)) 
        { 
            File.Delete(outputFilePath);
        }

        Console.WriteLine($"Start the download for {outputFile}?");
        Console.WriteLine("n/N = back to start");
        Console.WriteLine("y/Y = download");

        while (true)
        {
            Console.Write("\n> ");

            char startDownload = Console.ReadKey().KeyChar;

            if (startDownload == 'y' || startDownload == 'Y')
            {
                Console.WriteLine("\n\nStarting download..");
                break;
            }
            else if (startDownload == 'n' || startDownload == 'N')
            {
                Console.WriteLine();
                return string.Empty;
            }
            else
            {
                Console.WriteLine("Invalid input!");
            }
        }

        try
        {
            using var client = new HttpClient();
            // Fetch the response body
            using var stream = client.GetStreamAsync(fullPath);
            // Initialize output file stream
            using var fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate);

            // Write response to output file stream
            stream.Result.CopyTo(fileStream);
        }
        catch (AggregateException e)
        {
            Console.WriteLine(e.Message);
            return string.Empty;
        }

        return outputFilePath;
    }
}
