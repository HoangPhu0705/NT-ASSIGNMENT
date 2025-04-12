using Microsoft.AspNetCore.Mvc;

namespace CustomerSite.ViewComponents;

public class FloatingInputViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string id, string value, string placeholder, string label, string error = null)
    {
        var model = new FloatingInputViewModel
        {
            Id = id,
            Value = value,
            Placeholder = placeholder ?? " ",
            Label = label,
            Error = error
        };
        return View("Default", model);
    }
    
    public class FloatingInputViewModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }
        public string Label { get; set; }
        public string Error { get; set; }
    }
}