using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Infrastructure.Repositories;
using FourPrime.UI;
using Microsoft.EntityFrameworkCore;


namespace FourPrime.Ul.UserControls
{
    public partial class ucGerenciarCategorias : UserControl, IReloadable
    {
        public ucGerenciarCategorias()
        {
            InitializeComponent(); 
            ConfigurarGridCategorias();

            this.VisibleChanged += async (_, __) =>
            {
                if (this.Visible)
                    await CarregarCategoriasAsync();
            };
        }
        public async Task ReloadAsync()
        {
            await CarregarCategoriasAsync();
        }

        private async void ucGerenciarCategorias_Load(object sender, EventArgs e)
        {
            await CarregarCategoriasAsync();
        }

        private async Task CarregarCategoriasAsync()
        {
            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICategoriaRepository repo = new CategoriaRepository(context);

            var categorias = await repo.GetAllAsync();

            // ✅ GARANTE QUE O GRID É UNBOUND (sem DataSource)
            dgvCategorias.DataSource = null;
            dgvCategorias.AutoGenerateColumns = false;

            dgvCategorias.Rows.Clear();

            foreach (var c in categorias)
            {
                // Ordem do seu grid: Id, Categoria, Status
                dgvCategorias.Rows.Add(c.Id, c.Nome, "Ativa");
            }

        }

        private async void btnNovaCategorias_Click(object sender, EventArgs e)
        {
            using var frm = new FrmCategoriaForm();
            if (frm.ShowDialog() == DialogResult.OK)
                await CarregarCategoriasAsync();
        }

        private async void btnEditarCategorias_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma categoria para editar.");
                return;
            }

            var row = dgvCategorias.SelectedRows[0];

            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido da categoria selecionada.");
                return;
            }

            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICategoriaRepository repo = new CategoriaRepository(context);

            var categoriaDb = await repo.GetByIdAsync(id);
            if (categoriaDb is null)
            {
                MessageBox.Show("Categoria não encontrada.");
                return;
            }

            using var frm = new FrmCategoriaForm();
            frm.CarregarParaEdicao(categoriaDb);

            if (frm.ShowDialog() == DialogResult.OK)
                await CarregarCategoriasAsync();
        }


        private void dgvCategorias_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ConfigurarGridCategorias()
        {
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.RowHeadersVisible = false;

            dgvCategorias.EnableHeadersVisualStyles = false;

            dgvCategorias.BackgroundColor = Color.FromArgb(14, 14, 16);
            dgvCategorias.GridColor = Color.FromArgb(42, 42, 42);

            dgvCategorias.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(21, 21, 21);
            dgvCategorias.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dgvCategorias.DefaultCellStyle.BackColor = Color.FromArgb(14, 14, 16);
            dgvCategorias.DefaultCellStyle.ForeColor = Color.White;
            dgvCategorias.DefaultCellStyle.SelectionBackColor = Color.FromArgb(176, 0, 0);
            dgvCategorias.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvCategorias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnExcluirCategorias_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma categoria para excluir.");
                return;
            }

            var row = dgvCategorias.SelectedRows[0];

            if (row.Cells[0].Value is null || !int.TryParse(row.Cells[0].Value?.ToString(), out var id))
            {
                MessageBox.Show("Id inválido da categoria selecionada.");
                return;
            }

            var nome = row.Cells[1].Value?.ToString() ?? "";

            var confirmar = MessageBox.Show(
                $"Tem certeza que deseja excluir a categoria \"{nome}\"?",
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
                ICategoriaRepository repo = new CategoriaRepository(context);

                // ✅ Regra: só exclui se NÃO existir carro com essa categoria
                var temCarros = await context.Carros.AnyAsync(c => c.CategoriaId == id);
                if (temCarros)
                {
                    MessageBox.Show("Não é possível excluir: existem carros vinculados a esta categoria.");
                    return;
                }

                await repo.DeleteAsync(id);
                await CarregarCategoriasAsync();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Não foi possível excluir esta categoria porque existem carros vinculados a ela.\n" +
                    "Remova/alterar os carros dessa categoria antes de excluir.",
                    "Exclusão bloqueada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir categoria:\n" + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


    }
}