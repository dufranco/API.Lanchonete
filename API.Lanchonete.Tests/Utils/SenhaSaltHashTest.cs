using API.Lanchonete.Core.Utils;

namespace API.Lanchonete.Tests.Utils
{
    public class SenhaSaltHashTest
    {
        [Fact]
        public void GerarSalt_DeveRetornarSucesso_QuandoTamanhoPadrao()
        {
            var salt = SenhaSaltHash.GerarSalt();
            Assert.NotNull(salt);
            Assert.Equal(16, salt.Length);
        }

        [Fact]
        public void GerarSalt_DeveRetornarSucesso_QuandoTamanhoPersonalizado()
        {
            int tamanho = 32;
            var salt = SenhaSaltHash.GerarSalt(tamanho);
            Assert.NotNull(salt);
            Assert.Equal(tamanho, salt.Length);
        }

        [Fact]
        public void GerarHashSenha_DeveRetornarSucesso_QuandoSenhaEValida()
        {
            using var senha = "senha123".ToSecureString();
            var salt = SenhaSaltHash.GerarSalt();
            var hash = SenhaSaltHash.GerarHashSenha(senha, salt);

            Assert.NotNull(hash);
            Assert.Equal(32, hash.Length); // SHA256 hash size
        }

        [Fact]
        public void GerarHashSenha_DeveRetornarFalha_QuandoSaltNulo()
        {
            using var senha = "senha123".ToSecureString();

            Assert.Throws<ArgumentNullException>(() =>
            {
                SenhaSaltHash.GerarHashSenha(senha, null!);
            });
        }

        [Fact]
        public void CompararSenha_DeveRetornarSucesso_QuandoSenhaCorreta()
        {
            using var senha = "senha123".ToSecureString();
            var salt = SenhaSaltHash.GerarSalt();
            var hash = SenhaSaltHash.GerarHashSenha(senha, salt);
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = Convert.ToBase64String(hash);
            using var senhaVerificacao = "senha123".ToSecureString();
            var resultado = SenhaSaltHash.CompararSenha(senhaVerificacao, saltBase64, hashBase64);

            Assert.True(resultado);
        }

        [Fact]
        public void CompararSenha_DeveRetornarFalha_QuandoSenhaIncorreta()
        {
            using var senha = "senha123".ToSecureString();
            var salt = SenhaSaltHash.GerarSalt();
            var hash = SenhaSaltHash.GerarHashSenha(senha, salt);
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = Convert.ToBase64String(hash);
            using var senhaErrada = "senhaErrada".ToSecureString();
            var resultado = SenhaSaltHash.CompararSenha(senhaErrada, saltBase64, hashBase64);

            Assert.False(resultado);
        }

        [Fact]
        public void CompararSenha_DeveRetornarFalha_QuandoSaltInvalido()
        {
            using var senha = "senha123".ToSecureString();
            var saltBase64 = "saltInvalido==";
            var hashBase64 = Convert.ToBase64String(new byte[32]);

            Assert.Throws<FormatException>(() =>
            {
                SenhaSaltHash.CompararSenha(senha, saltBase64, hashBase64);
            });
        }

        [Fact]
        public void CompararSenha_DeveRetornarFalha_QuandoHashInvalido()
        {
            using var senha = "senha123".ToSecureString();
            var salt = SenhaSaltHash.GerarSalt();
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = "hashInvalido==";

            Assert.Throws<FormatException>(() =>
            {
                SenhaSaltHash.CompararSenha(senha, saltBase64, hashBase64);
            });
        }
    }
}
