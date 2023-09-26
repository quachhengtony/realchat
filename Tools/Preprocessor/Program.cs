using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

string inputDirectory = @"D:\Projects\Realchat.Data\Raw"; // Replace with your directory path
string outputDirectory = @"D:\Projects\Realchat.Data\Processed"; // Replace with your output directory path

string[] documentExtensions = { ".doc", ".docx" };

if (!Directory.Exists(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

foreach (string file in Directory.EnumerateFiles(inputDirectory, "*.*")
                                 .Where(f => documentExtensions.Contains(Path.GetExtension(f))))
{
    ProcessDocument(file, outputDirectory);
}

Console.WriteLine("Processing complete.");

static void ProcessDocument(string filePath, string outputDirectory)
{
    using WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false);
    var paragraphs = doc.MainDocumentPart.Document.Body.Elements<Paragraph>();

    var words = paragraphs.SelectMany(paragraph => paragraph.InnerText.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)).ToList();

    int chunkSize = 100;
    int chunkCount = (int)Math.Ceiling((double)words.Count / chunkSize);

    for (int i = 0; i < chunkCount; i++)
    {
        int startIndex = i * chunkSize;
        int endIndex = Math.Min(startIndex + chunkSize, words.Count);
        List<string> chunk = words.GetRange(startIndex, endIndex - startIndex);

        string chunkText = string.Join(" ", chunk); // Join words with spaces

        string outputFilePath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(filePath)}_{i + 1}.txt");

        File.WriteAllText(outputFilePath, chunkText); // Use WriteAllText to write the entire chunk as a single string
    }
}