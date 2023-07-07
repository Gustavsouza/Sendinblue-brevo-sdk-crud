using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sendinblue;
using System.Globalization;
using System.Text;

public class SendinBlueService
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public SendinBlueService(string apiKey)
    {
        _client = new HttpClient();
        _apiKey = apiKey;
        _client.DefaultRequestHeaders.Add("api-key", _apiKey);
    }

    public async Task CriarContato(Contato contato)
    {
        string url = "https://api.sendinblue.com/v3/contacts";

        var contactData = new
        {
            email = contato.Email,
            attributes = new
            {
                NOME = contato.NOME,
                DtNascimento = contato.DtNascimento,
                Ativo = contato.Ativo
            }
        };

        string jsonData = JsonConvert.SerializeObject(contactData);

        HttpResponseMessage response = await _client.PostAsync(url, new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"O contato foi criado com sucesso!");
        }
        else
        {
            Console.WriteLine($"Erro ao criar o contato. Código de status: " + response.StatusCode + "\n" + response.RequestMessage + "\n" + response.Content);
        }
    }

    public async Task<List<Contato>> BuscarContatos()
    {
        string url = "https://api.sendinblue.com/v3/contacts";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            // Criar um objeto anônimo com uma propriedade "contacts" para envolver o JSON retornado
            var contactsWrapper = new { Contacts = new List<Contato>() };

            // Desserializar o JSON utilizando o objeto anônimo
            var result = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseContent, contactsWrapper);

            // Retornar a lista de contatos
            return result.Contacts;
        }
        else
        {
            Console.WriteLine("Erro ao buscar os contatos. Código de status: " + response.StatusCode);
            return null;
        }
    }

    public async Task AtualizarAtributos(string email, Dictionary<string, object> attributes)
    {
        string url = $"https://api.sendinblue.com/v3/contacts/{email}";

        // Verificar e converter os valores dos atributos
        foreach (var attribute in attributes)
        {
            // Verificar se o atributo é uma string com formato de data
            if (attribute.Value is string stringValue)
            {
                if (DateTime.TryParseExact(stringValue, new[] { "dd-MM-yyyy", "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    // Converter a data para o formato YYYY-MM-DD
                    attributes[attribute.Key] = parsedDate.ToString("yyyy-MM-dd");
                }
            }
            // Verificar se o valor do atributo é "1" ou "2" e converter para booleano
            if (attribute.Value is string)
            {
                string attributeValue = (string)attribute.Value;
                if (attributeValue == "1")
                {
                    attributes[attribute.Key] = true;
                }
                else if (attributeValue == "2")
                {
                    attributes[attribute.Key] = false;
                }
                else if (attributeValue == "Serra")
                {
                    attributes[attribute.Key] = 1; // esse é o ID da cidade do tipo categoria #gambiarra para funcionar
                }
                else if (attributeValue == "Vitoria")
                {
                    attributes[attribute.Key] = 2; // esse é o ID da cidade do tipo categoria #gambiarra para funcionar
                }
            }
        }

        var contactData = new
        {
            attributes
        };

        string jsonData = JsonConvert.SerializeObject(contactData);

        HttpResponseMessage response = await _client.PutAsync(url, new StringContent(jsonData, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Os atributos do contato {email} foram atualizados com sucesso!");

            //// Salvar a resposta da API em um arquivo JSON
            //string responseContent = await response.Content.ReadAsStringAsync();

            //// Caminho do arquivo na área de trabalho
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string filePath = Path.Combine(desktopPath, "ContatoJsonAtualizar.json");

            //// Salvar o JSON em um arquivo
            //File.WriteAllText(filePath, responseContent);

            //Console.WriteLine($"O arquivo 'ContatoJsonAtualizar.json' foi salvo na área de trabalho.");
        }
        else
        {
            Console.WriteLine($"Erro ao atualizar os atributos do contato {email}. Código de status: " + response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Resposta de erro: {responseContent}");
        }
    }

    public async Task<string> BuscarUsuariosResponse()
    {
        string url = "https://api.sendinblue.com/v3/contacts";

        HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            // Formatar a resposta como JSON
            string jsonFormatted = JToken.Parse(responseContent).ToString(Formatting.Indented);

            // Salvar a resposta em um arquivo
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "ContatoResponse.json");
            File.WriteAllText(filePath, jsonFormatted);

            Console.WriteLine("Resposta salva com sucesso em ContatoResponse.json na área de trabalho!");

            return jsonFormatted;
        }
        else
        {
            Console.WriteLine($"Erro ao buscar os usuários. Código de status: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Resposta de erro: {responseContent}");
            return null;
        }
    }

    public async Task CriarAtributo(string categoria, string nomeAtributo, string tipoAtributo)
    {
        string url = $"https://api.sendinblue.com/v3/contacts/attributes/{categoria}/{nomeAtributo}";

        var createAttribute = new
        {
            type = tipoAtributo // Defina o tipo do atributo, por exemplo, "text", "date", "boolean", etc.
        };

        string jsonData = JsonConvert.SerializeObject(createAttribute);

        HttpResponseMessage response = await _client.PostAsync(url, new StringContent(jsonData, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"O atributo {nomeAtributo} foi criado com sucesso!");
        }
        else
        {
            Console.WriteLine($"Erro ao criar o atributo {nomeAtributo}. Código de status: " + response.StatusCode);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar o atributo {nomeAtributo}. Código de status: {response.StatusCode}");
            Console.WriteLine($"Resposta de erro: {responseContent}");
        }
    }
}