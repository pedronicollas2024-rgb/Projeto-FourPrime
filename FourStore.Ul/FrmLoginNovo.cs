using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using FourPrime.Application.Servicos;

namespace FourPrime.Ul
{
    public partial class FrmLoginNovo : Form
    {
        private readonly AutenticacaoService _auth;

        public FrmLoginNovo(AutenticacaoService auth)
        {
            InitializeComponent();
            _auth = auth;

            txtSenha.UseSystemPasswordChar = true;
            this.AcceptButton = buttonLogin;
        }

        private async void btnCriarConta_Click(object sender, EventArgs e)
        {
            buttonLogin.Enabled = false;

            try
            {
                var email = (txtUsuario.Text ?? "").Trim();
                var senha = (txtSenha.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    MessageBox.Show("Preencha Email e Senha.");
                    return;
                }

                var payload = new { email, password = senha };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await client.PostAsync("http://localhost:5138/api/Auth/login", content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    MessageBox.Show("Login inválido.");
                    return;
                }

                // Se você quiser validar que veio token, pode manter::
                using var doc = JsonDocument.Parse(body);
                var token = doc.RootElement.TryGetProperty("token", out var t) ? t.GetString() : null;

                if (string.IsNullOrWhiteSpace(token))
                {
                    MessageBox.Show("Login OK, mas token não veio da API.");
                    return;
                }

                // ✅ NÃO abre a web. Só abre o painel desktop.
                var admin = new FrmAdmin(this);
                admin.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            finally
            {
                buttonLogin.Enabled = true;
            }
        }

        private void CriarConta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using var frm = new FrmRegister();
            frm.ShowDialog(this);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
                "Função 'Esqueceu a senha' será implementada depois.",
                "Esqueceu a senha",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}