using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;

namespace FourPrime.Ul
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
        }




        private void linkEntrar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close(); // volta pro login
        }

        private void txtNomeCompleto_Click(object sender, EventArgs e)
        {

        }

        private async void btnCriarConta_Click(object sender, EventArgs e)
        {
            btnCriarConta.Enabled = false;

            try
            {
                var nome = (txtNomeCompleto.Text ?? "").Trim();
                var email = (txtEmail.Text ?? "").Trim();
                var senha = (txtSenha.Text ?? "").Trim();
                var confirmar = (txtConfirmarSenha.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(nome) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(senha))
                {
                    MessageBox.Show("Preencha Nome Completo, Email e Senha.");
                    return;
                }

                if (!string.Equals(senha, confirmar, StringComparison.Ordinal))
                {
                    MessageBox.Show("As senhas não conferem.");
                    return;
                }

                // (Opcional) validação mínima de senha
                if (senha.Length < 6)
                {
                    MessageBox.Show("A senha deve ter pelo menos 6 caracteres.");
                    return;
                }

                var payload = new
                {
                    nomeCompleto = nome,
                    email = email,
                    password = senha
                };

                using var client = new HttpClient();

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:5138/api/Auth/register", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Erro ao cadastrar: " + body);
                    return;
                }

                MessageBox.Show("Conta criada com sucesso! Agora faça login.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            finally
            {
                btnCriarConta.Enabled = true;
            }
        }
    }
}
