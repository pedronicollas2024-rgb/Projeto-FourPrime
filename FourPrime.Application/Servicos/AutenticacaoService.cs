using System;
using System.Threading.Tasks;
using FourPrime.Application.DTOs;
using FourPrime.Application.Interfaces;
using FourPrime.Domain.Entities;

namespace FourPrime.Application.Servicos
{
    public class AutenticacaoService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly ISessaoRepository _sessoes;
        private readonly ITokenRecuperacaoSenhaRepository _tokens;
        private readonly ICriptografiaSenha _criptografia;

        public AutenticacaoService(
            IUsuarioRepository usuarios,
            ISessaoRepository sessoes,
            ITokenRecuperacaoSenhaRepository tokens,
            ICriptografiaSenha criptografia)
        {
            _usuarios = usuarios;
            _sessoes = sessoes;
            _tokens = tokens;
            _criptografia = criptografia;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var chave = (request.UsuarioOuEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(chave) || string.IsNullOrWhiteSpace(request.Senha))
            {
                return new LoginResponse { Sucesso = false, Mensagem = "Informe usuário/e-mail e senha." };
            }

            // Tenta achar por e-mail, senão por nome de usuário
            Usuario? usuario = chave.Contains("@")
                ? await _usuarios.ObterPorEmailAsync(chave)
                : await _usuarios.ObterPorNomeDeUsuarioAsync(chave);

            if (usuario == null || !usuario.Ativo)
                return new LoginResponse { Sucesso = false, Mensagem = "Usuário não encontrado ou inativo." };

            // Bloqueio simples (se você estiver usando)
            if (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte.Value > DateTime.UtcNow)
                return new LoginResponse { Sucesso = false, Mensagem = "Usuário temporariamente bloqueado. Tente mais tarde." };

            var ok = _criptografia.Verificar(request.Senha, usuario.HashSenha);
            if (!ok)
            {
                usuario.TentativasLoginFalhas += 1;
                // Exemplo: bloqueia 5 min após 5 falhas (ajuste como quiser)
                if (usuario.TentativasLoginFalhas >= 5)
                    usuario.BloqueadoAte = DateTime.UtcNow.AddMinutes(5);

                usuario.AtualizadoEm = DateTime.UtcNow;
                await _usuarios.AtualizarAsync(usuario);

                return new LoginResponse { Sucesso = false, Mensagem = "Senha inválida." };
            }

            // Sucesso: zera falhas e cria sessão
            usuario.TentativasLoginFalhas = 0;
            usuario.BloqueadoAte = null;
            usuario.UltimoLoginEm = DateTime.UtcNow;
            usuario.AtualizadoEm = DateTime.UtcNow;
            await _usuarios.AtualizarAsync(usuario);

            var tokenSessao = Guid.NewGuid().ToString("N");
            var agora = DateTime.UtcNow;

            var sessao = new Sessao
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                TokenSessao = tokenSessao,
                Persistente = request.LembrarMe,
                CriadaEm = agora,
                ExpiraEm = request.LembrarMe ? agora.AddDays(30) : agora.AddHours(8),
                Ativa = true
            };

            await _sessoes.CriarAsync(sessao);

            return new LoginResponse
            {
                Sucesso = true,
                Mensagem = "Login realizado com sucesso.",
                UsuarioId = usuario.Id,
                NomeCompleto = usuario.NomeCompleto,
                Perfil = usuario.Perfil,
                TokenSessao = tokenSessao,
                ExpiraEm = sessao.ExpiraEm
            };
        }

        public async Task<bool> GerarTokenRecuperacaoSenhaAsync(EsqueciSenhaRequest request)
        {
            var email = (request.Email ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(email)) return false;

            var usuario = await _usuarios.ObterPorEmailAsync(email);
            if (usuario == null || !usuario.Ativo) return false;

            var token = new TokenRecuperacaoSenha
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Token = Guid.NewGuid().ToString("N"),
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddMinutes(30),
                Usado = false,
                Canal = "Email"
            };

            await _tokens.CriarAsync(token);

            // Aqui a Application normalmente chamaria um "INotificadorEmail" (infra) pra enviar o token.
            return true;
        }

        public async Task<bool> RedefinirSenhaAsync(RedefinirSenhaRequest request)
        {
            var tokenTxt = (request.Token ?? string.Empty).Trim();
            var novaSenha = request.NovaSenha ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tokenTxt) || string.IsNullOrWhiteSpace(novaSenha))
                return false;

            var token = await _tokens.ObterPorTokenAsync(tokenTxt);
            if (token == null || token.Usado || token.ExpiraEm < DateTime.UtcNow)
                return false;

            var usuario = await _usuarios.ObterPorIdAsync(token.UsuarioId);
            if (usuario == null || !usuario.Ativo)
                return false;

            usuario.HashSenha = _criptografia.GerarHash(novaSenha);
            usuario.AtualizadoEm = DateTime.UtcNow;
            await _usuarios.AtualizarAsync(usuario);

            token.Usado = true;
            token.UsadoEm = DateTime.UtcNow;
            await _tokens.AtualizarAsync(token);

            return true;
        }
    }
}
