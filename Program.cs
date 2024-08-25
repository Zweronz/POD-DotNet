using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using POD;

public class Program
{
    public enum SerializationType
    {
        POD,
        Json,
        Binary
    };

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("no input file! run -help");
            return;
        }

        string? input = null, output = null;
        SerializationType? inputFormat = null, outputFormat = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-help":
                    Console.WriteLine(@"
                    -i  | input file
                    -o  | output file
                    -if | input format
                    -of | output format
                    
                    available formats are POD, Json, and Binary
                    
                    ");
                    break;
                    
                case "-i":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.WriteLine("no input file! run -help");
                    }
                    input = args[i];
                    break;

                case "-o":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.WriteLine("no output file! run -help");
                    }
                    output = args[i];
                    break;

                case "-if":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.WriteLine("no input format! run -help");
                    }
                    inputFormat = TryParse(args[i]);
                    break;

                case "-of":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.WriteLine("no output format! run -help");
                    }
                    outputFormat = TryParse(args[i]);
                    break;
            }
        }

        if (input == null)
        {
            Console.WriteLine("no input file! run -help");
        }

        if (output == null)
        {
            Console.WriteLine("no output file! run -help");
        }

        if (inputFormat == null)
        {
            Console.WriteLine("no input format! run -help");
        }

        if (outputFormat == null)
        {
            Console.WriteLine("no output format! run -help");
        }

        PodScene scene = new PodScene();

        switch (inputFormat)
        {
            case SerializationType.POD:
                new PodFile().Read(ref scene, input!);
                break;

            case SerializationType.Json:
                scene = JsonConvert.DeserializeObject<PodScene>(File.ReadAllText(output!))!;
                break;

            case SerializationType.Binary:
                using (FileStream stream = new FileStream(input!, FileMode.Open))
                {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    scene = (PodScene)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                }
                break;
        }

        switch (outputFormat)
        {
            case SerializationType.POD:
                Console.WriteLine("POD output is not currently available");
                return;

            case SerializationType.Json:
                File.WriteAllText(output!, JsonConvert.SerializeObject(scene));
                break;

            case SerializationType.Binary:
                using (FileStream stream = new FileStream(output!, FileMode.OpenOrCreate))
                {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    new BinaryFormatter().Serialize(stream, scene);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                }
                break;
        }

        Console.WriteLine($"read {input}({inputFormat}) and wrote to {output}({outputFormat})");
    }

    public static SerializationType TryParse(string format)
    {
        switch (format)
        {
            case "pod":
            case "Pod":
            case "POD":
                return SerializationType.POD;

            case "json":
            case "Json":
            case "JSON":
                return SerializationType.Json;

            case "binary":
            case "Binary":
            case "BINARY":
                return SerializationType.Binary;
        }

        throw new Exception($"\"{format}\" is not a known format, run -help");
    }
}