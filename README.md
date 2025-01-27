# CRUD API

Esta é uma API CRUD desenvolvida em C# utilizando ASP.NET, EF Core e SQLite. A API permite a criação, leitura, atualização e exclusão de  produtos, itens e carrinhos.

## Endpoints

### Produtos

- **GET /v1/products**: Retorna todos os produtos.
- **GET /v1/products/{id}**: Retorna um produto específico pelo ID.
- **POST /v1/products**: Cria um novo produto.
- **PUT /v1/products/{id}**: Atualiza um produto existente pelo ID.
- **DELETE /v1/products/{id}**: Deleta um produto existente pelo ID.

### Itens

- **GET /v1/items**: Retorna todos os itens.
- **GET /v1/items/{id}**: Retorna um item específico pelo ID.
- **POST /v1/items**: Cria um novo item.
- **PUT /v1/items/{id}**: Atualiza um item existente pelo ID.
- **DELETE /v1/items/{id}**: Deleta um item existente pelo ID.

### Carrinhos

- **GET /v1/carts**: Retorna todos os carrinhos.
- **GET /v1/carts/{id}**: Retorna um carrinho específico pelo ID.
- **POST /v1/carts**: Cria um novo carrinho.
- **PUT /v1/carts/{id}**: Atualiza um carrinho existente pelo ID.
- **DELETE /v1/carts/{id}**: Deleta um carrinho existente pelo ID.

## Exceções

A API retorna mensagens de erro apropriadas para as seguintes situações:

- **404 Not Found**: Quando um item ou carrinho não é encontrado.
- **400 Bad Request**: Quando há um erro na requisição, como dados inválidos ou quantidade insuficiente de itens.
  
## Configuração

Para configurar e executar a API, siga os passos abaixo:

1. Clone o repositório.
2. Navegue até o diretório do projeto.
3. Execute `dotnet restore` para restaurar as dependências.
4. Execute `dotnet run` para iniciar a aplicação.

## Configuração de CORS

Se você precisar consumir a API de uma origem diferente, deve atualizar a configuração de CORS no arquivo `Program.cs`.

A configuração atual permite requisições apenas da origem `http://localhost:5173`. 
Para permitir uma nova origem, substitua `http://localhost:5173` pelo endereço desejado. Por exemplo, para permitir `http://example.com`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "DefaultPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
```
Certifique-se de reiniciar a aplicação após alterar as configurações.


## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.
