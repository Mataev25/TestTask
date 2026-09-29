using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Dapper;
using Npgsql;

namespace TestTask.API;

public class TestService
{
    private readonly string connectionString;
    private static readonly Regex EmailRegex = new Regex(
        @"[a-zA-Z0-9.%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        RegexOptions.Compiled
    );

    public TestService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<ResponseModel> ProcessAsync(RequestModel request)
    {
        var response = new ResponseModel();
    
        try
        {
            string url, html;
            try
            {
                url = Encoding
                    .UTF8
                    .GetString(Convert
                    .FromBase64String(request.Url_b64));
            }
            catch(FormatException e)
            {
                response.Is_error = 1;
                response.Error_code = "BASE64_URL_ERROR";
                response.Error_message = e.Message;
                return response;
            }

            try
            {
                html = Encoding
                    .UTF8
                    .GetString(Convert
                    .FromBase64String(request.Page_b64));
            }
            catch(FormatException e)
            {
                response.Is_error = 1;
                response.Error_code = "BASE64_PAGE_ERROR";
                response.Error_message = e.Message;
                return response;
            }

            response.Url = url;
            
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(html);
            var elements = document.QuerySelectorAll(request.Selector);
            response.Elements_count = elements.Length;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var attrValues = new List<string>();
                foreach (var el in elements)
                {
                    string attrValue = el
                        .GetAttribute(request.Attribute) ?? "";
                    attrValues.Add(attrValue);
                    await connection.ExecuteAsync(
                        "INSERT INTO elements (attr_value, html) VALUES (@AttrValue, @Html)",
                        new
                        {
                            AttrValue = attrValue,
                            Html = el.OuterHtml
                        }
                    );
                }
                response.Elements_attr_list = attrValues;
            }

            var matches = EmailRegex.Matches(html);
            response.Emails_count = matches.Count;

            var emails = new List<string>();
            foreach (Match m in matches)
                emails.Add(m.Value);
            response.Emails_list = emails;

            try
            {
                byte[] key = Convert.FromBase64String(request.Key_bytes_b64);
                byte[] cipher = Convert.FromBase64String(request.Encrypted_text_bytes_b64);

                response.Decrypted_plain_text = DecryptAes256Ecb(cipher, key);
            }
            catch (FormatException e)
            {
                response.Is_error = 1;
                response.Error_code = "BASE64_ENCRYPT_ERROR";
                response.Error_message = e.Message;
                return response;
            }
            catch(CryptographicException e) 
            {
                response.Is_error = 1;
                response.Error_code = "DECRYPT_ERROR";
                response.Error_message = e.Message;
                return response;
            }
            catch (Exception e) 
            {
                response.Is_error = 1;
                response.Error_code = "AES_ERROR";
                response.Error_message = e.Message;
                return response;
            }
        }
        catch(Exception e)
        {
            response.Is_error = 1;
            response.Error_code = "OTHER_ERROR";
            response.Error_message = e.Message;
        }
        return response;
    }

    private string DecryptAes256Ecb(byte[] cipher, byte[] key)
    {
        using Aes aes = Aes.Create() ?? throw new CryptographicException("Не удалось создать AES");

      

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
        {
            throw new CryptographicException(
                $"Недопустимая длина ключа: {key.Length} байт. Требуется 16, 24 или 32.");
        }
        
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        using (var decryptor = aes.CreateDecryptor())
        {
            byte[] plainBytes = decryptor
                .TransformFinalBlock(cipher, 0, cipher.Length)
                    ?? throw new CryptographicException("Дешифрование вернуло null");

            return Encoding.UTF8.GetString(plainBytes);
        }
        
    }
}