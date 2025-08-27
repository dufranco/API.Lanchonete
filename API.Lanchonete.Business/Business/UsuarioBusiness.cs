using API.Lanchonete.Core.Utils;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Lanchonete.Business.Business
{
    public class UsuarioBusiness(IConfiguration configuration, IMapper mapper, IUsuarioEFRepository usuarioEFRepository) : IUsuarioBusiness
    {
        private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IUsuarioEFRepository _usuarioRepository = usuarioEFRepository ?? throw new ArgumentNullException(nameof(usuarioEFRepository));

        public async Task<UsuarioCadastroResponseDto> CadastrarUsuario(UsuarioCadastroRequestDto usuarioCadastro)
        {
            var usuario = _mapper.Map<Usuario>(usuarioCadastro);
            var result = _mapper.Map<UsuarioCadastroResponseDto>(await _usuarioRepository.CadastrarUsuario(usuario));

            return result;
        }

        public async Task AtualizarUsuario(UsuarioAlteracaoRequestDto usuarioAlteracao)
        {
            if (usuarioAlteracao.IdUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser fornecido para atualização.");

            var usuario = _mapper.Map<Usuario>(usuarioAlteracao);

            await _usuarioRepository.AtualizarUsuario(usuario);
        }

        public async Task ExcluirUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser informado para exclusão.");

            await _usuarioRepository.ExcluirUsuario(idUsuario);
        }

        public async Task<UsuarioCadastroResponseDto> ObterUsuarioPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser informado para obter.");

            return _mapper.Map<UsuarioCadastroResponseDto>(await _usuarioRepository.ObterUsuarioPorId(idUsuario));
        }

        public async Task<IEnumerable<UsuarioCadastroResponseDto>> ListarUsuarios(UsuarioFiltroDto usuarioFiltro)
            => _mapper.Map<IEnumerable<UsuarioCadastroResponseDto>>(await _usuarioRepository.ListarUsuarios(usuarioFiltro));

        public async Task<LoginResponseDto> AutenticarUsuario(LoginRequestDto usuarioLogin)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorEmail(usuarioLogin.Email!);

            if (usuario == null || !SenhaSaltHash.CompararSenha(usuarioLogin.SenhaCriptografada!, usuario.SenhaSalt, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            return new LoginResponseDto { Token = GerarToken(usuario: _mapper.Map<UsuarioCadastroResponseDto>(usuario)) };
        }

        private string GerarToken(UsuarioCadastroResponseDto usuario)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new System.Security.Claims.Claim("Nome", usuario.Nome),
                new System.Security.Claims.Claim("Email", usuario.Email),
                new System.Security.Claims.Claim("IdPerfil", usuario.IdPerfil.ToString()),
                new System.Security.Claims.Claim("NomePerfil", usuario.NomePerfil ?? string.Empty),
                //new System.Security.Claims.Claim("TelasPermitidas", usuario.ControleAcessos.Count != 0 ? string.Join(';', usuario.ControleAcessos.Where(w => w.Permitido == true).Select(x => x.NomeTela)) : string.Empty)
            };

            var jwtSecretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey não encontrada na configuração.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    //issuer: _configuration["Jwt:Issuer"] ?? "LanchoneteAPI",
                    //audience: _configuration["Jwt:Audience"] ?? "LanchoneteAPI",
                    claims: claims,
                    expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "2")),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
