using System.Text.Json;
using SharedCore.Abstraction.Services;

namespace SharedCore.Abstraction.Collection;

public record SortDto<TColumnEnum>
    where TColumnEnum : struct, Enum
{
    public SortDto(string? sort)
    {
        if (sort is null)
        {
            Direction = SortDirection.Asc;
            Column = Enum.GetValues<TColumnEnum>()[0];
            return;
        }
        
        var arr = JsonSerializer.Deserialize<string[]>(sort).AssertNull();
        if (arr.Length != 2) throw new ArgumentException($"Sort must have two elements in array bout found: ${sort}");
        var direction = arr[1];
        var column = arr[0];

        var directionEnum = Enum.Parse<SortDirection>(direction, true);
        var columnEnum = Enum.Parse<TColumnEnum>(column, true);

        Direction = directionEnum;
        Column = columnEnum;
    }

    public TColumnEnum Column { get; init; }
    public SortDirection Direction { get; init; }
}