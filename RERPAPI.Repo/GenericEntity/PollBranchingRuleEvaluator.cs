using RERPAPI.Model.Entities;
using System.Globalization;
using System.Text.Json;

namespace RERPAPI.Repo.GenericEntity
{
    public class PollBranchingRuleEvaluator
    {
        public bool IsVisible(string? showIfJson, IReadOnlyDictionary<string, string?>? answers)
        {
            if (string.IsNullOrWhiteSpace(showIfJson))
                return true;

            try
            {
                using var document = JsonDocument.Parse(showIfJson);
                return EvaluateElement(document.RootElement, answers ?? EmptyAnswers());
            }
            catch (JsonException)
            {
                return true;
            }
        }

        public int? GetBranchNextSectionId(string? branchingRulesJson, IReadOnlyDictionary<string, string?>? answers)
        {
            return ResolveBranchingRules(branchingRulesJson, answers ?? EmptyAnswers()).NextSectionID;
        }

        public int? ResolveNextSectionId(
            PollSection currentSection,
            IEnumerable<PollSection> sections,
            IReadOnlyDictionary<string, string?>? answers)
        {
            var answerMap = answers ?? EmptyAnswers();
            var orderedSections = sections
                .Where(x => x.IsDeleted != true && x.PollFormID == currentSection.PollFormID)
                .OrderBy(x => x.SortOrder ?? int.MaxValue)
                .ThenBy(x => x.ID)
                .ToList();

            var branchResult = ResolveBranchingRules(currentSection.BranchingRulesJson, answerMap);
            if (branchResult.HasResult)
                return GetVisibleTarget(branchResult.NextSectionID, orderedSections, answerMap);

            var currentSort = currentSection.SortOrder ?? int.MaxValue;
            return orderedSections
                .Where(x => x.ID != currentSection.ID)
                .Where(x => (x.SortOrder ?? int.MaxValue) > currentSort || ((x.SortOrder ?? int.MaxValue) == currentSort && x.ID > currentSection.ID))
                .FirstOrDefault(x => IsVisible(x.ShowIfJson, answerMap))
                ?.ID;
        }

        private BranchResult ResolveBranchingRules(string? branchingRulesJson, IReadOnlyDictionary<string, string?> answers)
        {
            if (string.IsNullOrWhiteSpace(branchingRulesJson))
                return BranchResult.NoResult();

            try
            {
                using var document = JsonDocument.Parse(branchingRulesJson);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                    return ResolveRuleArray(root, answers);

                if (root.ValueKind != JsonValueKind.Object)
                    return BranchResult.NoResult();

                if (TryGetProperty(root, "rules", out var rules) && rules.ValueKind == JsonValueKind.Array)
                {
                    var ruleResult = ResolveRuleArray(rules, answers);
                    if (ruleResult.HasResult)
                        return ruleResult;
                }

                if (TryGetProperty(root, "branches", out var branches) && branches.ValueKind == JsonValueKind.Array)
                {
                    var branchResult = ResolveRuleArray(branches, answers);
                    if (branchResult.HasResult)
                        return branchResult;
                }

                if (TryGetNextSectionId(root, out var singleNextSectionId) && RuleMatches(root, answers))
                    return BranchResult.Result(singleNextSectionId);

                if (TryGetDefaultNextSectionId(root, out var defaultNextSectionId))
                    return BranchResult.Result(defaultNextSectionId);

                return BranchResult.NoResult();
            }
            catch (JsonException)
            {
                return BranchResult.NoResult();
            }
        }

        private BranchResult ResolveRuleArray(JsonElement rules, IReadOnlyDictionary<string, string?> answers)
        {
            foreach (var rule in rules.EnumerateArray())
            {
                if (rule.ValueKind != JsonValueKind.Object)
                    continue;

                if (!TryGetNextSectionId(rule, out var nextSectionId))
                    continue;

                if (RuleMatches(rule, answers))
                    return BranchResult.Result(nextSectionId);
            }

            return BranchResult.NoResult();
        }

        private bool RuleMatches(JsonElement rule, IReadOnlyDictionary<string, string?> answers)
        {
            if (TryGetProperty(rule, "condition", out var condition))
                return EvaluateElement(condition, answers);

            if (TryGetProperty(rule, "conditions", out _))
                return EvaluateElement(rule, answers);

            if (HasConditionKey(rule))
                return EvaluateCondition(rule, answers);

            return true;
        }

        private bool EvaluateElement(JsonElement element, IReadOnlyDictionary<string, string?> answers)
        {
            if (element.ValueKind == JsonValueKind.Array)
                return element.EnumerateArray().All(x => EvaluateElement(x, answers));

            if (element.ValueKind != JsonValueKind.Object)
                return false;

            if (TryGetProperty(element, "conditions", out var conditions) && conditions.ValueKind == JsonValueKind.Array)
            {
                var logic = GetString(element, "logic") ?? GetString(element, "operator") ?? "and";
                var results = conditions.EnumerateArray().Select(x => EvaluateElement(x, answers));

                return IsOr(logic)
                    ? results.Any(x => x)
                    : results.All(x => x);
            }

            return EvaluateCondition(element, answers);
        }

        private bool EvaluateCondition(JsonElement condition, IReadOnlyDictionary<string, string?> answers)
        {
            var key = GetConditionKey(condition);
            if (string.IsNullOrWhiteSpace(key))
                return false;

            var actualValues = GetAnswerValues(answers, key);
            var expectedValues = GetExpectedValues(condition);
            var op = (GetString(condition, "operator") ?? GetString(condition, "op") ?? "equals")
                .Trim()
                .ToLowerInvariant();

            return op switch
            {
                "empty" or "is_empty" => actualValues.Count == 0,
                "notempty" or "not_empty" or "is_not_empty" => actualValues.Count > 0,
                "equals" or "eq" or "==" or "is" => AnyEquals(actualValues, expectedValues),
                "notequals" or "not_equals" or "ne" or "!=" or "isnot" => !AnyEquals(actualValues, expectedValues),
                "contains" => AnyContains(actualValues, expectedValues),
                "notcontains" or "not_contains" => !AnyContains(actualValues, expectedValues),
                "in" => AnyEquals(actualValues, expectedValues),
                "notin" or "not_in" => !AnyEquals(actualValues, expectedValues),
                "greaterthan" or "greater_than" or "gt" or ">" => CompareNumber(actualValues, expectedValues, x => x > 0),
                "greaterorequal" or "greater_or_equal" or "gte" or ">=" => CompareNumber(actualValues, expectedValues, x => x >= 0),
                "lessthan" or "less_than" or "lt" or "<" => CompareNumber(actualValues, expectedValues, x => x < 0),
                "lessorequal" or "less_or_equal" or "lte" or "<=" => CompareNumber(actualValues, expectedValues, x => x <= 0),
                _ => AnyEquals(actualValues, expectedValues)
            };
        }

        private static int? GetVisibleTarget(int? nextSectionId, IEnumerable<PollSection> sections, IReadOnlyDictionary<string, string?> answers)
        {
            if (!nextSectionId.HasValue || nextSectionId.Value <= 0)
                return null;

            var evaluator = new PollBranchingRuleEvaluator();
            return sections.FirstOrDefault(x => x.ID == nextSectionId.Value && evaluator.IsVisible(x.ShowIfJson, answers))?.ID;
        }

        private static bool AnyEquals(IReadOnlyCollection<string> actualValues, IReadOnlyCollection<string> expectedValues)
        {
            if (expectedValues.Count == 0)
                return actualValues.Count == 0;

            return actualValues.Any(actual => expectedValues.Any(expected => StringEquals(actual, expected)));
        }

        private static bool AnyContains(IReadOnlyCollection<string> actualValues, IReadOnlyCollection<string> expectedValues)
        {
            if (expectedValues.Count == 0)
                return false;

            return actualValues.Any(actual =>
                expectedValues.Any(expected =>
                    actual.Contains(expected, StringComparison.OrdinalIgnoreCase) ||
                    StringEquals(actual, expected)));
        }

        private static bool CompareNumber(
            IReadOnlyCollection<string> actualValues,
            IReadOnlyCollection<string> expectedValues,
            Func<int, bool> predicate)
        {
            foreach (var actual in actualValues)
            {
                if (!double.TryParse(actual, NumberStyles.Any, CultureInfo.InvariantCulture, out var actualNumber))
                    continue;

                foreach (var expected in expectedValues)
                {
                    if (!double.TryParse(expected, NumberStyles.Any, CultureInfo.InvariantCulture, out var expectedNumber))
                        continue;

                    var comparison = actualNumber.CompareTo(expectedNumber);
                    if (predicate(comparison))
                        return true;
                }
            }

            return false;
        }

        private static bool StringEquals(string actual, string expected)
        {
            return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOr(string value)
        {
            return value.Equals("or", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("any", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasConditionKey(JsonElement condition)
        {
            return TryGetProperty(condition, "fieldKey", out _) ||
                   TryGetProperty(condition, "questionKey", out _) ||
                   TryGetProperty(condition, "key", out _) ||
                   TryGetProperty(condition, "questionId", out _) ||
                   TryGetProperty(condition, "questionID", out _);
        }

        private static string? GetConditionKey(JsonElement condition)
        {
            return GetString(condition, "fieldKey")
                ?? GetString(condition, "questionKey")
                ?? GetString(condition, "key")
                ?? GetString(condition, "questionId")
                ?? GetString(condition, "questionID");
        }

        private static IReadOnlyCollection<string> GetAnswerValues(IReadOnlyDictionary<string, string?> answers, string key)
        {
            if (!answers.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
                return Array.Empty<string>();

            var value = rawValue.Trim();
            if (value.StartsWith("[") || value.StartsWith("\""))
            {
                try
                {
                    using var document = JsonDocument.Parse(value);
                    return FlattenJsonValue(document.RootElement);
                }
                catch (JsonException)
                {
                    return new[] { value };
                }
            }

            return new[] { value };
        }

        private static IReadOnlyCollection<string> GetExpectedValues(JsonElement condition)
        {
            if (TryGetProperty(condition, "values", out var values) && values.ValueKind == JsonValueKind.Array)
                return values.EnumerateArray().SelectMany(FlattenJsonValue).ToList();

            if (TryGetProperty(condition, "value", out var value))
                return FlattenJsonValue(value);

            return Array.Empty<string>();
        }

        private static IReadOnlyCollection<string> FlattenJsonValue(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.Array => value.EnumerateArray().SelectMany(FlattenJsonValue).ToList(),
                JsonValueKind.String => new[] { value.GetString() ?? string.Empty },
                JsonValueKind.Number => new[] { value.GetRawText() },
                JsonValueKind.True => new[] { "true" },
                JsonValueKind.False => new[] { "false" },
                JsonValueKind.Null => Array.Empty<string>(),
                _ => new[] { value.GetRawText() }
            };
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            if (!TryGetProperty(element, propertyName, out var value))
                return null;

            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null
            };
        }

        private static bool TryGetNextSectionId(JsonElement element, out int? nextSectionId)
        {
            return TryGetNullableInt(element, out nextSectionId,
                "nextSectionId",
                "nextSectionID",
                "targetSectionId",
                "targetSectionID",
                "goToSectionId",
                "goToSectionID");
        }

        private static bool TryGetDefaultNextSectionId(JsonElement element, out int? nextSectionId)
        {
            return TryGetNullableInt(element, out nextSectionId,
                "defaultNextSectionId",
                "defaultNextSectionID",
                "defaultSectionId",
                "defaultSectionID");
        }

        private static bool TryGetNullableInt(JsonElement element, out int? value, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (!TryGetProperty(element, propertyName, out var property))
                    continue;

                if (property.ValueKind == JsonValueKind.Null)
                {
                    value = null;
                    return true;
                }

                if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var intValue))
                {
                    value = intValue;
                    return true;
                }

                if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out intValue))
                {
                    value = intValue;
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        private static IReadOnlyDictionary<string, string?> EmptyAnswers()
        {
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }

        private sealed class BranchResult
        {
            private BranchResult(bool hasResult, int? nextSectionId)
            {
                HasResult = hasResult;
                NextSectionID = nextSectionId;
            }

            public bool HasResult { get; }
            public int? NextSectionID { get; }

            public static BranchResult Result(int? nextSectionId)
            {
                return new BranchResult(true, nextSectionId);
            }

            public static BranchResult NoResult()
            {
                return new BranchResult(false, null);
            }
        }
    }
}