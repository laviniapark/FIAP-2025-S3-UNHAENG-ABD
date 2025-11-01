# Projeto de Gestão da Mottu

📌 Nota: Projeto desenvolvido para fins acadêmicos na disciplina de Advanced Business Development with .NET

Este é um sistema desenvolvido em ASP.NET Core Minimal API para o cadastro e gerenciamento de motos, funcionários e filiais da Mottu.

A aplicação permite realizar CRUD completo (criação, listagem, edição e exclusão) para todas as entidades do domínio, garantindo controle centralizado e eficiente das informações.

Além disso, conta com recursos de versionamento de API, suporte a idempotência, HealthChecks e Rate Limiting, proporcionando maior confiabilidade, estabilidade e padronização para o consumo dos serviços.

## Índice

- [Integrantes](#integrantes)
- [Justificativas da Arquitetura](#justificativas-da-arquitetura)
- [Funcionalidades](#funcionalidades)
- [Como Rodar o Projeto](#como-rodar-o-projeto)
- [Efetuar Testes no Programa](#efetuar-testes-no-programa)
- [Fontes](#fontes)

## Integrantes

RM555679 - Lavinia Soo Hyun Park

## Justificativas da Arquitetura

A aplicação utiliza ASP.NET Core Minimal API por ser mais leve e adequada para CRUDs simples. O Entity Framework Core com Oracle facilita o mapeamento objeto-relacional e garante compatibilidade com o ambiente corporativo. Além disso, a separação em camadas inspirada na Clean Architecture melhora a manutenção e evolução do sistema. Recursos como versionamento de API, idempotência, healthchecks e rate limiting foram adotados por seguirem boas práticas de mercado para aplicações mais seguras e resilientes.

## Funcionalidades

- Cadastro de filiais, motos e funcionários
- Integração com o banco Oracle utilizando Entity Framework Core
- Documentação da API interativa gerada com Scalar (OpenAPI)
- Versionamento de API para suportar evolução e compatibilidade entre versões
- Idempotência na requisição de Cadastro para evitar operações duplicadas acidentais
- HealthChecks e monitoramento do banco Oracle para verificar disponibilidade
- Rate Limiting para controle de consumo e proteção contra sobrecarga
- Relacionamentos entre entidades (ex.: motos vinculadas a uma filial, funcionário associado a filial)

## Como Rodar o Projeto

**IMPORTANTE**

Clone o repositório e dê `git clone [link repositorio]` na pasta desejada para poder rodá-lo em sua IDE

### 1. Requisitos

- .NET SDK 9.0 instalado (https://dotnet.microsoft.com/en-us/download)
- Oracle XE local ou acesso ao banco da sua instituição
- IDE: Visual Studio ou Rider (ou VS Code)
- API Client: Insomnia (utilizado nesse projeto), Postman ou outro de sua preferência

### 2. Configuração da conexão com o Banco de Dados

No arquivo `appsettings.json`, configure sua conexão Oracle:

```
"ConnectionStrings": {
    "DefaultConnection": "Data Source=[ORACLE-URL]:1521/[ORACLE-HOST];User Id=[ORACLE-USER];Password=[ORACLE-PASSWORD]"
  }
```

**Os campos que precisam ser preenchidos estão definidos por colchetes**

### 3. Executando o Projeto (Utilizando o terminal)

1. Restaure os pacotes

```
dotnet restore
```

2. Rode as migrations (ele irá criar automaticamente as tabelas no banco de dados)

```
dotnet ef database update
```

3. Inicie o servidor

```
dotnet run
```

4. Copie o link da URL gerado pelo .NET (ex.: http://localhost:5011)

## Efetuar testes no programa

Vamos primeiro ter uma visão geral dos endpoints do projeto.

Cole a URL gerada pelo .NET no navegador juntamente com o caminho do Scalar (http://localhost:5011/scalar). Irá aparecer a seguinte página:

![Scalar](/ManagementApp/docs/images/SCALAR.png)

Pelo Scalar, você poderá visualizar todos os endpoints disponíveis para cada tabela existente no projeto, incluindo descrições de o que cada método faz, e quais são os tipos de dados aceitos por cada atributo.

### 1. Verificar saúde do projeto / banco

Aqui nós vamos utilizar o Health Check para ver se a aplicação está rodando corretamente e se o banco de dados Oracle está acessível. Ele funciona como um check-up rápido do sistema, mostrando se os principais serviços estão no ar.

```
http://localhost:5011/health
```

![Health Resposta](/ManagementApp/docs/images/HEALTH-CHECK.png)

**Agora iremos para o CRUD das tabelas, utilizaremos como base os prints da tabela "Filial", porém em cada tópico terá dados exemplos prontos e instruções de cada requisição**

### 2. Ver a lista de dados cadastrados em cada tabela (GET)

Assim que você efetuou a Migration, o projeto ja subiu **10 dados prontos** para o Banco. Iremos primeiro visualizar eles.

Para isto, vamos usar a URL http://localhost:5011/api/v1/filiais?PageNumber=1&PageSize=2

**É crucial manter o caminho /api/v1 para todas as operações que formos fazer!**

Dados Alteráveis:
- ```/api/v1/[tabela]``` = filiais, motos ou funcionarios
- PageNumber = use por padrao 1 (para começar na primeira pagina)
- PageSize = escolha a quantidade de registros que você quer visualizar por página

![Resposta do Metodo GETALL de Filiais](/docs/images/FILIAIS-GETALL-PAGINATED.png)

Como podemos ver na imagem acima, o endpoint retorna os dados utilizando **paginação**.
Isso significa que:
- Os registros são divididos em páginas.
- A quantidade de registros na pagina é o mesmo que o valor que definimos em PageSize
- A resposta também inclui informações adicionais, como links para próxima página, página anterior, etc.

### 3. Listar um item pelo ID (GET)

Utilize a URL: http://localhost:5011/api/v1/filiais/{id}

**Não esqueça de pegar o ID desejado acessando o GET ALL**

![Resposta do metodo GETBYID de Filiais](/docs/images/FILIAIS-GETBYID-200.png)

No resultado do GET by Id, além dos dados do registro solicitado, a resposta também traz links adicionais que indicam quais ações podem ser realizadas a partir daquele recurso.

Isso segue o conceito de HATEOAS (Hypermedia as the Engine of Application State), onde a própria API guia o usuário sobre os próximos passos possíveis.

No caso das filiais, por exemplo, a resposta inclui links para:
- GET by CNPJ da mesma filial
- PUT (atualizar a filial)
- DELETE (encerrar a filial)

Esses links facilitam a navegação na API, tornando mais claro quais operações estão disponíveis sem precisar consultar separadamente a documentação.

***Todas as 3 tabelas possuem uma busca de registro específica (CNPJ, CPF e PLACA), para saber como testar veja a URL pelo HATEOAS do GETBYID de cada um ou acessando o Scalar***

### 4. Cadastrar um novo registro (POST)

Utilize a URL: http://localhost:5011/api/v1/filiais

No POST teremos um mini passo-a-passo:

1. Adicione na aba Headers do API Client = ```Idempotency-Key: 3b8d0c71-4100-4b47-9324-32c0e91eab49```

**Essa Key deverá ser mudada toda vez que você for fazer um novo POST, para isso basta mudar o numero final da Key**

2. Acesse a aba Body, selecione tipo de dado JSON, e insira os dados a serem cadastrados

Deixarei como exemplo 1 registro novo para cada tabela:

FILIAIS
```
{
	"nome": "Filial Lins",
	"cnpj": "23897364000123",
	"telefone": "(11) 4002-1010",
	"dataAbertura": "2024-09-19T00:00:00",
	"dataEncerramento": null,
	"endereco": {
		"cep": "01152-000",
		"logradouro": "Av. Lins de Vasconcelos",
		"numero": "10",
		"complemento": null,
		"bairro": "Vila Mariana",
		"cidade": "São Paulo",
		"uf": "SP",
		"pais": "Brasil"
	}
}
```

MOTOS
```
{
	"placa": "LIN3S77",
	"marca": "Fiap",
	"modelo": "TI 456",
	"ano": 2024,
	"status": "disponivel",
	"filialId": "[ID DA FILIAL DESEJADA]"
}
```

FUNCIONARIOS
```
{
	"nomeCompleto": "Joao Lins de Vasconcelos",
	"cpf": "57689375910",
	"cargo": "gestor",
	"ativo": true,
	"filialId": "[ID DA FILIAL DESEJADA]"
}
```

***Para as tabelas MOTO e FUNCIONARIO, é obrigatório cadastrar um registro com o ID de uma FILIAL ja existente.***

3. Envie os dados

![Resposta do metodo POST de Filiais](/ManagementApp/docs/images/FILIAIS-POST-201.png)


### 5. Atualizar um Registro (PUT)

Utilize a URL: http://localhost:5011/api/v1/filiais/{id}

Para o metodo PUT, copie todos os atributos do registro (juntamente com os dados originais), e mude os que você deseja atualizar **(não inclua o ID do proprio registro, inicie a partir do segundo atributo)**

![Resposta do metodo PUT de Filiais](/ManagementApp/docs/images/FILIAIS-PUT-200.png)

### 6. Deletar um Registro (DELETE)

Utilize a URL: http://localhost:5011/api/v1/filiais/{id}

**OBS.: para a tabela FILIAIS, o endereço URL para deletar inclui ```/encerrar```, para mais detalhes consulte o Scalar**

![Resposta do metodo DELETE de filial](/ManagementApp/docs/images/FILIAIS-DELETE-200.png)

## Fontes

Paginação: https://osamadev.medium.com/implementing-pagination-in-net-api-a-simple-guide-fd7b85103739

HATEOAS: https://poornimanayar.co.uk/blog/minimal-apis-and-hateoas/
