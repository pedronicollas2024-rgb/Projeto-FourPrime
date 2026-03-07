using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Infrastructure.Persistence;
using FourPrime.UI;
using FourPrime.Ul.UserControls;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Ul
{
    public partial class FrmAdmin : Form
    {
        private readonly Form _login;

        // construtor usado pelo login
        public FrmAdmin(Form login)
        {
            InitializeComponent();
            _login = login;
        }

        // construtor para o designer
        public FrmAdmin()
        {
            InitializeComponent();
            _login = null!;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            if (_login != null)
                _login.Show();

            this.Close();
        }

        private async void FrmAdmin_Load(object sender, EventArgs e)
        {
            await AtualizarDashboardAsync();
        }

        private async Task AtualizarDashboardAsync()
        {
            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);

            var totalCarros = await context.Carros.CountAsync();
            var totalMarcas = await context.Marcas.CountAsync();
            var totalCategorias = await context.Categorias.CountAsync();

           
        }

        // --- navegação ---

        private async void CarregarTela(UserControl tela)
        {
            pnlDashboardHome.Visible = false;

            for (int i = pnlContent.Controls.Count - 1; i >= 0; i--)
            {
                var ctrl = pnlContent.Controls[i];
                if (ctrl != pnlDashboardHome)
                    pnlContent.Controls.RemoveAt(i);
            }

            tela.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(tela);
            tela.BringToFront();

            if (tela is IReloadable reloadable)
                await reloadable.ReloadAsync();
        }

        private void btngerenciarcarros_Click(object sender, EventArgs e)
        {
            CarregarTela(new ucGerenciarCarros());
        }

        private void btngerenciarmarcas_Click(object sender, EventArgs e)
        {
            CarregarTela(new ucGerenciarMarcas());
        }

        private void btngerenciarcategorias_Click(object sender, EventArgs e)
        {
            CarregarTela(new ucGerenciarCategorias());
        }

        private async void btnDashboard_Click(object sender, EventArgs e)
        {
            for (int i = pnlContent.Controls.Count - 1; i >= 0; i--)
            {
                var ctrl = pnlContent.Controls[i];
                if (ctrl != pnlDashboardHome)
                    pnlContent.Controls.RemoveAt(i);
            }

            pnlDashboardHome.Visible = true;
            pnlDashboardHome.BringToFront();

            await AtualizarDashboardAsync();
        }
    }
}