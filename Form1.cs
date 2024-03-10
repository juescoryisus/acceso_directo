using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace acceso_directo
{
    public partial class Form1 : Form
    {
        private const int TamañoRegistro = 50; // Tamaño de cada registro en bytes

        public Form1()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("datos.dat", FileMode.Append)))
                {
                    string clave = txtClave.Text.PadRight(10); // Asegurando que la clave tenga exactamente 10 caracteres
                    string valor = txtValor.Text.PadRight(40); // Asegurando que el valor tenga exactamente 40 caracteres
                    writer.Write(clave + valor);
                }

                MessageBox.Show("Registro agregado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string claveBuscar = txtClaveBuscar.Text.PadRight(10);

                using (BinaryReader reader = new BinaryReader(File.Open("datos.dat", FileMode.Open)))
                {
                    long registroActual = 0;
                    long tamañoArchivo = reader.BaseStream.Length;

                    while (registroActual < tamañoArchivo)
                    {
                        reader.BaseStream.Seek(registroActual, SeekOrigin.Begin);
                        string clave = new string(reader.ReadChars(10));
                        if (clave == claveBuscar)
                        {
                            string valor = new string(reader.ReadChars(40)).Trim(); // Leyendo solo los siguientes 40 caracteres para obtener el valor
                            MessageBox.Show($"Clave: {clave}\nValor: {valor}", "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        registroActual += TamañoRegistro;
                    }
                }

                MessageBox.Show("La clave no fue encontrada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string claveEliminar = txtClaveEliminar.Text.PadRight(10);

                using (BinaryReader reader = new BinaryReader(File.Open("datos.dat", FileMode.Open)))
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("temp.dat", FileMode.Create)))
                    {
                        long registroActual = 0;
                        long tamañoArchivo = reader.BaseStream.Length;

                        while (registroActual < tamañoArchivo)
                        {
                            reader.BaseStream.Seek(registroActual, SeekOrigin.Begin);
                            string clave = new string(reader.ReadChars(10));
                            if (clave != claveEliminar)
                            {
                                string valor = new string(reader.ReadChars(40));
                                writer.Write(clave + valor);
                            }
                            registroActual += TamañoRegistro;
                        }
                    }
                }

                File.Delete("datos.dat");
                File.Move("temp.dat", "datos.dat");

                MessageBox.Show("Registro eliminado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LimpiarCampos()
        {
            txtClave.Clear();
            txtValor.Clear();
            txtClaveBuscar.Clear();
            txtClaveEliminar.Clear();
        }
    }
}
