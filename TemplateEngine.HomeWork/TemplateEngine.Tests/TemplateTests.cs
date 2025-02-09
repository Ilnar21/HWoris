using TemplateEngine.Tests.models;

namespace TemplateEngine.Tests;

public class Tests
{
    [Test]
    public void RenderTest()
    {
        var template = "Hi, {{Name}}! You are {{Age}} years old.";
        var data = new { Name = "Timerkhan", Age = "100" };
        var engine = new HomeWork.TemplateEngine();

        var result = engine.Render(template, data);
        Assert.AreEqual("Hello, Timerkhan! You`re 100 years old.", result);
    }


    [Test]
    public void ProcessLoopsTest()
    {
        var template = "{{#foreach Items}}Hello, {{Name}}! {{/foreach}}";
        var data = new
        {
            Items = new List<Item>
                { new Item { Name = "Arthur" }, new Item { Name = "Dexter" }, new Item { Name = "Stepa" } }
        };
        var engine = new HomeWork.TemplateEngine();

        var result = engine.ProcessLoops(template, data);

        Assert.AreEqual("Hello, Max! Hello, Mike! Hello, Sasha! ", result);
    }

    [Test]
    public void ProcessConditionsTest()
    {
        var template = "{{#if IsMan}}Hello, Men!{{#else}}Hello, Women!{{/if}}";
        var engine = new HomeWork.TemplateEngine();


        var dataTrue = new { IsMan = true };
        var resultTrue = engine.ProcessConditions(template, dataTrue);
        Assert.AreEqual("Hello, Men!", resultTrue);

        var dataFalse = new { IsMan = false };
        var resultFalse = engine.ProcessConditions(template, dataFalse);
        Assert.AreEqual("Hello, Women!", resultFalse);
    }
}