using System.Collections.Generic;

namespace XForge.DesignSystem.Components.Icon;

public static class XFIconCategoryRules
{
    public static readonly IReadOnlyList<(XFIconCategory Cat, string[] Keywords)> Rules = new[]
    {
        (XFIconCategory.Communication, new[] { "mail", "phone", "message", "chat", "envelope" }),
        (XFIconCategory.Media, new[] { "music", "video", "film", "play", "pause" }),
        (XFIconCategory.Travel, new[] { "plane", "car", "ship", "travel", "luggage" }),
        (XFIconCategory.Money, new[] { "dollar", "euro", "money", "credit", "wallet" }),
        (XFIconCategory.Tools, new[] { "wrench", "hammer", "screwdriver", "tool" }),
        (XFIconCategory.Shopping, new[] { "cart", "shop", "bag", "store" }),
        (XFIconCategory.Maps, new[] { "map", "location", "pin", "compass" }),
        (XFIconCategory.Sports, new[] { "ball", "sport", "game", "trophy" }),
        (XFIconCategory.Food, new[] { "food", "apple", "pizza", "burger" }),
        (XFIconCategory.Animals, new[] { "dog", "cat", "fish", "bird" }),
        (XFIconCategory.Nature, new[] { "tree", "leaf", "flower", "sun" }),
        (XFIconCategory.Weather, new[] { "cloud", "rain", "snow", "storm" }),
        (XFIconCategory.Buildings, new[] { "house", "building", "office", "factory" }),
        (XFIconCategory.Transportation, new[] { "car", "bus", "train", "bicycle" }),
        (XFIconCategory.People, new[] { "user", "person", "people", "man", "woman" }),
        (XFIconCategory.Healthcare, new[] { "hospital", "heart", "pulse", "health" }),
        (XFIconCategory.Education, new[] { "book", "school", "graduation", "pen" }),
        (XFIconCategory.Alerts, new[] { "warning", "alert", "exclamation", "bell" }),
        (XFIconCategory.Code, new[] { "code", "bug", "terminal", "git" }),
        (XFIconCategory.Files, new[] { "file", "folder", "document", "pdf" }),
        (XFIconCategory.Arrows, new[] { "arrow", "chevron", "caret" }),
        (XFIconCategory.Devices, new[] { "mobile", "tablet", "laptop", "desktop" }),
        (XFIconCategory.Objects, new[] { "box", "package", "gift", "tag" }),
    };
}