using Newtonsoft.Json;
using Sendinblue;
using System.Globalization;

public class Program
{
    private static async Task Main(string[] args)
    {
       Configuration.Default.ApiKey.Add("api-key", "SUA CHAVE BONITÃO");

        SendinBlueService sendinBlueService = new SendinBlueService(apiKey);
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nDigite a opção desejada:");
            Console.WriteLine("1. Criar um contato");
            Console.WriteLine("3. Buscar contatos");
            Console.WriteLine("4. Atualizar atributos de um contato");
            Console.WriteLine("5. Criar novos atributos para todos os contato");
            Console.WriteLine("6. Buscar contatos response");
            Console.WriteLine("0. Sair");

            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Console.WriteLine("Digite o e-mail do contato:");
                    string email = Console.ReadLine();

                    Console.WriteLine("Digite o nome do contato:");
                    string nome = Console.ReadLine();

                    Console.WriteLine("Digite a data de nascimento do contato (formato: dd/mm/aaaa):");
                    string dataNascimentoStr = Console.ReadLine();
                    DateTime dataNascimento;

                    if (DateTime.TryParseExact(dataNascimentoStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                    {
                        Contato contato = new Contato
                        {
                            Email = email,
                            EmailBlacklisted = false,
                            SmsBlacklisted = false,
                            CreatedAt = DateTime.Now,
                            ModifiedAt = DateTime.Now,
                            ListIds = new List<int>(),
                            NOME = nome,
                            DtNascimento = dataNascimento.ToString("yyyy-MM-dd"),
                            Ativo = true,
                        };

                        await sendinBlueService.CriarContato(contato);
                    }
                    else
                    {
                        Console.WriteLine("Data de nascimento inválida.");
                    }
                    break;

                case "3":
                    // Buscar todos os contatos
                    List<Contato> contacts = await sendinBlueService.BuscarContatos();
                    if (contacts != null)
                    {
                        Console.WriteLine("Contatos encontrados:");
                        foreach (var contact in contacts)
                        {
                            Console.WriteLine($"Email: {contact.Email}");
                            Console.WriteLine($"ID: {contact.Id}");
                            Console.WriteLine($"AtributosPersonalizados: {contact.Attributes.Count()}");
                            foreach (var attribute in contact.Attributes)
                            {
                                Console.WriteLine($"{attribute.Key}: {attribute.Value}");
                            }
                            Console.WriteLine();
                        }

                        // Serializar a lista de contatos em formato JSON
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(contacts, Formatting.Indented);

                        // Caminho do arquivo na área de trabalho
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string filePath = Path.Combine(desktopPath, "contatos.json");

                        // Salvar o JSON em um arquivo
                        File.WriteAllText(filePath, json);

                        Console.WriteLine($"O arquivo 'contatos.json' foi salvo na área de trabalho.");
                    }
                    break;

                case "4":
                    Console.WriteLine("Digite o e-mail do contato para atualizar os atributos:");
                    string emailContato = Console.ReadLine();

                    Console.WriteLine("Digite a chave do atributo a ser atualizado:");
                    string NomeAtributoAtualizar = Console.ReadLine();

                    Console.WriteLine("Digite o valor do atributo:");
                    string ValorAtributoAtualizar = Console.ReadLine();

                    var attributesToUpdate = new Dictionary<string, object>()
                    {
                        { NomeAtributoAtualizar, ValorAtributoAtualizar }
                    };

                    await sendinBlueService.AtualizarAtributos(emailContato, attributesToUpdate);
                    break;

                case "5":
                    Console.WriteLine("Digite o nome do atributo:");
                    string nomeAtributo = Console.ReadLine();
                    string categoria = "normal";
                    // Defina o tipo do atributo como "boolean" ou qualquer outro tipo suportado pelo Sendinblue
                    Console.WriteLine("Digite o tipo do atributo:");
                    string tipoAtributo = Console.ReadLine();
                    await sendinBlueService.CriarAtributo(categoria, nomeAtributo, tipoAtributo);
                    break;

                case "6":
                    // Chamar o método BuscarUsuarios
                    string response = await sendinBlueService.BuscarUsuariosResponse();

                    // Verificar se a resposta é válida
                    if (response != null)
                    {
                        // Processar a resposta conforme necessário
                        Console.WriteLine("Resposta da API:");
                        Console.WriteLine(response);
                    }
                    else
                    {
                        // Tratar o caso de erro na resposta
                        Console.WriteLine("Erro ao buscar os usuários.");
                    }

                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Opção inválida. Digite uma opção válida.");
                    break;
            }
        }
    }
}