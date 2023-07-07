using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

Configuration.Default.ApiKey.Add("api-key", "SUA KEY BONITÃO");

// Instanciando a classe de API para manipular os contatos
var apiInstance = new ContactsApi();

bool sair = false;
while (!sair)
{
    Console.WriteLine("=== MENU ===");
    Console.WriteLine("1. Criar Contato");
    Console.WriteLine("2. Atualizar Atributo do Contato");
    Console.WriteLine("3. Criar Atributos do Contato");
    Console.WriteLine("4. Deletar um Contato");
    Console.WriteLine("5. Buscar um Contato");
    Console.WriteLine("6. Buscar todos os atributos");
    Console.WriteLine("7. Criar contatos com todos os atributos existentes");
    Console.WriteLine("8. Sair");

    Console.Write("Digite a opção desejada: ");
    string opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            // Criar Contato
            CriarContato(apiInstance);
            break;

        case "2":
            // Atualizar Atributo do Contato
            AtualizarContato(apiInstance);
            break;

        case "3":
            // Inserir Atributos do Contato
            CriarAtributoContatos(apiInstance);
            break;

        case "4":
            //Apagar usuario
            DeletarUsuario(apiInstance);
            break;

        case "5":
            //Apagar usuario
            BuscarUsuario(apiInstance);
            break;

        case "6":
            //Buscar todos os atributos existentes
            ListarAtributos(apiInstance);
            break;

        case "7":
            //Criar contatos com todos os atributos existentes
            CriarContatoComAtributosDisponiveis(apiInstance);
            break;

        case "8":
            // Sair
            sair = true;
            break;

        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }

    Console.WriteLine();
}

static void CriarContato(ContactsApi apiInstance)
{
    Console.WriteLine("========= CRIAR CONTATO =========");
    try
    {
        Console.WriteLine("Digite o e-mail do contato:");
        string email = Console.ReadLine();

        Console.WriteLine("Digite o nome do contato:");
        string nome = Console.ReadLine();

        Console.WriteLine("Digite a data de nascimento do contato:");
        string Dtnascimento = Console.ReadLine();

        // Criando um objeto de contato
        CreateContact contato = new CreateContact
        {
            Email = email,
            ListIds = new System.Collections.Generic.List<long?> { 1 }, // IDs das listas de contatos às quais você deseja adicionar o contato(caso tenha)
            UpdateEnabled = true, // Defina como true para atualizar o contato se ele já existir ou false para criar um novo contato
            Attributes = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Nome", nome }, // Exemplo de atributo personalizado
            { "DTNASCIMENTO", Dtnascimento } // Exemplo de atributo personalizado
        }
        };

        // Criando o contato
        CreateUpdateContactModel result = apiInstance.CreateContact(contato);
        Console.WriteLine("Contato criado com sucesso. ID do contato: " + result.Id);
    }
    catch (Exception e)
    {
        Console.WriteLine("ERRO: " + e.Message);
    }
}

static void AtualizarContato(ContactsApi apiInstance)
{
    Console.WriteLine("========= ATUALIZAR CONTATO =========");
    Console.WriteLine("Digite o ID do usuário para ser atualizado");
    string id = Console.ReadLine();

    if (long.TryParse(id, out long contatoId))
    {
        Console.Write("Digite o nome do atributo: ");
        string atributoNome = Console.ReadLine();

        Console.Write("Digite o valor do atributo: ");
        string atributoValor = Console.ReadLine();

        var attributes = new Dictionary<string, object>(); // Usar object para aceitar valores booleanos

        if (DateTime.TryParse(atributoValor, out DateTime data))
        {
            attributes.Add(atributoNome, data.ToString("yyyy-MM-dd"));
        }
        else if (bool.TryParse(atributoValor, out bool atributoValorBool))
        {
            attributes.Add(atributoNome, atributoValorBool);
        }
        else
        {
            attributes.Add(atributoNome, atributoValor);
        }

        var contatoAtributos = new UpdateContact
        {
            Attributes = attributes
        };

        try
        {
            apiInstance.UpdateContact(id, contatoAtributos);
            Console.WriteLine("Atributo do contato atualizado com sucesso.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro ao atualizar o atributo do contato: " + e.Message);
        }
    }
    else
    {
        Console.WriteLine("ID de contato inválido.");
    }
}

static async void CriarAtributoContatos(ContactsApi apiInstance)
{
    Console.WriteLine("========= CRIAR ATRIBUTO PARA CONTATO =========");
    Console.Write("Digite o nome do atributo: ");
    string atributoNome = Console.ReadLine();

    Console.Write("Digite o tipo do atributo: ");
    string atributoTipo = Console.ReadLine();

    Console.Write("Digite a categoria do atributo: ");
    string attributeCategory = Console.ReadLine();

    CreateAttribute createAttribute = null;

    if (atributoTipo.ToLower() == "boolean")
    {
        createAttribute = new CreateAttribute { Value = null, Type = CreateAttribute.TypeEnum.Boolean };
    }
    else if (atributoTipo.ToLower() == "datetime")
    {
        createAttribute = new CreateAttribute { Value = null, Type = CreateAttribute.TypeEnum.Date };
    }
    else if (atributoTipo.ToLower() == "float")
    {
        createAttribute = new CreateAttribute { Value = null, Type = CreateAttribute.TypeEnum.Float };
    }
    else if (atributoTipo.ToLower() == "text")
    {
        createAttribute = new CreateAttribute { Type = CreateAttribute.TypeEnum.Text };
    }
    else
    {
        //string attributeCategory = "category";
        // Criar as enumerações do atributo
        List<CreateAttributeEnumeration> enumerations = new List<CreateAttributeEnumeration>();

        Console.WriteLine("Digite as opções de enumeração (digite 'sair' para finalizar):");

        string optionValue;
        int optionNumber = 1;

        while (true)
        {
            Console.Write($"Opção {optionNumber}: ");
            optionValue = Console.ReadLine();

            if (optionValue.ToLower() == "sair")
            {
                break;
            }

            var enumeration = new CreateAttributeEnumeration(optionNumber, optionValue);
            enumerations.Add(enumeration);

            optionNumber++;
        }

        CreateAttribute.TypeEnum type = CreateAttribute.TypeEnum.Category;

        try
        {
            // Criar o objeto CreateAttribute
            createAttribute = new CreateAttribute(null, false, enumerations, type);

            // Inserir o atributo
            apiInstance.CreateAttribute(attributeCategory, atributoNome, createAttribute);

            Console.WriteLine("Atributo criado com sucesso.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro ao criar o atributo: " + e.Message);
        }
    }
    try
    {
        //string attributeCategory = "normal";
        if (attributeCategory.ToLower() != "category")
        {
            await apiInstance.CreateAttributeAsync(attributeCategory, atributoNome, createAttribute);
            Console.WriteLine("Atributo do contato criado com sucesso.");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Erro ao criar o atributo do contato: " + e.Message);
    }
}

static void DeletarUsuario(ContactsApi apiInstance)
{
    Console.WriteLine("========= DELETAR CONTATO =========");
    Console.Write("Digite o email do usuário a ser deletado ");
    string email = Console.ReadLine();

    try
    {
        apiInstance.DeleteContact(email);
        Console.WriteLine("Usuario deletado com sucesso.");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}

static void BuscarUsuario(ContactsApi apiInstance)
{
    Console.WriteLine("========= BUSCAR CONTATO =========");
    Console.Write("Digite o email ou ID do usuário a ser buscado ");
    string email = Console.ReadLine();
    try
    {
        GetExtendedContactDetails result = apiInstance.GetContactInfo(email);
        if (result is not null)
        {
            Console.WriteLine("Contato encontrado com sucesso! \n");
            Console.WriteLine(result.ToJson());
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}

static void ListarAtributos(ContactsApi apiInstance)
{
    Console.WriteLine("========= LISTAR ATRIBUTOS PREENCHÍVEIS PARA O CONTATO =========");
    try
    {
        GetAttributes result = apiInstance.GetAttributes();
        // Filtrar os atributos pela categoria NORMAL E CATEGORY
        List<GetAttributesAttributes> atributosNormaisEcategoria = result.Attributes
            .Where(x => x.Category == GetAttributesAttributes.CategoryEnum.Normal || x.Category == GetAttributesAttributes.CategoryEnum.Category)
            .ToList();

        foreach (var atributo in atributosNormaisEcategoria)
        {
            Console.WriteLine($"Nome do atributo: {atributo.Name}");
            Console.WriteLine($"Tipo do atributo: {atributo.Type}");
            Console.WriteLine($"Categoria do atributo: {atributo.Category}");

            if (atributo.Category == GetAttributesAttributes.CategoryEnum.Category)
            {
                Console.WriteLine($"Subcategorias do atributo: ");
                foreach (var subcategoria in atributo.Enumeration)
                {
                    Console.WriteLine($"- {subcategoria.Label}");
                }
            }

            Console.WriteLine();
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.ReadLine();
    }
}

static void CriarContatoComAtributosDisponiveis(ContactsApi apiInstance)
{
    Console.WriteLine("========= CRIAR CONTATO =========");

    try
    {
        Console.WriteLine("Digite o e-mail do contato:");
        string email = Console.ReadLine();

        // Obter os atributos preenchíveis do usuário
        GetAttributes attributeResult = apiInstance.GetAttributes();
        List<string> attributeNames = attributeResult.Attributes.Where(x => x.Category == GetAttributesAttributes.CategoryEnum.Normal || x.Category == GetAttributesAttributes.CategoryEnum.Category).Select(a => a.Name).ToList();

        Dictionary<string, object> attributes = new Dictionary<string, object>();

        foreach (var attributeName in attributeNames)
        {
            Console.WriteLine($"Digite o valor para o atributo '{attributeName}':");
            string attributeValue = Console.ReadLine();
            if (bool.TryParse(attributeValue, out bool atributoValorBool))
            {
                attributes.Add(attributeName, atributoValorBool);
            }
            else
            {
                attributes.Add(attributeName, attributeValue);
            }
        }

        // Criando um objeto de contato
        CreateContact contato = new CreateContact
        {
            Email = email,
            ListIds = new List<long?> { 1 }, // IDs das listas de contatos às quais você deseja adicionar o contato (caso tenha)
            UpdateEnabled = true, // Defina como true para atualizar o contato se ele já existir ou false para criar um novo contato
            Attributes = attributes
        };

        // Criando o contato
        CreateUpdateContactModel result = apiInstance.CreateContact(contato);
        Console.WriteLine("Contato criado com sucesso. ID do contato: " + result.Id);
    }
    catch (Exception e)
    {
        Console.WriteLine("Exception when calling ContactsApi.CreateContact: " + e.Message);
    }
}