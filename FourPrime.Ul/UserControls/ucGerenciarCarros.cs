using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Infrastructure.Repositories;
using FourPrime.UI;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Ul.UserControls
{
    public partial class ucGerenciarCarros : UserControl, IReloadable
    {
        public async Task ReloadAsync()
        {
            await CarregarCarrosAsync();
        }
        public ucGerenciarCarros()
        {
            InitializeComponent();

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible)
                    await CarregarCarrosAsync();
            };
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void pnlTopBar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvMarcas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private async Task CarregarCarrosAsync()
        {
            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICarroRepository repo = new CarroRepository(context);

            var carros = await repo.GetAllAsync();

            dgvCarros.DataSource = null;
            dgvCarros.AutoGenerateColumns = false;

            dgvCarros.Rows.Clear();

            foreach (var c in carros)
            {
                // Ajuste para a ordem real das suas colunas:
                // exemplo comum: Id, Modelo, Marca, Categoria, Ano, Preço, Status
                dgvCarros.Rows.Add(
                    c.Id,
                    c.Modelo,
                    c.Marca?.Nome ?? "",
                    c.Categoria?.Nome ?? "",
                    c.Ano,
                    c.Preco,
                    "Ativo"
                );
            }
        }
        private async void ucGerenciarCarros_Load(object sender, EventArgs e)
        {
            await CarregarCarrosAsync();
        }



        private async void btnNovoCarro_Click(object sender, EventArgs e)
        {
            using var frm = new FrmCarroForm();
            if (frm.ShowDialog() == DialogResult.OK)
                await CarregarCarrosAsync();
        }



        private async void btnEditarCarros_Click(object sender, EventArgs e)
        {
            if (dgvCarros.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um carro para editar.");
                return;
            }

            var row = dgvCarros.SelectedRows[0];

            // Ordem do grid: Id, Modelo, Marca, Categoria, Ano, Preço, Status
            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido do carro selecionado.");
                return;
            }

            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICarroRepository repo = new CarroRepository(context);

            var carroDb = await repo.GetByIdAsync(id);
            if (carroDb is null)
            {
                MessageBox.Show("Carro não encontrado.");
                return;
            }

            using var frm = new FrmCarroForm();
            frm.CarregarParaEdicao(carroDb);

            if (frm.ShowDialog() == DialogResult.OK)
                await CarregarCarrosAsync();
        }
        /// Botão de exclusão de carro
        private async void btnExcluirCarro_Click(object sender, EventArgs e)
        {
            if (dgvCarros.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um carro para excluir.");
                return;
            }

            var row = dgvCarros.SelectedRows[0];

            // Ordem do grid: Id, Modelo, Marca, Categoria, Ano, Preço, Status
            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido do carro selecionado.");
                return;
            }

            var modelo = row.Cells[1].Value?.ToString() ?? "";

            var confirmar = MessageBox.Show(
                $"Tem certeza que deseja excluir o carro \"{modelo}\"?",
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
                ICarroRepository repo = new CarroRepository(context);

                await repo.DeleteAsync(id);

                await CarregarCarrosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir carro:\n" + ex.Message);
            }
        }
    }
}
