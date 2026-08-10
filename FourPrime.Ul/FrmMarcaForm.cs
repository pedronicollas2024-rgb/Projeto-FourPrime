using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure;
using FourPrime.Infrastructure.Database;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Infrastructure.Repositories;
using FourPrime.UI;
using Microsoft.EntityFrameworkCore;


namespace FourPrime.Ul
{
    public partial class FrmMarcaForm : Form
    {
        public Marca? MarcaCriada { get; private set; }
        public int? MarcaIdEmEdicao { get; private set; }
        public FrmMarcaForm()
        {
            InitializeComponent();
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            // 1) validação básica
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome da marca.");
                return;
            }

            if (!int.TryParse(txtAnoFundacao.Text, out var ano))
            {
                MessageBox.Show("Ano de fundação inválido.");
                return;
            }

            // 2) monta entidade (novo ou edição)
            var marca = new Marca
            {
                Id = MarcaIdEmEdicao ?? 0,
                Nome = txtNome.Text.Trim(),
                PaisOrigem = txtPaisOrigem.Text.Trim(),
                AnoFundacao = ano,
                Ativo = true
            };

            // 3) salva no banco
            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            IMarcaRepository repo = new MarcaRepository(context);

            if (MarcaIdEmEdicao.HasValue)
                await repo.UpdateAsync(marca);
            else
                await repo.AddAsync(marca);

            // 4) devolve e fecha
            MarcaCriada = marca;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void CarregarParaEdicao(Marca marca)
        {
            MarcaIdEmEdicao = marca.Id;

            txtNome.Text = marca.Nome;
            txtPaisOrigem.Text = marca.PaisOrigem;
            txtAnoFundacao.Text = marca.AnoFundacao.ToString();

            lblTitulo.Text = "Editar Marca"; // se você tiver label de título
        }

    }
}
