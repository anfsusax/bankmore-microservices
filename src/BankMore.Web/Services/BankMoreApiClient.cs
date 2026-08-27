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

        public async Task<ApiResponse<string>> LoginAsync(LoginModel model)
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
                    string token = "";
                    if (doc.RootElement.TryGetProperty("token", out var t) || doc.RootElement.TryGetProperty("Token", out t))
                        token = t.GetString() ?? string.Empty;

                    return new ApiResponse<string>
                    {
                        Sucesso = true,
                        Mensagem = "Login realizado com sucesso!",
                        Dados = token
                    };
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                return new ApiResponse<string>
                {
                    Sucesso = false,
                    Mensagem = ExtractErrorMessage(errorJson, "Credenciais inválidas.")
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Sucesso = false, Mensagem = ex.Message };
            }
        }

        public async Task<ApiResponse<Guid>> CriarContaCorrenteAsync(CriarContaModel model)
        {
            try
            {
                var payload = new
                {
                    nome = model.Nome,
                    numero = model.Numero,
                    senha = model.Senha
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/contacorrente", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    string? contaIdStr = null;
                    if (doc.RootElement.TryGetProperty("id", out var idProp) || doc.RootElement.TryGetProperty("Id", out idProp))
                        contaIdStr = idProp.GetString();

                    var contaId = Guid.TryParse(contaIdStr, out var parsedId) ? parsedId : Guid.NewGuid();

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

        public async Task<decimal> ObterSaldoAsync(Guid contaId, int? numeroConta = null)
        {
            try
            {
                if (contaId != Guid.Empty)
                {
                    var response = await _httpClient.GetAsync($"/api/contacorrente/contas/{contaId}/saldo");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (decimal.TryParse(content.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var saldo))
                        {
                            return saldo;
                        }
                    }
                }

                if (numeroConta.HasValue && numeroConta.Value > 0)
                {
                    return await ObterSaldoPorNumeroAsync(numeroConta.Value);
                }
            }
            catch
            {
                // Fallback
            }
            return 0m;
        }

        public async Task<decimal> ObterSaldoPorNumeroAsync(int numeroConta)
        {
            try
            {
                if (numeroConta > 0)
                {
                    var response = await _httpClient.GetAsync($"/api/contacorrente/contas/numero/{numeroConta}/saldo");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (decimal.TryParse(content.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var saldo))
                        {
                            return saldo;
                        }
                    }
                }
            }
            catch
            {
                // Fallback
            }
            return 0m;
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
