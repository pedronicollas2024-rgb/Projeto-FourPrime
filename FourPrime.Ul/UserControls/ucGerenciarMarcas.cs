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
using FourPrime.Ul;
using Microsoft.EntityFrameworkCore;


namespace FourPrime.Ul.UserControls
{
    public partial class ucGerenciarMarcas : UserControl, IReloadable
    {
        public async Task ReloadAsync()
        {
            await CarregarMarcasAsync();
        }
        public ucGerenciarMarcas()
        {
            InitializeComponent();
            ConfigurarGridMarcas();

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible)
                    await CarregarMarcasAsync();
            };
        }

        private void pnlTopBar_Paint(object sender, PaintEventArgs e) 
        {

        }

        private void dgvMarcas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private async Task CarregarMarcasAsync()
        {
            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            IMarcaRepository repo = new MarcaRepository(context);

            var marcas = await repo.GetAllAsync();

            dgvMarcas.Rows.Clear();

            foreach (var m in marcas)
            {
                dgvMarcas.Rows.Add(m.Id, m.Nome, m.PaisOrigem, m.AnoFundacao, "Ativa");
            }
        }

        private async void ucGerenciarMarcas_Load(object sender, EventArgs e)
        {
            await CarregarMarcasAsync();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private async void btnNovaMarca_Click_1(object sender, EventArgs e)
        {
            using var frm = new FrmMarcaForm();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                await CarregarMarcasAsync();
            }
        }

        private async void btnExcluirMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma marca para excluir.");
                return;
            }

            var row = dgvMarcas.SelectedRows[0];

            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido da marca selecionada.");
                return;
            }

            var nome = row.Cells[1].Value?.ToString() ?? "";

            var confirmar = MessageBox.Show(
                $"Tem certeza que deseja excluir a marca \"{nome}\"?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmar != DialogResult.Yes) return;

            try
            {
                var conn = ConfigHelper.GetDefaultConnection();

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(conn)
                    .Options;

                using var context = new AppDbContext(options);
                IMarcaRepository repo = new MarcaRepository(context);

                // ✅ Regra: só exclui se NÃO existir carro com essa marca
                var temCarros = await context.Carros.AnyAsync(c => c.MarcaId == id);
                if (temCarros)
                {
                    MessageBox.Show("Não é possível excluir: existem carros vinculados a esta marca.");
                    return;
                }

                await repo.DeleteAsync(id);

                await CarregarMarcasAsync();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Não foi possível excluir esta marca porque existem carros vinculados a ela.",
                    "Exclusão bloqueada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir marca:\n" + ex.Message);
            }
        }

        private async void btnEditarMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma marca para editar.");
                return;
            }

            var row = dgvMarcas.SelectedRows[0];

            // Coluna 0 = Id
            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido da marca selecionada.");
                return;
            }

            var nome = row.Cells[1].Value?.ToString() ?? "";
            var pais = row.Cells[2].Value?.ToString() ?? "";
            var anoStr = row.Cells[3].Value?.ToString() ?? "0";
            _ = int.TryParse(anoStr, out var ano);

            var marca = new Marca
            {
                Id = id,
                Nome = nome,
                PaisOrigem = pais,
                AnoFundacao = ano
                // Se sua entidade tiver Ativo e você quiser usar:
                // Ativo = (row.Cells[4].Value?.ToString() == "Ativa")
            };

            using var frm = new FrmMarcaForm();
            frm.CarregarParaEdicao(marca);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                await CarregarMarcasAsync();
            }
        }

        private void ConfigurarGridMarcas()
        {
            dgvMarcas.AllowUserToAddRows = false;
            dgvMarcas.RowHeadersVisible = false;

            dgvMarcas.EnableHeadersVisualStyles = false;

            dgvMarcas.BackgroundColor = Color.FromArgb(14, 14, 16);
            dgvMarcas.GridColor = Color.FromArgb(42, 42, 42);

            dgvMarcas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(21, 21, 21);
            dgvMarcas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dgvMarcas.DefaultCellStyle.BackColor = Color.FromArgb(14, 14, 16);
            dgvMarcas.DefaultCellStyle.ForeColor = Color.White;
            dgvMarcas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(176, 0, 0);
            dgvMarcas.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvMarcas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
