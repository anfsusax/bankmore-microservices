 
using BankMore.Auth.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
  

namespace BankMore.Auth.Application.Commands
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, string>
    {
        private readonly IUsuarioRepository _repository;
        private readonly IConfiguration _config;

        public AutenticarUsuarioCommandHandler(IUsuarioRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<string> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            // Tenta encontrar o usuário por CPF ou email
            var usuario = await _repository.ObterPorCpfAsync(request.DocumentoOuConta) 
                         ?? await _repository.ObterPorEmailAsync(request.DocumentoOuConta);
            
            if (usuario is null)
                throw new UnauthorizedAccessException("Usuário não encontrado");

            if (!usuario.Ativo)
                throw new UnauthorizedAccessException("Usuário inativo");

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Senha inválida");

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _config["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", usuario.Id.ToString()),
                    new Claim("nome", usuario.Nome),
                    new Claim("cpf", usuario.Cpf.Numero),
                    new Claim("email", usuario.Email.Endereco)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpiresInMinutes"])),  
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
