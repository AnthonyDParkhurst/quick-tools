// See https://aka.ms/new-console-template for more information

using System.Text;

foreach (var file in args)
{
    if (NeedsUpdating(file))
    {
        UpdateFile(file);
    }
}

void UpdateFile(string file)
{
    using var infile = File.OpenText(file);
    var outname = Path.ChangeExtension(file, "updated");
    using var outfile = new StreamWriter(File.OpenWrite(outname), new UTF8Encoding(true));

    while (!infile.EndOfStream)
    {
        var line = infile.ReadLine();
        if (line == null)
        {
            break;
        }

        outfile.WriteLine(line);

        if (line.Contains("CaptureConfiguration"))
        {
            outfile.WriteLine(line.Replace("CaptureConfiguration", "CaptureDesign"));
        }
    }

    infile.Close();
    outfile.Close();

    var bakFile = Path.ChangeExtension(file, "mybak");

    if (File.Exists(bakFile))
    {
        File.Delete(bakFile);
    }

    File.Move(file, bakFile);
    File.Move(outname, Path.ChangeExtension(outname, "config"));
}

bool NeedsUpdating(string file)
{
    var data = File.ReadAllText(file);

    return data.Contains("CaptureConfiguration") && !data.Contains("CaptureDesign");
}