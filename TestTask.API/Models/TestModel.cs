using System.Collections.Generic;
using FluentValidation;

namespace TestTask.API;

public class RequestModel
{
    public string Selector {get; set;} = "";
    public string Attribute {get; set;} = "";
    public string Url_b64 {get; set;} = "";
    public string Encrypted_text_bytes_b64 {get; set;} = "";
    public string Key_bytes_b64 {get; set;} = "";
    public string Page_b64 {get; set;} = "";
}

public class ResponseModel
{
    public int Is_error {get; set;}
    public string Error_code {get; set;} = "";
    public string Error_message {get; set;} = "";
    public int Elements_count {get; set;}
    public int Emails_count {get; set;}
    public string Url {get; set;} = "";
    public string Decrypted_plain_text {get; set;} = "";
    public List<string> Elements_attr_list {get; set;} = new();
    public List<string> Emails_list {get; set;} = new();
}

public class RequestValidator : AbstractValidator<RequestModel>
{
    public RequestValidator()
    {
        RuleFor(r => r.Selector)
            .NotEmpty().WithMessage("Селектор не может быть пустым");
        RuleFor(r => r.Attribute)
            .NotEmpty().WithMessage("Атрибут не может быть пустым");
        RuleFor(r => r.Url_b64)
            .NotEmpty().WithMessage("URL не может быть пустым");
        RuleFor(r => r.Encrypted_text_bytes_b64)
            .NotEmpty().WithMessage("Зашифрованный текст не может быть пустым");
        RuleFor(r => r.Key_bytes_b64)
            .NotEmpty().WithMessage("Ключ не может быть пустым");
        RuleFor(r => r.Page_b64)
            .NotEmpty().WithMessage("Страница не может быть пустой");
    }
}