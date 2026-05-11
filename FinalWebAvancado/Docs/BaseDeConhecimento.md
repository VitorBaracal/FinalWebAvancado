# CORS, DTOs e JWT — guia simples

*(Em cada bloco de código, o número à **esquerda** é a linha no ficheiro `.cs` do projeto — facilita bater certo com as tabelas.)*

---

## Sumário

1. [Parte A — Resumo rápido (visão geral)](#parte-a)
2. [Parte B — CORS no `Program.cs`](#parte-b)
3. [Parte B2 — `Program.cs`: resto do arranque](#parte-b2)
4. [Parte C — `TokenService.cs`](#parte-c)
5. [O que são *claims*](#claims-jwt)
6. [Parte D — `AuthController.cs`](#parte-d)
7. [Parte E — Anotações nos controllers](#parte-e)
   - [E.1 — `AuthController`](#parte-e1)
   - [E.2 — `TaskController.cs`](#parte-e2)
   - [E.3 — `UserController.cs`](#parte-e3)
   - [E.4 — Resumo dos três](#parte-e4)
8. [Parte F — DTOs, anotações e `ModelState`](#parte-f)
9. [Parte G — Lista de tasks com categorias (join SQL / LINQ)](#parte-g)

---

<a id="parte-a"></a>

## Parte A — Resumo rápido (visão geral)

| Tema | Em uma frase |
|------|----------------|
| **CORS** | O browser só deixa o teu front chamar a API se a API disser que essa origem (URL do front) é permitida. |
| **DTOs** | Classes só para mandar/receber JSON; não expões a base de dados “crua” nem misturas validação com o modelo da BD. |
| **JWT** | Depois do login a API devolve um token; nos pedidos seguintes o cliente envia esse token e a API valida sem pedir password outra vez. |

---

<a id="parte-b"></a>

## Parte B — CORS no `Program.cs` *(só o trecho de CORS)*

Trecho retirado de `Program.cs` (registo da política + uso na pipeline):

```csharp
 21 | builder.Services.AddCors(options =>{
 22 |     options.AddPolicy("FullAccess", policy =>
 23 |         policy.WithOrigins("https://localhost:3000")
 24 |               .AllowAnyHeader()
 25 |               .AllowAnyMethod());
 26 | });
```

```csharp
 73 | app.UseCors("FullAccess");
```

| Linha(s) | O que faz |
|----------|-----------|
| **21–26** | `AddCors`: política **`"FullAccess"`** — só **`https://localhost:3000`** pode chamar a API; `AllowAnyHeader()` e `AllowAnyMethod()` liberam cabeçalhos e verbos (GET, POST, PUT, DELETE, …). |
| **73** | `UseCors("FullAccess")`: na pipeline HTTP, **ativa** essa política em cada pedido. |

**Nota:** se o front usar outra URL ou `http`, ajusta `WithOrigins(...)` na linha 23.

---

<a id="parte-b2"></a>

## Parte B2 — `Program.cs`: resto do arranque

Aqui fica o que o `Program.cs` configura **para além do bloco CORS** (base de dados, JWT, Swagger, serviços, ordem da pipeline). O CORS em detalhe está na **Parte B**.

`Program.cs` completo, com números de linha:

```csharp
  1 | using Microsoft.AspNetCore.Authentication.JwtBearer;
  2 | using Microsoft.IdentityModel.Tokens;
  3 | using Microsoft.OpenApi.Models;
  4 | using System.Text;
  5 | using Api.Data;
  6 | using Microsoft.EntityFrameworkCore;
  7 | using Api.Services;
  8 |
  9 | /**
 10 |  * Vitor Baraçal Guimarães - 40457125
 11 |  * João Vitor Pereira Borges -38146762
 12 |  * Cauã Tobias de Souza Proença - 40174255
 13 |  */
 14 |
 15 | var builder = WebApplication.CreateBuilder(args);
 16 |
 17 | var conn = builder.Configuration.GetConnectionString("DefaultConn");
 18 |
 19 | builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
 20 |
 21 | builder.Services.AddCors(options =>{
 22 |     options.AddPolicy("FullAccess", policy =>
 23 |         policy.WithOrigins("https://localhost:3000")
 24 |               .AllowAnyHeader()
 25 |               .AllowAnyMethod());
 26 | });
 27 |
 28 | builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 29 |     .AddJwtBearer(options =>{
 30 |         var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
 31 |
 32 |         options.TokenValidationParameters = new TokenValidationParameters{
 33 |             ValidateIssuer = false,   
 34 |             ValidateAudience = false, 
 35 |             ValidateIssuerSigningKey = true,
 36 |             IssuerSigningKey = new SymmetricSecurityKey(key),
 37 |             ClockSkew = TimeSpan.Zero 
 38 |         };
 39 |     });
 40 |
 41 | builder.Services.AddControllers();
 42 | builder.Services.AddEndpointsApiExplorer();
 43 |
 44 | builder.Services.AddSwaggerGen(c => {
 45 |     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
 46 |         Type = SecuritySchemeType.Http,
 47 |         Scheme = "bearer",
 48 |         BearerFormat = "Jwt"
 49 |     });
 50 |
 51 |     c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
 52 |             new OpenApiSecurityScheme{
 53 |                 Reference = new OpenApiReference{
 54 |                     Type = ReferenceType.SecurityScheme,
 55 |                     Id = "Bearer"
 56 |                 }
 57 |             },
 58 |             Array.Empty<string>()
 59 |         }
 60 |     });
 61 | });
 62 |
 63 | builder.Services.AddSingleton<TokenService>(); 
 64 |
 65 | var app = builder.Build();
 66 |
 67 | if (app.Environment.IsDevelopment()){
 68 |     app.UseSwagger();
 69 |     app.UseSwaggerUI();
 70 | }
 71 |
 72 | app.UseHttpsRedirection();
 73 | app.UseCors("FullAccess");
 74 | app.UseAuthentication();
 75 | app.UseAuthorization();
 76 | app.MapControllers();
 77 |
 78 | app.Run();
```

### O que mais vale a pena saber (linha a linha / blocos)

| Linha(s) | O que faz |
|----------|-----------|
| **1–7** | `using`: JWT Bearer, tokens, OpenAPI/Swagger, texto↔bytes, `AppDbContext`, EF Core, `TokenService`. |
| **15** | `WebApplication.CreateBuilder`: início da app ASP.NET Core e acesso à configuração (`appsettings`). |
| **17–19** | Lê **connection string** `DefaultConn` e regista o **EF Core** com **MySQL** (Pomelo + `ServerVersion.AutoDetect`). |
| **21–26** | **CORS** — vê a **Parte B** (explicação dedicada). |
| **28–39** | **JWT no servidor:** `AddAuthentication` + `AddJwtBearer`; lê **`Jwt:Key`**, valida assinatura; issuer/audience desligados para simplificar; `ClockSkew = Zero`. |
| **41–42** | Ativa **controllers** (as tuas classes `[ApiController]`) e o explorador mínimo para Swagger. |
| **44–61** | **Swagger:** define esquema **Bearer** e exige-o na UI de testes — útil para colar o token nos endpoints protegidos. |
| **63** | **Singleton** do `TokenService` — uma instância para gerar JWT no login. |
| **65** | `Build()` — fecha a fase de configuração e cria a app. |
| **67–70** | Em **Development**, liga página **Swagger / Swagger UI** (documentação e “Try it out”). |
| **72** | Redireciona pedidos HTTP para HTTPS (quando aplicável). |
| **73** | **CORS** na pipeline — vê **Parte B**. |
| **74–75** | **`UseAuthentication`** identifica o utilizador pelo JWT; **`UseAuthorization`** aplica **`[Authorize]`** / políticas. |
| **76** | **`MapControllers`** — publica as rotas dos teus controllers. |
| **78** | **`Run()`** — arranca o servidor (Kestrel). |

**Ordem útil de lembrar:** depois de construir `app`, na pipeline costuma importar: HTTPS → **CORS** → **Authentication** → **Authorization** → **MapControllers** → **Run**.

---

<a id="parte-c"></a>

## Parte C — `Services/TokenService.cs`

```csharp
  1 | using System.IdentityModel.Tokens.Jwt;
  2 | using Microsoft.IdentityModel.Tokens;
  3 | using System.Security.Claims;
  4 | using System.Text;
  5 |
  6 | namespace Api.Services;
  7 |
  8 | public class TokenService {
  9 |     private readonly IConfiguration _config;
 10 |
 11 |     public TokenService(IConfiguration config) {
 12 |         _config = config;
 13 |     }
 14 |
 15 |     public string GenerateToken(string username) {
 16 |         var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
 17 |         var claims = new[] {
 18 |             new Claim(ClaimTypes.Name, username)
 19 |         };
 20 |
 21 |         var token = new JwtSecurityToken(
 22 |             claims: claims,
 23 |             expires: DateTime.UtcNow.AddHours(8),
 24 |             signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
 25 |         );
 26 |
 27 |         return new JwtSecurityTokenHandler().WriteToken(token);
 28 |     }
 29 | }
```

| Linha(s) | O que faz |
|----------|-----------|
| **1–4** | Bibliotecas JWT, claims, chave simétrica, bytes. |
| **8–13** | Classe + construtor com `IConfiguration` (lê `Jwt:Key`). |
| **15–27** | `GenerateToken`: claims com o login, expiração 8 h, assinatura HMAC-SHA256. |
| **27** | Devolve o JWT em **string**. |

<a id="claims-jwt"></a>

### O que são *claims* (no teu código, linhas 17–19)?

**Claim** = uma **afirmação** que o servidor **coloca dentro do JWT**, no formato **tipo + valor**.

- Pensa em “campos de identidade” que viajam **dentro do token** (não são a password; são dados que a API decidiu guardar no token depois do login bem-sucedido).
- Cada claim tem um **nome** (tipo) e um **valor**. No projeto usas **`ClaimTypes.Name`** com o valor **`username`** (o login do utilizador).

No código:

```csharp
var claims = new[] {
    new Claim(ClaimTypes.Name, username)
};
```

Isto significa: “este token representa alguém cujo **nome de utilizador** (no sentido do ASP.NET / `User.Identity`) é o **login** passado ao `GenerateToken`”.

**Para que serve:** quando um pedido chega com o Bearer token, o middleware de autenticação lê o JWT, valida a assinatura e **reconstrói o utilizador** a partir das claims. Assim o código pode usar coisas como o nome do utilizador associado ao pedido (em APIs mais ricas poderias ter mais claims, por exemplo id do utilizador ou email — aqui só precisas do login).

**Importante (bom saber):** o conteúdo “legível” do JWT (incluindo as claims) **não é encriptado** — quem tem o token consegue decodificar e **ler** esses valores. A **segurança** vem da **assinatura**: se alguém alterar uma claim, a assinatura deixa de bater certo e o servidor **rejeita** o token. Por isso **não** se mete dados secretos (password, dados médicos, etc.) nas claims.

---

<a id="parte-d"></a>

## Parte D — `Controllers/AuthController.cs`

```csharp
  1 | using Api.Data;
  2 | using Api.Dto;
  3 | using Api.Enums;
  4 | using Api.Services;
  5 | using Microsoft.AspNetCore.Mvc;
  6 | using Microsoft.EntityFrameworkCore;
  7 | using static BCrypt.Net.BCrypt;
  8 |
  9 | namespace Api.Controllers;
 10 |
 11 | [ApiController]
 12 | [Route("api/[controller]")]
 13 | public class AuthController : ControllerBase
 14 | {
 15 |     private readonly AppDbContext _context;
 16 |     private readonly TokenService _tokenService;
 17 |
 18 |     public AuthController(AppDbContext context, TokenService tokenService) {
 19 |         _context = context;
 20 |         _tokenService = tokenService;
 21 |     }
 22 |
 23 |     [HttpPost("login")]
 24 |     public async Task<IActionResult> Login(LoginDto dto) {
 25 |
 26 |         var user = await _context.User
 27 |             .FirstOrDefaultAsync(u => u.Login == dto.Login);
 28 |
 29 |         if (user is null)
 30 |             return Unauthorized(new { message = EnumMessageReponse.InvalidLogin });
 31 |
 32 |         if (!Verify(dto.Password, user.PasswordHash))
 33 |             return Unauthorized(new { message = EnumMessageReponse.InvalidLogin });
 34 |
 35 |         var token = _tokenService.GenerateToken(user.Login);
 36 |
 37 |         return Ok(new {
 38 |             token,
 39 |             name = user.Name,
 40 |             user = user.Login
 41 |         });
 42 |     }  
 43 |
 44 | }
```

| Linha(s) | O que faz |
|----------|-----------|
| **11–12** | `[ApiController]` + rota **`/api/Auth`**. |
| **15–21** | Injeta BD + `TokenService`. |
| **23–24** | **POST** `api/Auth/login`. |
| **26–33** | Procura utilizador; valida password (BCrypt); falhas → **401**. |
| **35** | Gera JWT. |
| **37–41** | **200** com `token`, `name`, `user`. |

**Importante:** **não há `[Authorize]`** nesta classe — o login tem de ser **público**.

---

<a id="parte-e"></a>

## Parte E — Anotações: `AuthController` vs `TaskController` vs `UserController`

| Controller | No topo da classe | Efeito |
|------------|-------------------|--------|
| **AuthController** | Só `[ApiController]` e `[Route]` — **sem** `[Authorize]` | Tudo **sem** token. |
| **TaskController** | `[Authorize]` | **Todos** os métodos exigem JWT. |
| **UserController** | `[Authorize]` + **`[AllowAnonymous]`** no **POST** (registo) | Por defeito JWT; **uma** ação pública. |

<a id="parte-e1"></a>

### E.1 — `AuthController`

Código na **Parte D** — falta de `[Authorize]` = acesso sem Bearer.

<a id="parte-e2"></a>

### E.2 — `Controllers/TaskController.cs`

```csharp
  1 | using Microsoft.AspNetCore.Authorization;
  2 | using Microsoft.AspNetCore.Mvc;
  3 | using Microsoft.EntityFrameworkCore;
  4 | using Api.Data;
  5 | using Api.Dto;
  6 | using Api.Enums;
  7 |
  8 | namespace Api.Controllers;
  9 |
 10 | [Authorize]
 11 | [ApiController]
 12 | [Route("api/[controller]")]
 13 | public class TaskController : ControllerBase
 14 | {
 15 |     private readonly AppDbContext _context;
 16 |
 17 |     public TaskController(AppDbContext context) {
 18 |         _context = context;
 19 |     }
 20 |
 21 |     [HttpGet]
 22 |     public async Task<ActionResult<IEnumerable<TaskDto>>> ListTasksAsync()
 23 |     {
 24 |         var tasks = await _context.Tasks.OrderBy(t => t.Id).ToListAsync();
 25 |
 26 |         var taskIds = tasks.Select(t => t.Id).ToList();
 27 |
 28 |         var linksWithCategories = await (
 29 |             from tc in _context.TaskCategories
 30 |             join c in _context.Categories on tc.CategoryId equals c.Id
 31 |             where taskIds.Contains(tc.TaskId)
 32 |             orderby tc.TaskId, c.Id
 33 |             select new { tc.TaskId, c.Id, c.UserId, c.Name, c.ColorHex }
 34 |         ).ToListAsync();
 35 |
 36 |         var response = tasks.Select(t => new TaskDto {
 37 |             Id = t.Id,
 38 |             UserId = t.UserId,
 39 |             Name = t.Name,
 40 |             Description = t.Description,
 41 |             Level = t.Level,
 42 |             Status = t.Status,
 43 |             Categories = linksWithCategories
 44 |                 .Where(x => x.TaskId == t.Id)
 45 |                 .Select(x => new CategoryDto {
 46 |                     Id = x.Id,
 47 |                     UserId = x.UserId,
 48 |                     Name = x.Name,
 49 |                     ColorHex = x.ColorHex
 50 |                 })
 51 |                 .ToList(),
 52 |             CreatedAt = t.CreatedAt,
 53 |             UpdatedAt = t.UpdatedAt
 54 |         });
 55 |
 56 |         return Ok(response);
 57 |     }
 58 |
 59 |     [HttpGet("{id:int}", Name = "GetTaskById")]
 60 |     public async Task<IActionResult> GetByIdAsync(int id) {
 61 |
 62 |         var task = await _context.Tasks.FindAsync(id);
 63 |
 64 |         if (task == null) return NotFound();
 65 |
 66 |         var categories = await (
 67 |             from tc in _context.TaskCategories
 68 |             join c in _context.Categories on tc.CategoryId equals c.Id
 69 |             where tc.TaskId == id
 70 |             orderby c.Id
 71 |             select new CategoryDto {
 72 |                 Id = c.Id,
 73 |                 UserId = c.UserId,
 74 |                 Name = c.Name,
 75 |                 ColorHex = c.ColorHex
 76 |             }).ToListAsync();
 77 |
 78 |         return Ok(new TaskDto {
 79 |             Id = task.Id,
 80 |             UserId = task.UserId,
 81 |             Name = task.Name,
 82 |             Description = task.Description,
 83 |             Level = task.Level,
 84 |             Status = task.Status,
 85 |             Categories = categories,
 86 |             CreatedAt = task.CreatedAt,
 87 |             UpdatedAt = task.UpdatedAt
 88 |         });
 89 |     }
 90 |
 91 |     [HttpPost]
 92 |     public async Task<IActionResult> CreateTaskAsync(PostTaskDto dto) {
 93 |
 94 |         if (!ModelState.IsValid) return BadRequest(ModelState);
 95 |
 96 |         var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);
 97 |
 98 |         if (!userExists) return BadRequest(new { message = EnumMessageReponse.InvalidUserReference });
 99 |
100 |         var utcNow = DateTime.UtcNow;
101 |
102 |         var entity = new Api.Models.Task {
103 |             UserId = dto.UserId,
104 |             Name = dto.Name,
105 |             Description = dto.Description,
106 |             Level = dto.Level,
107 |             Status = dto.Status,
108 |             CreatedAt = utcNow,
109 |             UpdatedAt = utcNow
110 |         };
111 |
112 |         _context.Tasks.Add(entity);
113 |         await _context.SaveChangesAsync();
114 |
115 |         return CreatedAtRoute(
116 |             "GetTaskById",
117 |             new { id = entity.Id },
118 |             new TaskDto {
119 |                 Id = entity.Id,
120 |                 UserId = entity.UserId,
121 |                 Name = entity.Name,
122 |                 Description = entity.Description,
123 |                 Level = entity.Level,
124 |                 Status = entity.Status,
125 |                 Categories = [],
126 |                 CreatedAt = entity.CreatedAt,
127 |                 UpdatedAt = entity.UpdatedAt
128 |             });
129 |     }
130 |
131 |     [HttpPut("{id:int}")]
132 |     public async Task<IActionResult> UpdateAsync(int id, PutTaskDto dto)
133 |     {
134 |         var task = await _context.Tasks
135 |             .FirstOrDefaultAsync(t => t.Id == id);
136 |
137 |         if (task is null) return NotFound();
138 |
139 |         if (dto.Name is not null)
140 |             task.Name = dto.Name;
141 |         if (dto.Description is not null)
142 |             task.Description = dto.Description;
143 |         if (dto.Level is not null)
144 |             task.Level = dto.Level.Value;
145 |         if (dto.Status is not null)
146 |             task.Status = dto.Status.Value;
147 |
148 |         var utcNow = DateTime.UtcNow;
149 |
150 |         task.UpdatedAt = utcNow;
151 |
152 |         await _context.SaveChangesAsync();
153 |         return NoContent();
154 |     }
155 |
156 |     [HttpDelete("{id:int}")]
157 |     public async Task<IActionResult> DeleteAsync(int id)
158 |     {
159 |         var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
160 |
161 |         if (task == null) return NotFound();
162 |
163 |         _context.Tasks.Remove(task);
164 |
165 |         await _context.SaveChangesAsync();
166 |
167 |         return NoContent();
168 |     }
169 | }
```

| Anotação | Linha | Significado |
|----------|-------|-------------|
| **`[Authorize]`** | **10** | Todos os endpoints desta classe exigem JWT válido. |

<a id="parte-e3"></a>

### E.3 — `Controllers/UserController.cs` — `[AllowAnonymous]` no registo

```csharp
  1 | using Microsoft.AspNetCore.Mvc;
  2 | using Microsoft.EntityFrameworkCore;
  3 | using Microsoft.AspNetCore.Authorization;
  4 | using static BCrypt.Net.BCrypt;
  5 | using Api.Data;
  6 | using Api.Dto;
  7 | using Api.Enums;
  8 | using Api.Models;
  9 |
 10 | namespace Api.Controllers;
 11 |
 12 | [Authorize]
 13 | [ApiController]
 14 | [Route("api/[controller]")]
 15 | public class UserController : ControllerBase
 16 | {
 17 |     private readonly AppDbContext _context;
 18 |
 19 |     public UserController(AppDbContext context) {
 20 |         _context = context;
 21 |     }
 22 |
 23 |     [HttpGet]
 24 |     public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync()
 25 |     {
 26 |         var users = await _context.User.OrderBy(u => u.Id).Select(u => new UserDto {
 27 |                 Id = u.Id,
 28 |                 Name = u.Name,
 29 |                 Login = u.Login
 30 |             })
 31 |             .ToListAsync();
 32 |
 33 |         return Ok(users);
 34 |     }
 35 |
 36 |     [HttpGet("{id:int}", Name = "GetUserById")]
 37 |     public async Task<IActionResult> GetByIdAsync(int id) {
 38 |         var user = await _context.User.FindAsync(id);
 39 |
 40 |         if (user == null) return NotFound();
 41 |
 42 |         return Ok(new UserDto {
 43 |                 Id = user.Id,
 44 |                 Name = user.Name,
 45 |                 Login = user.Login
 46 |             });
 47 |     }
 48 |
 49 |     [AllowAnonymous]
 50 |     [HttpPost]
 51 |     public async Task<IActionResult> CreateUserAsync(PostUserDto dto)
 52 |     {
 53 |         if (!ModelState.IsValid) return BadRequest(ModelState);
 54 |
 55 |         if (dto.Password != dto.ConfirmPassword)
 56 |             return BadRequest(new { message = EnumMessageReponse.DistinctPasswords });
 57 |
 58 |         var exists = await _context.User.AnyAsync(u => u.Login == dto.Login);
 59 |
 60 |         if (exists) return BadRequest(new { message = EnumMessageReponse.UsedLogin });
 61 |
 62 |         string passwordHash = HashPassword(dto.Password);
 63 |
 64 |         var user = new User {
 65 |             Name = dto.Name,
 66 |             Login = dto.Login,
 67 |             PasswordHash = passwordHash 
 68 |         };
 69 |
 70 |         _context.User.Add(user);
 71 |         await _context.SaveChangesAsync();
 72 |
 73 |         return CreatedAtRoute("GetUserById",
 74 |             new { id = user.Id },
 75 |             new UserDto {
 76 |                 Id = user.Id,
 77 |                 Name = user.Name,
 78 |                 Login = user.Login
 79 |             }
 80 |         );
 81 |     }
 82 |
 83 |     [HttpPut("{id:int}")]
 84 |     public async Task<IActionResult> UpdateAsync(int id, PutUserDto dto)
 85 |     {
 86 |         if (!ModelState.IsValid) return BadRequest(ModelState);
 87 |
 88 |         var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
 89 |
 90 |         if (user is null) return NotFound();
 91 |
 92 |         if (dto.Password is not null || dto.ConfirmPassword is not null)
 93 |         {
 94 |             if (dto.Password is null || dto.ConfirmPassword is null || dto.Password != dto.ConfirmPassword)
 95 |                 return BadRequest(new { message = EnumMessageReponse.DistinctPasswords });
 96 |         }
 97 |
 98 |         if (dto.Login is not null)
 99 |         {
100 |             var loginTaken = await _context.User.AnyAsync(u => u.Login == dto.Login && u.Id != id);
101 |             if (loginTaken)
102 |                 return BadRequest(new { message = EnumMessageReponse.UsedLogin });
103 |         }
104 |
105 |         if (dto.Name is not null)
106 |             user.Name = dto.Name;
107 |         if (dto.Login is not null)
108 |             user.Login = dto.Login;
109 |         if (dto.Password is not null)
110 |             user.PasswordHash = HashPassword(dto.Password);
111 |
112 |         await _context.SaveChangesAsync();
113 |         return NoContent();
114 |     }
115 |
116 |     [HttpDelete("{id:int}")]
117 |     public async Task<IActionResult> DeleteAsync(int id)
118 |     {
119 |         var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
120 |
121 |         if (user is null) return NotFound();
122 |
123 |         var hasDependencies =
124 |             await _context.Tasks.AnyAsync(t => t.UserId == id)
125 |             || await _context.Categories.AnyAsync(c => c.UserId == id)
126 |             || await _context.TaskCategories.AnyAsync(tc => tc.UserId == id);
127 |
128 |         if (hasDependencies)
129 |             return BadRequest(new { message = EnumMessageReponse.UserHasDependentData });
130 |
131 |         _context.User.Remove(user);
132 |         await _context.SaveChangesAsync();
133 |         return NoContent();
134 |     }
135 | }
```

| Anotação | Linha | Significado |
|----------|-------|-------------|
| **`[Authorize]`** | **12** | GET lista, GET id, PUT, DELETE → precisam de JWT. |
| **`[AllowAnonymous]`** | **49** | O **POST** seguinte (registo) **não** exige JWT. |

<a id="parte-e4"></a>

### E.4 — Resumo dos três

- **AuthController:** sem `[Authorize]` na classe → login público.  
- **TaskController:** `[Authorize]` na classe → tudo fechado.  
- **UserController:** `[Authorize]` + **`[AllowAnonymous]`** só no **POST** de criação de utilizador.

**Fluxo:** registo (`POST /api/User`) ou login (`POST /api/Auth/login`) → JWT → pedidos com `Authorization: Bearer <token>` onde há `[Authorize]`.

---

<a id="parte-f"></a>

## Parte F — DTOs, anotações e `ModelState`

Esta parte **liga** o resumo da **Parte A (DTOs)** ao que acontece no código quando o cliente envia JSON.

### O que são as anotações nos DTOs?

Nas classes em `DTOs/` (por exemplo `PostUserDto`, `PostTaskDto`, `PutUserDto`, `PutCategoryDto`) usas atributos do namespace **`System.ComponentModel.DataAnnotations`**, por exemplo:

| Atributo | Função (ideia simples) |
|----------|-------------------------|
| **`[Required]`** | O campo tem de vir preenchido no JSON (senão falha validação). |
| **`[StringLength(n)]`** | Limite de tamanho para strings (ex.: descrição da task). |
| **`[Range(...)]`** | Número dentro de um intervalo (ex.: ids positivos). |
| **`[RegularExpression(...)]`** | O texto tem de obedecer a um padrão; no projeto os padrões estão centralizados em **`EnumRegex`** (ex.: password forte, cor em hex). |
| **`[EnumDataType(typeof(...))]`** | Garante que o valor enviado corresponde a um **enum** válido (ex.: nível e estado da task no `PostTaskDto`). |

Ou seja: o **DTO** não é só “forma do JSON” — também define **regras de validação** antes de o controller usar os dados.

### O que é o `ModelState`?

Com **`[ApiController]`** no ASP.NET Core, o framework **valida automaticamente** o corpo do pedido contra as anotações do DTO. O resultado fica em **`ModelState`**:

- Se algo falhar (campo em falta, password fora do regex, enum inválido, etc.), **`ModelState.IsValid`** fica **`false`** e podes devolver **`400 BadRequest`** com os erros.

Nos controllers do projeto aparece por exemplo:

```csharp
if (!ModelState.IsValid) return BadRequest(ModelState);
```

Isto devolve ao cliente uma resposta com **detalhe dos campos** que falharam a validação (útil para o front mostrar mensagens).

### Resumo em uma frase

**DTO + anotações** = contrato do JSON + regras; **`ModelState`** = resultado dessa validação; **`BadRequest(ModelState)`** = informar o cliente o que corrigir, **sem** ir à base de dados com dados inválidos.

---

<a id="parte-g"></a>

## Parte G — Lista de tasks com categorias (join SQL / LINQ)

No **`GET /api/Task`** (`ListTasksAsync` em `TaskController.cs`) cada task pode ter **várias categorias**. Na base de dados isso está na tabela **`TaskCategories`**: cada linha liga **uma task** a **uma category** (`TaskId`, `CategoryId`). Para mostrar nome e cor da categoria, é preciso **juntar** essa tabela à tabela **`Categories`**.

### Código no projeto (linhas 28–51 do `TaskController.cs`)

```csharp
 28 |         var linksWithCategories = await (
 29 |             from tc in _context.TaskCategories
 30 |             join c in _context.Categories on tc.CategoryId equals c.Id
 31 |             where taskIds.Contains(tc.TaskId)
 32 |             orderby tc.TaskId, c.Id
 33 |             select new { tc.TaskId, c.Id, c.UserId, c.Name, c.ColorHex }
 34 |         ).ToListAsync();
 35 |
 36 |         var response = tasks.Select(t => new TaskDto {
 37 |             Id = t.Id,
 38 |             UserId = t.UserId,
 39 |             Name = t.Name,
 40 |             Description = t.Description,
 41 |             Level = t.Level,
 42 |             Status = t.Status,
 43 |             Categories = linksWithCategories
 44 |                 .Where(x => x.TaskId == t.Id)
 45 |                 .Select(x => new CategoryDto {
 46 |                     Id = x.Id,
 47 |                     UserId = x.UserId,
 48 |                     Name = x.Name,
 49 |                     ColorHex = x.ColorHex
 50 |                 })
 51 |                 .ToList(),
 52 |             CreatedAt = t.CreatedAt,
 53 |             UpdatedAt = t.UpdatedAt
 54 |         });
```

### O que faz cada parte (ideia simples)

| Linhas | Explicação |
|--------|-------------|
| **29–30** | **`from tc`** percorre as linhas da tabela de **ligações** (`TaskCategories`). **`join c`** junta cada linha à **categoria** correspondente: o **`CategoryId`** da ligação tem de ser igual ao **`Id`** da categoria (`on … equals …`). Resultado: para cada par task↔categoria tens já os dados da categoria (`Name`, `ColorHex`, etc.). |
| **31** | **`where taskIds.Contains(tc.TaskId)`** — só interessa ligações das **tasks** que estás a listar (as que acabaste de carregar), para não ir buscar dados de tasks que nem vêm na resposta. |
| **32** | **`orderby`** — ordena por task e depois por id da categoria (lista estável e previsível). |
| **33** | **`select new { … }`** — montas um objeto **anónimo** só com o que precisas: qual **task** (`TaskId`) e os campos da **categoria** para depois mapear para `CategoryDto`. |
| **34** | **`ToListAsync()`** — corre a consulta na **base de dados** (uma vez) e trás tudo para memória numa **lista plana** de linhas (cada linha = uma task ligada a uma categoria). |

Depois, para **cada** task no **`tasks.Select(t => …)`**:

| Linhas | Explicação |
|--------|-------------|
| **43–51** | **`linksWithCategories`** é essa lista plana. **`Where(x => x.TaskId == t.Id)`** fica só as linhas **desta** task. **`Select`** transforma cada linha num **`CategoryDto`**. **`ToList()`** vira lista para o JSON (`categories` dentro da task). |

**Porque assim:** primeiro **uma** consulta à BD com **join** (eficiente); depois em memória **filtras** por `TaskId` para encaixar as categorias em cada task — fácil de ler para trabalho académico.

### GET por id (`GetByIdAsync`)

No **GET uma task por id** usas a **mesma ideia** de join (`TaskCategories` + `Categories`), mas com **`where tc.TaskId == id`** — só uma task, logo não precisas da lista `taskIds` nem do agrupamento por várias tasks.

---

*Trabalho académico — números de linha alinhados com os `.cs` do projeto.*
