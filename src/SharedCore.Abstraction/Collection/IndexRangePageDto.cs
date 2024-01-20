using System.Text.Json;
using SharedCore.Abstraction.ProcessResult;
using SharedCore.Abstraction.Services;

namespace SharedCore.Abstraction.Collection;

public record IndexRangePageDto 
{
    public IndexRangePageDto(string? range)
    {
        if (range is null)
        {
            StartIndex = 0;
            EndIndex = 1000;
            return;
        }
        
        var arr = JsonSerializer.Deserialize<int[]>(range).AssertNull();
        if (arr.Length != 2) throw new ArgumentException($"Range must have two elements in array bout found: ${range}");

        StartIndex = arr[0];
        EndIndex = arr[1];
    }

    public int StartIndex { get; init; }
    public int EndIndex { get; init; }

    public int GetTakeCount() => EndIndex - StartIndex + 1;

    public RichRange ToRichRange(int total) => new(StartIndex, EndIndex, total);
}