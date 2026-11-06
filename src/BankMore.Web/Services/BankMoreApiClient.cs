using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BankMore.Web.Models;

namespace BankMore.Web.Services
{
    public class BankMoreApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public BankMoreApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<string>> RegistrarUsuarioAsync(RegisterModel model)
        {
            try
            {
                var payload = new
                {
                    nome = model.Nome,
                    cpf = model.Cpf,
                    email = model.Email,
                    senha = model.Senha
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/auth/registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    string usuarioId = "";
                    if (doc.RootElement.TryGetProperty("usuarioId", out var uId) || doc.RootElement.TryGetProperty("UsuarioId", out uId))
                        usuarioId = uId.GetString() ?? string.Empty;

                    return new ApiResponse<string>
                    {
                        Sucesso = true,
                        Mensagem = "Usuário cadastrado com sucesso!",
                        Dados = usuarioId
                    };
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                return new ApiResponse<string>
                {
                    Sucesso = false,
                    Mensagem = ExtractErrorMessage(errorJson, "Falha ao registrar usuário.")
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Sucesso = false, Mensagem = ex.Message };
            }
        }

        public async Task<ApiResponse<LoginResponseData>> LoginAsync(LoginModel model)
        {
            try
            {
                var payload = new
                {
                    documentoOuConta = model.DocumentoOuConta,
                    senha = model.Senha
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    
                    var loginData = new LoginResponseData();

                    if (doc.RootElement.TryGetProperty("token", out var t) || doc.RootElement.TryGetProperty("Token", out t))
                        loginData.Token = t.GetString() ?? string.Empty;

                    if (doc.RootElement.TryGetProperty("usuario", out var u) || doc.RootElement.TryGetProperty("Usuario", out u))
                    {
                        if (u.ValueKind == JsonValueKind.Object)
                        {
                            loginData.Usuario = new UsuarioInfoDto();
                            if (u.TryGetProperty("id", out var uId) || u.TryGetProperty("Id", out uId))
                                loginData.Usuario.Id = Guid.TryParse(uId.GetString(), out var g) ? g : Guid.Empty;
                            if (u.TryGetProperty("nome", out var uNome) || u.TryGetProperty("Nome", out uNome))
                                loginData.Usuario.Nome = uNome.GetString() ?? string.Empty;
                            if (u.TryGetProperty("cpf", out var uCpf) || u.TryGetProperty("Cpf", out uCpf))
                                loginData.Usuario.Cpf = uCpf.GetString() ?? string.Empty;
                            if (u.TryGetProperty("email", out var uEmail) || u.TryGetProperty("Email", out uEmail))
                                loginData.Usuario.Email = uEmail.GetString() ?? string.Empty;
                        }
                    }

                    if (doc.RootElement.TryGetProperty("conta", out var c) || doc.RootElement.TryGetProperty("Conta", out c))
                    {
                        if (c.ValueKind == JsonValueKind.Object)
                        {
                            loginData.Conta = new ContaInfoDto();
                            if (c.TryGetProperty("id", out var cId) || c.TryGetProperty("Id", out cId))
                                loginData.Conta.Id = Guid.TryParse(cId.GetString(), out var cg) ? cg : Guid.Empty;
                            if (c.TryGetProperty("numero", out var cNum) || c.TryGetProperty("Numero", out cNum))
                                loginData.Conta.Numero = cNum.GetInt32();
                            if (c.TryGetProperty("nome", out var cNome) || c.TryGetProperty("Nome", out cNome))
                                loginData.Conta.Nome = cNome.GetString() ?? string.Empty;
                            if (c.TryGetProperty("saldo", out var cSaldo) || c.TryGetProperty("Saldo", out cSaldo))
                                loginData.Conta.Saldo = cSaldo.GetDecimal();
                            if (c.TryGetProperty("ativo", out var cAtivo) || c.TryGetProperty("Ativo", out cAtivo))
                                loginData.Conta.Ativo = cAtivo.GetBoolean();
                        }
                    }

                    return new ApiResponse<LoginResponseData>
                    {
                        Sucesso = true,
                        Mensagem = "Login realizado com sucesso!",
                        Dados = loginData
                    };
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                return new ApiResponse<LoginResponseData>
                {
                    Sucesso = false,
                    Mensagem = ExtractErrorMessage(errorJson, "Credenciais inválidas.")
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseData> { Sucesso = false, Mensagem = ex.Message };
            }
        }

        /// <summary>Abre a conta do usuário autenticado: o titular vem do token, então o login vem antes.</summary>
        public async Task<ApiResponse<Guid>> CriarContaCorrenteAsync(CriarContaModel model, string token)
        {
            try
            {
                var payload = new
                {
                    nome = model.Nome,
                    numero = model.Numero,
                    senha = model.Senha
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, "/api/contacorrente")
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    string? contaIdStr = null;
                    if (doc.RootElement.TryGetProperty("id", out var idProp) || doc.RootElement.TryGetProperty("Id", out idProp))
                        contaIdStr = idProp.GetString();

                    var contaId = Guid.TryParse(contaIdStr, out var parsedId) ? parsedId : Guid.Empty;

                    return new ApiResponse<Guid>
                    {
                        Sucesso = true,
                        Mensagem = "Conta corrente criada com sucesso!",
                        Dados = contaId
                    };
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Guid>
                {
                    Sucesso = false,
                    Mensagem = ExtractErrorMessage(errorJson, "Erro ao criar conta corrente.")
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Guid> { Sucesso = false, Mensagem = ex.Message };
            }
        }

        /// <summary>
        /// Conta e saldo do usuário autenticado (GET /api/contacorrente/minha).
        /// As rotas públicas de saldo por id/número foram removidas da API: expunham o saldo de qualquer conta.
        /// </summary>
        public async Task<ContaInfoDto?> ObterMinhaContaAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/api/contacorrente/minha");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                return new ContaInfoDto
                {
                    Id = root.TryGetProperty("idConta", out var id) && Guid.TryParse(id.GetString(), out var g) ? g : Guid.Empty,
                    Numero = root.TryGetProperty("numero", out var n) ? n.GetInt32() : 0,
                    Nome = root.TryGetProperty("nome", out var nome) ? nome.GetString() ?? string.Empty : string.Empty,
                    Saldo = root.TryGetProperty("saldo", out var s) ? s.GetDecimal() : 0m,
                    Ativo = root.TryGetProperty("ativo", out var a) && a.GetBoolean()
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<decimal> ObterSaldoAsync(string token)
        {
            var conta = await ObterMinhaContaAsync(token);
            return conta?.Saldo ?? 0m;
        }

        public async Task<ApiResponse<bool>> MovimentarContaAsync(MovimentoModel model, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/movimentacoes");
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var payload = new
                {
                    tipo = model.Tipo,
                    valor = model.Valor,
                    chaveIdempotencia = string.IsNullOrWhiteSpace(model.ChaveIdempotencia) ? Guid.NewGuid().ToString() : model.ChaveIdempotencia,
                    numeroConta = model.NumeroConta,
                    descricao = model.Descricao
                };

                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        Sucesso = true,
                        Mensagem = model.Tipo == "C" ? "Depósito realizado com sucesso!" : "Saque realizado com sucesso!",
                        Dados = true
                    };
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Sucesso = false,
                    Mensagem = ExtractErrorMessage(errorJson, "Falha ao processar movimentação.")
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Sucesso = false, Mensagem = ex.Message };
            }
        }

        private static string ExtractErrorMessage(string json, string fallback)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 1. Procura por erros de validação (array de erros)
                if (root.TryGetProperty("errors", out var errors) || root.TryGetProperty("Errors", out errors))
                {
                    if (errors.ValueKind == JsonValueKind.Array)
                    {
                        var messages = new List<string>();
                        foreach (var err in errors.EnumerateArray())
                        {
                            if (err.TryGetProperty("message", out var msg) || err.TryGetProperty("Message", out msg))
                            {
                                var text = msg.GetString();
                                if (!string.IsNullOrWhiteSpace(text))
                                    messages.Add(text);
                            }
                        }
                        if (messages.Count > 0)
                            return string.Join("; ", messages);
                    }
                }

                // 2. Procura propriedades de texto comuns (detail, message, title)
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.NameEquals("detail") || prop.NameEquals("Detail") ||
                        prop.NameEquals("message") || prop.NameEquals("Message"))
                    {
                        var str = prop.Value.GetString();
                        if (!string.IsNullOrWhiteSpace(str))
                            return str;
                    }
                }

                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.NameEquals("title") || prop.NameEquals("Title"))
                    {
                        var str = prop.Value.GetString();
                        if (!string.IsNullOrWhiteSpace(str))
                            return str;
                    }
                }
            }
            catch
            {
            }
            return fallback;
        }
    }
}
