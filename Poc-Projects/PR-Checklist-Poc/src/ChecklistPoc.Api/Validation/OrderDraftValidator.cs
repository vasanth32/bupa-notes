using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Validation;

public static class OrderDraftValidator
{
    public static IReadOnlyDictionary<string, string[]> Validate(OrderDraft draft)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(draft.CustomerId))
        {
            Add(errors, nameof(draft.CustomerId), "CustomerId is required.");
        }

        if (draft.Lines is null || draft.Lines.Count == 0)
        {
            Add(errors, nameof(draft.Lines), "At least one line is required.");
            return ToResult(errors);
        }

        for (var i = 0; i < draft.Lines.Count; i++)
        {
            var line = draft.Lines[i];
            var prefix = $"Lines[{i}]";

            if (string.IsNullOrWhiteSpace(line.ProductCode))
            {
                Add(errors, $"{prefix}.{nameof(line.ProductCode)}", "ProductCode is required.");
            }

            if (line.Quantity <= 0)
            {
                Add(errors, $"{prefix}.{nameof(line.Quantity)}", "Quantity must be > 0.");
            }

            if (line.UnitPrice <= 0)
            {
                Add(errors, $"{prefix}.{nameof(line.UnitPrice)}", "UnitPrice must be > 0.");
            }
        }

        var duplicateProductCodes = draft.Lines
            .Where(l => !string.IsNullOrWhiteSpace(l.ProductCode))
            .GroupBy(l => l.ProductCode, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplicateProductCodes.Length > 0)
        {
            Add(errors, nameof(draft.Lines), $"Duplicate product codes are not allowed: {string.Join(", ", duplicateProductCodes)}");
        }

        return ToResult(errors);
    }

    private static void Add(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.TryGetValue(key, out var list))
        {
            list = [];
            errors[key] = list;
        }

        list.Add(message);
    }

    private static IReadOnlyDictionary<string, string[]> ToResult(Dictionary<string, List<string>> errors)
        => errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(), StringComparer.OrdinalIgnoreCase);
}

