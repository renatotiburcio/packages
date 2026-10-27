using System.Collections.Generic;
namespace XForge.DesignSystem.Components.Icon;
public interface IXFIconClassifier { XFIconCategory Classify(XFIconInfo info); IEnumerable<XFIconInfo> ByCategory(XFIconCategory cat); }