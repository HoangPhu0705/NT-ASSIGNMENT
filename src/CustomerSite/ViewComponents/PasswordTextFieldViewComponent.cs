using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.ViewComponents;

public class PasswordTextFieldViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string id, string name, string label, 
        string placeholder = " ", string value = "", string error = null)
    {
        var model = new PasswordTextFieldViewModel
        {
            Id = id,
            Name = name,
            Label = label,
            Placeholder = placeholder,
            Value = value,
            Error = error
        };
            
        return View(model);
    }
}

public class PasswordTextFieldViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public string Placeholder { get; set; }
    public string Value { get; set; }
    public string Error { get; set; }
}