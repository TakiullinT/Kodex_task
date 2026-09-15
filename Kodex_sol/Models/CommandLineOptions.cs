namespace Kodex_task.Models;

public record CommandLineOptions
{
    public string Mode { get; init; } = "console";
    public string InputFilePath { get; init; } = string.Empty;
    public string? OutputFilePath { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool ShowHelp { get; init; }

    public static CommandLineOptions Parse(string[] args)
    {
        var mode = "console";
        string input = string.Empty;
        string? output = null;
        DateTime? startDate = null;
        DateTime? endDate = null;
        bool showHelp = false;

        foreach (var arg in args)
        {
            if (arg is "--help" or "-h")
            {
                showHelp = true;
                continue;
            }

            var parts = arg.Split('=', 2);
            if (parts.Length != 2) continue;

            var key = parts[0].ToLowerInvariant();
            var value = parts[1].Trim('"');

            switch (key)
            {
                case "--mode" or "-m":
                    mode = value.ToLowerInvariant();
                    break;
                case "--input":
                    input = value;
                    break;
                case "--output":
                    output = value;
                    break;
                case "--start_date":
                    if (DateTime.TryParse(value, out var sd)) startDate = sd;
                    break;
                case "--end_date":
                    if (DateTime.TryParse(value, out var ed)) endDate = ed;
                    break;
            }
        }

        return new CommandLineOptions
        {
            Mode = mode,
            InputFilePath = input,
            OutputFilePath = output,
            StartDate = startDate,
            EndDate = endDate,
            ShowHelp = showHelp
        };
    }
}