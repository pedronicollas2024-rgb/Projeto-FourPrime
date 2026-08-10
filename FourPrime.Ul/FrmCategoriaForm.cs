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
    public partial class FrmCategoriaForm : Form
    {
       
        public FrmCategoriaForm()
        {
            InitializeComponent();
        }

        public Categoria? CategoriaCriada { get; private set; }
        public int? CategoriaIdEmEdicao { get; private set; }

        public void CarregarParaEdicao(Categoria categoria)
        {
            CategoriaIdEmEdicao = categoria.Id;

            txtNome.Text = categoria.Nome;
            txtDescricao.Text = categoria.Descricao;

            // se tiver label de título:
            // lblTitulo.Text = "Editar Categoria";
        }





        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome da categoria.");
                return;
            }

            var categoria = new Categoria
            {
                Id = CategoriaIdEmEdicao ?? 0,
                Nome = txtNome.Text.Trim(),
                Descricao = txtDescricao.Text.Trim()
            };

            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICategoriaRepository repo = new CategoriaRepository(context);

            if (CategoriaIdEmEdicao.HasValue)
                await repo.UpdateAsync(categoria);
            else
                await repo.AddAsync(categoria);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();  
        }


    }
}
