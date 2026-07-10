using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IntAgent_8Quens
{
    public partial class Form1 : Form
    {
        // Pengaturan GA
        Random rand = new Random();
        int populationSize = 100;
        double mutationRate = 0.1;
        Button[,] grid = new Button[8, 8];

        public Form1()
        {
            InitializeComponent();
            BuildGrid(); // Membuat visual papan saat start
        }

        private void BuildGrid()
        {
            panel1.Controls.Clear();
            int size = panel1.Width / 8;
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(size, size);
                    btn.Location = new Point(c * size, r * size);
                    btn.BackColor = (r + c) % 2 == 0 ? Color.White : Color.Gray;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Enabled = false; // Hanya untuk display
                    panel1.Controls.Add(btn);
                    grid[r, c] = btn;
                }
            }
        }

        // --- LOGIKA GENETIC ALGORITHM ---

        // Hitung bentrokan (Fitness: 0 adalah sempurna)
        int GetConflicts(int[] chromosome)
        {
            int conflicts = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    if (chromosome[i] == chromosome[j]) conflicts++; // Horizontal
                    if (Math.Abs(i - j) == Math.Abs(chromosome[i] - chromosome[j])) conflicts++; // Diagonal
                }
            }
            return conflicts;
        }

        // Mutation: Mengubah satu posisi queen secara acak
        void Mutate(int[] chromosome)
        {
            if (rand.NextDouble() < mutationRate)
                chromosome[rand.Next(8)] = rand.Next(8);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.AppendText("Mencari Solusi dengan Genetic Algorithm...\n");

            // 1. Inisialisasi Populasi Awal
            List<int[]> population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                int[] chromo = new int[8];
                for (int j = 0; j < 8; j++) chromo[j] = rand.Next(8);
                population.Add(chromo);
            }

            int generation = 0;
            while (generation < 5000) // Batas 5000 generasi
            {
                // Urutkan berdasarkan fitness terbaik (conflicts terkecil)
                population = population.OrderBy(x => GetConflicts(x)).ToList();

                // Jika ketemu solusi (conflicts = 0)
                if (GetConflicts(population[0]) == 0)
                {
                    ShowResult(population[0], generation);
                    return;
                }

                // 2. Seleksi & Crossover (Ambil 50 terbaik untuk buat anak)
                List<int[]> nextGen = new List<int[]>();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    int[] parent1 = population[i];
                    int[] parent2 = population[rand.Next(populationSize / 2)];

                    // Single point crossover
                    int crossPoint = rand.Next(8);
                    int[] child = new int[8];
                    for (int j = 0; j < 8; j++)
                        child[j] = (j < crossPoint) ? parent1[j] : parent2[j];

                    Mutate(child);
                    nextGen.Add(child);
                    nextGen.Add(parent1); // Elitism: simpan parent terbaik
                }
                population = nextGen;
                generation++;
            }
            richTextBox1.AppendText("Gagal menemukan solusi sempurna.");
        }

        private void ShowResult(int[] solution, int gen)
        {
            BuildGrid();
            for (int col = 0; col < 8; col++)
            {
                grid[solution[col], col].Text = "👑";
                grid[solution[col], col].ForeColor = Color.Red;
                grid[solution[col], col].Font = new Font("Arial", 14, FontStyle.Bold);
            }
            richTextBox1.AppendText($"Solusi Ditemukan pada Generasi ke-{gen}!\n");
            richTextBox1.AppendText("Kromosom: [" + string.Join(", ", solution) + "]");
        }

        // --- Event Kosong agar tidak error ---
        private void Form1_Load(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}