# 🚗 FourPrime – Plataforma Integrada de Concessionária de Veículos

[![.NET](https://img.shields.io/badge/.NET-10.0%20%7C%209.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0.2-512BD4?logo=nuget&logoColor=white)](https://docs.microsoft.com/ef/core/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## 📌 Sobre o Projeto

O **FourPrime** é um sistema completo e integrado de gestão e catálogo para concessionária de veículos de luxo e seminovos. 

O projeto foi desenvolvido como **Projeto Integrador** do curso **Técnico em Informática do SENAC**. A plataforma foi fortemente **inspirada na T-Car**, referência no mercado de comércio de veículos.

O ecossistema é composto por uma **Aplicação Web (MVC)** para clientes e administradores, uma **Web API RESTful** para integração e um **Sistema Desktop (Windows Forms)** para gestão rápida e administrativa de catálogo e estoque.

---

### 🧱 Camadas da Solução:

1. **`FourPrime.Domain` (Domínio)**
   * **Responsabilidade**: Coração do sistema. Contém as entidades de negócio (`Carro`, `Categoria`, `Marca`, `Usuario`, `Sessao`), objetos de valor, *enums* e contratos/interfaces essenciais. Não possui dependência de nenhum outro projeto ou framework externo.
2. **`FourPrime.Application` (Aplicação)**
   * **Responsabilidade**: Casos de uso e regras de negócio da aplicação. Armazena os DTOs (*Data Transfer Objects*), contratos de serviços (`AutenticacaoService`, `CarroQueryService`, `CatalogLookupServices`) e abstrações.
3. **`FourPrime.Infrastructure` (Infraestrutura)**
   * **Responsabilidade**: Acesso a dados e serviços externos. Implementa o contexto do banco de dados `AppDbContext` via Entity Framework Core, repositórios concretos (`CarroRepository`, `CategoriaRepository`, `MarcaRepository`, `UsuarioRepository`), criptografia de senhas, migrações e *seeding* inicial de dados.
4. **`FourPrime.Api` (Web API)**
   * **Responsabilidade**: API RESTful para integração entre sistemas. Implementa autenticação baseada em **JWT (JSON Web Token)**, controle de CORS, OpenAPI/Swagger UI e mapeamento de rotas para recursos.
5. **`FourPrime.Web` (Plataforma Web)**
   * **Responsabilidade**: Interface web interativa voltada para clientes finais e painel administrativo web (`/Admin/Dashboard`). Desenvolvida em **ASP.NET Core MVC**, utilizando **Bootstrap 5**, JavaScript reativo, autenticação via cookies e suporte a **Google OAuth 2.0**.
6. **`FourPrime.Ul` (Aplicação Desktop)**
   * **Responsabilidade**: Software de gestão administrativa desktop para rápida manipulação do estoque e cadastros. Desenvolvido em **Windows Forms** com a suíte visual **Guna.UI2.WinForms**, proporcionando telas modernas com suporte a tema escuro.

---

## 🛠️ Tecnologias e Frameworks Utilizados

### **Back-end & Core**
* **Linguagem**: C# 13 / .NET 10.0 & .NET 9.0
* **Framework Web**: ASP.NET Core MVC & ASP.NET Core Web API
* **ORM & Banco de Dados**: Entity Framework Core 9.0.2 com **SQL Server**
* **Autenticação & Segurança**: 
  * ASP.NET Core Identity
  * JWT Bearer Tokens (para a API)
  * Cookie Authentication & Google OAuth 2.0 (para a Web)
  * Criptografia / Hashing seguro de senhas (BCrypt / SHA256)
* **Documentação da API**: Swagger / OpenAPI (Swashbuckle)
* **Injeção de Dependências**: Native .NET IoC (`Microsoft.Extensions.DependencyInjection`)

### **Front-end & Interfaces**
* **Web**:
  * HTML5, CSS3, JavaScript (ES6+)
  * **Bootstrap 5** (Layouts responsivos e componentes flexíveis)
  * FontAwesome & Bootstrap Icons
* **Desktop (WinForms)**:
  * Windows Forms (.NET 10.0-windows)
  * **Guna.UI2.WinForms 2.0.4.7** (Componentes visuais modernos)

---

## 🚀 Como Executar o Projeto

### 📋 Pré-requisitos
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [SQL Server](https://www.microsoft.com/sql-server/) ou **SQL Server Express LocalDB** (incluso com o Visual Studio)
* IDE recomendada: **Visual Studio 2022 / 2026** ou **VS Code** com extensão C# Dev Kit.

### 🔧 Passos para Inicialização

1. **Clonar o Repositório**:
   ```bash
   git clone https://github.com/pedronicollas2024-rgb/Projeto-FourPrime.git
   cd Projeto-FourPrime
   ```

2. **Configurar a String de Conexão**:
   Certifique-se de que a string de conexão no `appsettings.json` dos projetos `FourPrime.Web`, `FourPrime.Api` e `FourPrime.Ul` aponta para o seu servidor SQL Server:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FourPrimeDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Restaurar Dependências e Compilar**:
   ```bash
   dotnet restore FourPrime.sln
   dotnet build FourPrime.sln
   ```

4. **Aplicar Migrações do Banco de Dados**:
   O banco de dados é inicializado automaticamente na primeira execução através do `DatabaseInitializer` e `DataBaseSeeder`. Caso queira atualizar manualmente via EF CLI:
   ```bash
   dotnet ef database update --project FourPrime.Infrastructure --startup-project FourPrime.Api
   ```

5. **Configuração de Inicialização no Visual Studio / Visual Studio Insiders**:
   Para rodar a API, o Site Web e a Aplicação Desktop integrados de uma só vez:
   1. Clique com o botão direito na **Solução 'FourPrime'** e selecione **"Configurar Projetos de Inicialização..."** (*Set Startup Projects...*).
   2. Marque a opção **"Vários projetos de inicialização"** (*Multiple startup projects*).
   3. Configure as ações e o perfil de execução em **HTTP**:
      * **`FourPrime.Api`**: Ação = **Iniciar** | Perfil = **`http`** (Porta `5138`)
      * **`FourPrime.Web`**: Ação = **Iniciar** | Perfil = **`http`** (Porta `5043`)
      * **`FourPrime.Ul`**: Ação = **Iniciar**
   4. Clique em **Aplicar** e pressione **F5** (ou no botão de Play ▶️).

6. **Execução via Terminal (CLI)**:
   * **Executar o Site Web**:
     ```bash
     dotnet run --project FourPrime.Web
     ```
     Acesse no navegador: `http://localhost:5043`
   * **Executar a Web API**:
     ```bash
     dotnet run --project FourPrime.Api
     ```
     Documentação Swagger em: `http://localhost:5138/swagger`
   * **Executar a Aplicação Desktop (WinForms)**:
     ```bash
     dotnet run --project FourPrime.Ul
     ```

## 📄 Licença

Este projeto é de fins acadêmicos e educacionais. Sinta-se à vontade para utilizar como referência ou aprendizado.
