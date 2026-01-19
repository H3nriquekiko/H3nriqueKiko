# Checklist (MVP)

Sistema web de checklist (MVP) usando ASP.NET Core Razor Pages, EF Core, Identity e PostgreSQL.

## Pré-requisitos

- .NET SDK 8.x
- Docker + Docker Compose

## Como rodar do zero

1. Suba o banco PostgreSQL:
   ```bash
   docker compose up -d
   ```
2. Restaure dependências:
   ```bash
   dotnet restore Checklist.Web
   ```
3. Execute o projeto:
   ```bash
   dotnet run --project Checklist.Web
   ```
4. Acesse:
   - Login: <http://localhost:5000/Account/Login>

## Credenciais seed (Admin)

- **Email:** admin@local
- **Senha:** Admin1234

## Fluxo básico

1. Admin cria um checklist e seus itens em `/Admin/Checklists`.
2. Em `/Today`, clique em **Executar** para iniciar a execução do checklist.
3. Em `/Run/{id}`, marque os itens e finalize.
4. Consulte em `/History`.
