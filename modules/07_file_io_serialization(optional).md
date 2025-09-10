# Module 7 — File I/O and Serialization (Data Persistence)

## Overview

File I/O (Input/Output) and serialization are essential for data persistence in C# applications. File I/O allows you to read from and write to files, while serialization converts objects to formats that can be stored or transmitted. This module covers working with different file formats, implementing robust file operations, and using various serialization techniques to save and load application data.

## Topics Covered

- File and directory operations
- Reading and writing text files
- Working with streams
- Binary file operations
- JSON serialization with System.Text.Json
- XML serialization
- Binary serialization
- File system best practices
- Asynchronous file operations
- Error handling for file operations
- File watching and monitoring

## File and Directory Operations

### Basic File Operations

```csharp
using System;
using System.IO;

class FileOperations
{
    public void DemonstrateFileOperations()
    {
        string filePath = "example.txt";
        string directoryPath = "MyFolder";

        // Check if file exists
        if (File.Exists(filePath))
        {
            Console.WriteLine("File exists");
        }

        // Create directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Get file information
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Console.WriteLine($"File size: {fileInfo.Length} bytes");
            Console.WriteLine($"Created: {fileInfo.CreationTime}");
            Console.WriteLine($"Modified: {fileInfo.LastWriteTime}");
        }

        // List files in directory
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                Console.WriteLine($"File: {Path.GetFileName(file)}");
            }
        }

        // Copy, move, and delete files
        string sourceFile = "source.txt";
        string destFile = "destination.txt";

        if (File.Exists(sourceFile))
        {
            File.Copy(sourceFile, destFile, overwrite: true);
            File.Move(destFile, Path.Combine(directoryPath, "moved.txt"));
        }

        // Clean up
        if (File.Exists(Path.Combine(directoryPath, "moved.txt")))
        {
            File.Delete(Path.Combine(directoryPath, "moved.txt"));
        }

        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath);
        }
    }
}
```

### Path Operations

```csharp
public class PathOperations
{
    public void DemonstratePathOperations()
    {
        string path = @"C:\Users\John\Documents\file.txt";

        // Extract components
        Console.WriteLine($"Full path: {path}");
        Console.WriteLine($"Directory: {Path.GetDirectoryName(path)}");
        Console.WriteLine($"File name: {Path.GetFileName(path)}");
        Console.WriteLine($"File name without extension: {Path.GetFileNameWithoutExtension(path)}");
        Console.WriteLine($"Extension: {Path.GetExtension(path)}");

        // Path manipulation
        string newPath = Path.Combine("C:", "Users", "John", "Documents", "newfile.txt");
        Console.WriteLine($"Combined path: {newPath}");

        // Create valid file names
        string invalidFileName = "file:with*invalid?chars";
        string validFileName = MakeValidFileName(invalidFileName);
        Console.WriteLine($"Valid file name: {validFileName}");

        // Temporary files
        string tempFile = Path.GetTempFileName();
        Console.WriteLine($"Temporary file: {tempFile}");

        string tempPath = Path.GetTempPath();
        Console.WriteLine($"Temp directory: {tempPath}");
    }

    private string MakeValidFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }
}
```

## Reading and Writing Text Files

### Synchronous Text File Operations

```csharp
public class TextFileOperations
{
    public void WriteAllTextExample()
    {
        string content = "Hello, World!\nThis is a text file.\nLine 3";
        File.WriteAllText("example.txt", content);
    }

    public void ReadAllTextExample()
    {
        if (File.Exists("example.txt"))
        {
            string content = File.ReadAllText("example.txt");
            Console.WriteLine("File content:");
            Console.WriteLine(content);
        }
    }

    public void WriteAllLinesExample()
    {
        string[] lines = {
            "First line",
            "Second line",
            "Third line"
        };
        File.WriteAllLines("lines.txt", lines);
    }

    public void ReadAllLinesExample()
    {
        if (File.Exists("lines.txt"))
        {
            string[] lines = File.ReadAllLines("lines.txt");
            Console.WriteLine("Lines:");
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {lines[i]}");
            }
        }
    }

    public void AppendTextExample()
    {
        File.AppendAllText("log.txt", $"{DateTime.Now}: Application started\n");
        File.AppendAllText("log.txt", $"{DateTime.Now}: Processing data\n");
        File.AppendAllText("log.txt", $"{DateTime.Now}: Application finished\n");
    }

    public void ReadWriteWithEncoding()
    {
        // Write with specific encoding
        string content = "Hello, 世界!";
        File.WriteAllText("unicode.txt", content, Encoding.UTF8);

        // Read with specific encoding
        string readContent = File.ReadAllText("unicode.txt", Encoding.UTF8);
        Console.WriteLine(readContent);
    }
}
```

### Line-by-Line Processing

```csharp
public class LineByLineProcessing
{
    public void ProcessLargeFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found");
            return;
        }

        int lineCount = 0;
        int wordCount = 0;
        int charCount = 0;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineCount++;

                // Count words (simple approach)
                string[] words = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                wordCount += words.Length;

                // Count characters
                charCount += line.Length;

                // Process each line
                if (line.Contains("ERROR"))
                {
                    Console.WriteLine($"Error found on line {lineCount}: {line}");
                }
            }
        }

        Console.WriteLine($"Lines: {lineCount}, Words: {wordCount}, Characters: {charCount}");
    }

    public void WriteLinesIncrementally()
    {
        using (StreamWriter writer = new StreamWriter("output.txt"))
        {
            for (int i = 1; i <= 100; i++)
            {
                writer.WriteLine($"Line {i}: {DateTime.Now}");
                Thread.Sleep(10); // Simulate processing time
            }
        }
    }
}
```

## Working with Streams

### Stream Fundamentals

```csharp
public class StreamOperations
{
    public void DemonstrateStreams()
    {
        string sourceFile = "source.txt";
        string destFile = "destination.txt";

        // Create source file
        File.WriteAllText(sourceFile, "This is the content to copy.");

        // Copy using streams
        using (FileStream sourceStream = File.OpenRead(sourceFile))
        using (FileStream destStream = File.Create(destFile))
        {
            sourceStream.CopyTo(destStream);
        }

        // Read from stream
        using (FileStream stream = File.OpenRead(destFile))
        using (StreamReader reader = new StreamReader(stream))
        {
            string content = reader.ReadToEnd();
            Console.WriteLine($"Copied content: {content}");
        }
    }

    public void BinaryStreamOperations()
    {
        string fileName = "data.bin";

        // Write binary data
        using (FileStream stream = File.Create(fileName))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(42);           // int
            writer.Write(3.14159);      // double
            writer.Write("Hello");      // string
            writer.Write(true);         // bool
        }

        // Read binary data
        using (FileStream stream = File.OpenRead(fileName))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            int intValue = reader.ReadInt32();
            double doubleValue = reader.ReadDouble();
            string stringValue = reader.ReadString();
            bool boolValue = reader.ReadBoolean();

            Console.WriteLine($"Int: {intValue}, Double: {doubleValue}, String: {stringValue}, Bool: {boolValue}");
        }
    }
}
```

### Memory Streams

```csharp
public class MemoryStreamExample
{
    public void DemonstrateMemoryStream()
    {
        // Create data in memory
        using (MemoryStream memoryStream = new MemoryStream())
        using (StreamWriter writer = new StreamWriter(memoryStream))
        {
            writer.WriteLine("Line 1");
            writer.WriteLine("Line 2");
            writer.WriteLine("Line 3");
            writer.Flush();

            // Convert to byte array
            byte[] data = memoryStream.ToArray();

            // Read from memory
            memoryStream.Position = 0; // Reset position
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine("Memory content:");
                Console.WriteLine(content);
            }

            // Write to file
            File.WriteAllBytes("from_memory.txt", data);
        }
    }
}
```

## JSON Serialization

### Basic JSON Operations

```csharp
using System.Text.Json;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public List<string> Hobbies { get; set; } = new List<string>();
}

public class JsonSerializationExample
{
    public void SerializeToJson()
    {
        var person = new Person
        {
            Name = "Alice Johnson",
            Age = 30,
            Email = "alice@example.com",
            Hobbies = new List<string> { "Reading", "Hiking", "Photography" }
        };

        // Serialize to JSON
        string jsonString = JsonSerializer.Serialize(person, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine("JSON Output:");
        Console.WriteLine(jsonString);

        // Save to file
        File.WriteAllText("person.json", jsonString);
    }

    public void DeserializeFromJson()
    {
        string jsonString = File.ReadAllText("person.json");

        // Deserialize from JSON
        Person person = JsonSerializer.Deserialize<Person>(jsonString);

        Console.WriteLine("Deserialized Person:");
        Console.WriteLine($"Name: {person.Name}");
        Console.WriteLine($"Age: {person.Age}");
        Console.WriteLine($"Email: {person.Email}");
        Console.WriteLine("Hobbies:");
        foreach (string hobby in person.Hobbies)
        {
            Console.WriteLine($"  - {hobby}");
        }
    }

    public void SerializeComplexObject()
    {
        var company = new
        {
            Name = "Tech Corp",
            Founded = 2010,
            Employees = new[]
            {
                new { Name = "Alice", Department = "Engineering" },
                new { Name = "Bob", Department = "Sales" },
                new { Name = "Charlie", Department = "HR" }
            }
        };

        string json = JsonSerializer.Serialize(company, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine("Company JSON:");
        Console.WriteLine(json);
    }
}
```

### Advanced JSON Features

```csharp
using System.Text.Json.Serialization;

public class AdvancedJsonExample
{
    public void CustomSerialization()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var data = new
        {
            FullName = "John Doe",
            BirthDate = new DateTime(1990, 5, 15),
            Salary = 75000.50m,
            IsActive = true,
            Notes = (string)null // Will be ignored due to WhenWritingNull
        };

        string json = JsonSerializer.Serialize(data, options);
        Console.WriteLine("Custom JSON:");
        Console.WriteLine(json);
    }

    public void HandleCircularReferences()
    {
        var parent = new Parent { Name = "Parent" };
        var child = new Child { Name = "Child", Parent = parent };
        parent.Child = child;

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };

        // This would normally throw due to circular reference
        // but ReferenceHandler.Preserve handles it
        string json = JsonSerializer.Serialize(parent, options);
        Console.WriteLine("Circular reference JSON:");
        Console.WriteLine(json);
    }
}

public class Parent
{
    public string Name { get; set; }
    public Child Child { get; set; }
}

public class Child
{
    public string Name { get; set; }
    public Parent Parent { get; set; }
}
```

## XML Serialization

### Basic XML Operations

```csharp
using System.Xml.Serialization;

[XmlRoot("Person")]
public class XmlPerson
{
    [XmlElement("FullName")]
    public string Name { get; set; }

    [XmlElement("YearsOld")]
    public int Age { get; set; }

    [XmlElement("EmailAddress")]
    public string Email { get; set; }

    [XmlArray("Hobbies")]
    [XmlArrayItem("Hobby")]
    public List<string> Hobbies { get; set; } = new List<string>();
}

public class XmlSerializationExample
{
    public void SerializeToXml()
    {
        var person = new XmlPerson
        {
            Name = "Alice Johnson",
            Age = 30,
            Email = "alice@example.com",
            Hobbies = new List<string> { "Reading", "Hiking", "Photography" }
        };

        XmlSerializer serializer = new XmlSerializer(typeof(XmlPerson));

        using (StreamWriter writer = new StreamWriter("person.xml"))
        {
            serializer.Serialize(writer, person);
        }

        Console.WriteLine("XML file created successfully");
    }

    public void DeserializeFromXml()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlPerson));

        using (StreamReader reader = new StreamReader("person.xml"))
        {
            XmlPerson person = (XmlPerson)serializer.Deserialize(reader);

            Console.WriteLine("Deserialized from XML:");
            Console.WriteLine($"Name: {person.Name}");
            Console.WriteLine($"Age: {person.Age}");
            Console.WriteLine($"Email: {person.Email}");
        }
    }
}
```

## Asynchronous File Operations

### Async File I/O

```csharp
public class AsyncFileOperations
{
    public async Task WriteFileAsync(string filePath, string content)
    {
        await File.WriteAllTextAsync(filePath, content);
        Console.WriteLine("File written asynchronously");
    }

    public async Task<string> ReadFileAsync(string filePath)
    {
        string content = await File.ReadAllTextAsync(filePath);
        Console.WriteLine("File read asynchronously");
        return content;
    }

    public async Task ProcessLargeFileAsync(string inputPath, string outputPath)
    {
        using (StreamReader reader = new StreamReader(inputPath))
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            string line;
            int lineCount = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineCount++;

                // Process line (e.g., convert to uppercase)
                string processedLine = line.ToUpper();

                await writer.WriteLineAsync(processedLine);

                // Report progress every 1000 lines
                if (lineCount % 1000 == 0)
                {
                    Console.WriteLine($"Processed {lineCount} lines...");
                }
            }

            Console.WriteLine($"Processing complete. Total lines: {lineCount}");
        }
    }

    public async Task CopyFileAsync(string sourcePath, string destPath)
    {
        using (FileStream sourceStream = File.OpenRead(sourcePath))
        using (FileStream destStream = File.Create(destPath))
        {
            await sourceStream.CopyToAsync(destStream);
        }

        Console.WriteLine("File copied asynchronously");
    }
}
```

## File System Watching

### Monitoring File Changes

```csharp
public class FileWatcherExample
{
    private FileSystemWatcher watcher;

    public void StartWatching(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Directory does not exist");
            return;
        }

        watcher = new FileSystemWatcher
        {
            Path = directoryPath,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            Filter = "*.txt",
            EnableRaisingEvents = true
        };

        // Subscribe to events
        watcher.Changed += OnFileChanged;
        watcher.Created += OnFileCreated;
        watcher.Deleted += OnFileDeleted;
        watcher.Renamed += OnFileRenamed;

        Console.WriteLine($"Watching directory: {directoryPath}");
        Console.WriteLine("Press any key to stop watching...");
        Console.ReadKey();

        // Clean up
        watcher.Dispose();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File changed: {e.FullPath}");
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File created: {e.FullPath}");
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File deleted: {e.FullPath}");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"File renamed: {e.OldFullPath} -> {e.FullPath}");
    }
}
```

## Best Practices

### 1. Error Handling

```csharp
public class RobustFileOperations
{
    public bool TryReadFile(string filePath, out string content)
    {
        content = null;

        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist");
                return false;
            }

            content = File.ReadAllText(filePath);
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied: {ex.Message}");
            return false;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    public bool TryWriteFile(string filePath, string content)
    {
        try
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content);
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied: {ex.Message}");
            return false;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }
}
```

### 2. Resource Management

```csharp
public class ResourceManagement
{
    public void ProcessFileSafely(string filePath)
    {
        StreamReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            Console.WriteLine($"File content length: {content.Length}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file: {ex.Message}");
        }
        finally
        {
            // Ensure resources are cleaned up
            reader?.Dispose();
        }
    }

    // Using statement (preferred approach)
    public void ProcessFileWithUsing(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine($"File content length: {content.Length}");
            }
            // reader.Dispose() is called automatically
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file: {ex.Message}");
        }
    }
}
```

### 3. File Path Security

```csharp
public class SecureFileOperations
{
    public bool IsValidFilePath(string filePath)
    {
        try
        {
            // Check for invalid characters
            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;

            // Check for directory traversal attempts
            if (filePath.Contains("..") || filePath.Contains("\\..") || filePath.Contains("../"))
                return false;

            // Ensure path is absolute and within allowed directory
            string fullPath = Path.GetFullPath(filePath);
            string allowedDirectory = Path.GetFullPath("AllowedFiles");

            if (!fullPath.StartsWith(allowedDirectory, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string SanitizeFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        // Remove other potentially dangerous characters
        fileName = fileName.Replace(".", "").Replace("/", "").Replace("\\", "");

        // Ensure filename is not too long
        if (fileName.Length > 255)
            fileName = fileName.Substring(0, 255);

        return fileName;
    }
}
```

## Comprehensive Examples

### Example 1: Configuration File Manager

```csharp
using System.Text.Json;

public class ConfigurationManager
{
    private readonly string configFilePath;
    private Dictionary<string, object> settings;

    public ConfigurationManager(string configFile = "config.json")
    {
        configFilePath = configFile;
        LoadConfiguration();
    }

    private void LoadConfiguration()
    {
        try
        {
            if (File.Exists(configFilePath))
            {
                string jsonContent = File.ReadAllText(configFilePath);
                settings = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
            }
            else
            {
                // Create default configuration
                settings = new Dictionary<string, object>
                {
                    ["AppName"] = "MyApplication",
                    ["Version"] = "1.0.0",
                    ["MaxConnections"] = 100,
                    ["DebugMode"] = false,
                    ["LogLevel"] = "Info"
                };
                SaveConfiguration();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            settings = new Dictionary<string, object>();
        }
    }

    private void SaveConfiguration()
    {
        try
        {
            string jsonContent = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(configFilePath, jsonContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
        }
    }

    public T GetSetting<T>(string key, T defaultValue = default)
    {
        if (settings.TryGetValue(key, out object value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    public void SetSetting<T>(string key, T value)
    {
        settings[key] = value;
        SaveConfiguration();
    }

    public void PrintConfiguration()
    {
        Console.WriteLine("Current Configuration:");
        foreach (var setting in settings)
        {
            Console.WriteLine($"{setting.Key}: {setting.Value}");
        }
    }
}

class Program
{
    static void Main()
    {
        var config = new ConfigurationManager();

        // Read settings
        string appName = config.GetSetting<string>("AppName", "DefaultApp");
        int maxConnections = config.GetSetting<int>("MaxConnections", 50);
        bool debugMode = config.GetSetting<bool>("DebugMode", false);

        Console.WriteLine($"App: {appName}, Max Connections: {maxConnections}, Debug: {debugMode}");

        // Modify settings
        config.SetSetting("MaxConnections", 200);
        config.SetSetting("LastUpdated", DateTime.Now);

        config.PrintConfiguration();
    }
}
```

### Example 2: CSV File Processor

```csharp
using System.Globalization;

public class CsvProcessor
{
    public List<Dictionary<string, string>> ReadCsv(string filePath)
    {
        var records = new List<Dictionary<string, string>>();

        if (!File.Exists(filePath))
            throw new FileNotFoundException("CSV file not found", filePath);

        using (var reader = new StreamReader(filePath))
        {
            string headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
                return records;

            string[] headers = ParseCsvLine(headerLine);

            string dataLine;
            while ((dataLine = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(dataLine))
                    continue;

                string[] values = ParseCsvLine(dataLine);

                if (values.Length != headers.Length)
                {
                    Console.WriteLine($"Warning: Line has {values.Length} values but expected {headers.Length}");
                    continue;
                }

                var record = new Dictionary<string, string>();
                for (int i = 0; i < headers.Length; i++)
                {
                    record[headers[i]] = values[i];
                }
                records.Add(record);
            }
        }

        return records;
    }

    public void WriteCsv(string filePath, List<Dictionary<string, string>> records)
    {
        if (records == null || records.Count == 0)
            return;

        using (var writer = new StreamWriter(filePath))
        {
            // Write headers
            var headers = records[0].Keys;
            writer.WriteLine(string.Join(",", headers.Select(EscapeCsvValue)));

            // Write data
            foreach (var record in records)
            {
                var values = headers.Select(header => record.ContainsKey(header) ? record[header] : "");
                writer.WriteLine(string.Join(",", values.Select(EscapeCsvValue)));
            }
        }
    }

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote
                    currentField += '"';
                    i++; // Skip next quote
                }
                else
                {
                    // Toggle quote state
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        // Add the last field
        result.Add(currentField);

        return result.ToArray();
    }

    private string EscapeCsvValue(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        return value;
    }
}

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
}

class Program
{
    static void Main()
    {
        var processor = new CsvProcessor();

        // Create sample data
        var people = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { ["Name"] = "Alice", ["Age"] = "30", ["City"] = "New York" },
            new Dictionary<string, string> { ["Name"] = "Bob", ["Age"] = "25", ["City"] = "Los Angeles" },
            new Dictionary<string, string> { ["Name"] = "Charlie", ["Age"] = "35", ["City"] = "Chicago" }
        };

        // Write CSV
        processor.WriteCsv("people.csv", people);
        Console.WriteLine("CSV file created");

        // Read CSV
        var readData = processor.ReadCsv("people.csv");
        Console.WriteLine("Read data:");
        foreach (var record in readData)
        {
            Console.WriteLine($"{record["Name"]}, {record["Age"]}, {record["City"]}");
        }
    }
}
```

## Exercises

1. **File System Explorer**
   - Create a program that recursively explores directories
   - Display file sizes, types, and modification dates
   - Implement search functionality for files

2. **Data Import/Export Utility**
   - Create a utility that can import/export data in multiple formats (JSON, XML, CSV)
   - Implement data validation during import
   - Add support for large file processing

3. **Backup System**
   - Implement a file backup system with incremental backups
   - Add compression and encryption support
   - Create a restore functionality

4. **Log File Analyzer**
   - Create a program that analyzes log files
   - Extract error patterns and statistics
   - Generate reports on system health

5. **Configuration Management**
   - Build a hierarchical configuration system
   - Support multiple configuration sources (files, environment variables, command line)
   - Implement configuration validation and hot-reloading

6. **File Synchronization Tool**
   - Create a tool that synchronizes files between directories
   - Implement conflict resolution strategies
   - Add progress tracking and error recovery

File I/O and serialization are fundamental to data persistence in C# applications. By mastering these concepts, you can create applications that reliably store and retrieve data in various formats. Remember to always handle exceptions properly, use the `using` statement for resource management, and validate file paths for security!
