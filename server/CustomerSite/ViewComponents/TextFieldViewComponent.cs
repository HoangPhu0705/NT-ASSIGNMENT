using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.ViewComponents;

public class TextFieldViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string id, string name, string label, string type,
        string placeholder = " ", string value = "", string error = null)
    {
        var model = new TextFieldViewModel
        {
            Id = id,
            Name = name,
            Label = label,
            Type = type,
            Placeholder = placeholder,
            Value = value,
            Error = error
        };
            
        return View(model);
    }
}

public class TextFieldViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public string Placeholder { get; set; }
    public string Value { get; set; }
    public string Error { get; set; }
}