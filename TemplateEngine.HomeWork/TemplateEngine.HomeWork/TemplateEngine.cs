using System.Text.RegularExpressions;

namespace TemplateEngine.HomeWork;

public class TemplateEngine
{
    public string Render(string template, object data)
    {
        template = ProcessLoops(template, data);
        
        var properties = data.GetType().GetProperties();
        var result = template;

        foreach (var property in properties)
        {
            var placeholder = $"{{{{{property.Name}}}}}";
            var value = property.GetValue(data);
            
            if (value is DateTime dateValue)
            {
                value = dateValue.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));
            }

            result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
        }

        return result;
    }

    public string ProcessLoops(string template, object data)
    {
        var regex = new Regex(@"\{\{#foreach (.*?)\}\}(.*?)\{\{\/foreach\}\}", RegexOptions.Singleline);
        return regex.Replace(template, match =>
        {
            var propertyName = match.Groups[1].Value;
            var loopContent = match.Groups[2].Value;

            var property = data.GetType().GetProperty(propertyName);
            if (property != null)
            {
                var collection = property.GetValue(data) as IEnumerable<object>;
                if (collection != null && collection.Any())
                {
                    var loopResult = string.Empty;
                    foreach (var item in collection)
                    {
                        loopResult += new TemplateEngine().Render(loopContent, item);
                    }
                    return loopResult;
                }
            }
            return string.Empty;
        });
    }

    public string ProcessConditions(string template, object data)
    {
        var regex = new Regex(@"\{\{#if (.*?)\}\}(.*?)(\{\{#else\}\}(.*?))?\{\{\/if\}\}", RegexOptions.Singleline);
    
        return regex.Replace(template, match =>
        {
            var conditionProperty = match.Groups[1].Value.Trim();
            var ifContent = match.Groups[2].Value.Trim();     
            var elseContent = match.Groups[4].Value.Trim();

            var property = data.GetType().GetProperty(conditionProperty);
            if (property != null)
            {
                var value = property.GetValue(data);
                bool conditionResult = value != null && (value is bool b && b || !(value is bool));

                if (conditionResult)
                {
                    return new TemplateEngine().Render(ifContent, data); 
                }
                else if (!string.IsNullOrEmpty(elseContent))
                {
                    return new TemplateEngine().Render(elseContent, data);
                }
            }

            return string.Empty;
        });
    }
}