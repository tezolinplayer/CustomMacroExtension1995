using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Necessário para ler a tela

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // --- CONFIGURAÇÕES DE RECOIL ---
        private int forcaVertical = 30;
        private int tickCount = 0;

        // --- CONFIGURAÇÕES DO AIMBOT (AUTO COLOR) ---
        // Tamanho da caixa de busca (em pixels) ao redor da mira
        // Se for muito grande, o PC pode ficar lento. 60 a 100 é o ideal.
        private int tamanhoBusca = 80; 
        
        // O quão parecida a cor tem que ser? (Menor = mais rigoroso, Maior = pega tudo)
        private int tolerancia = 60; 
        
        // Velocidade que a mira puxa (suavidade)
        private double sensibilidadeAimbot = 0.5;

        // Definindo as cores alvo
        private Color corAlvoVermelha = Color.FromArgb(180, 40, 40); // Vermelho de Inimigo
        private Color corAlvoRoxa = Color.FromArgb(160, 50, 240);    // Roxo
        private Color corAlvoAmarela = Color.FromArgb(255, 255, 0);  // Amarelo

        public override void Init()
        {
            MainGate.Text = "Daniel Aimbot Real";

            // Menu Principal
            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            MainGate.Add(CreateTVN(">> AIMBOT: Vermelho"));// [3]
            MainGate.Add(CreateTVN(">> AIMBOT: Roxo"));    // [4]
            MainGate.Add(CreateTVN(">> AIMBOT: Amarelo")); // [5]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // 1. Anti-Recoil
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                int novoY = RealDS4.RY + forcaVertical;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; else VirtualDS4.R2 = 0;
            }

            // 3. Drop Shot
            if (MainGate[2].Enable && RealDS4.R2 > 200) VirtualDS4.Circle = true;

            // =========================================================
            // LÓGICA REAL DO AIMBOT (Scan de Tela)
            // =========================================================
            // Só ativa se segurar L2 (Mirar)
            if (RealDS4.L2 > 50)
            {
                // Verifica qual cor está ativada no menu e chama a busca
                if (MainGate[3].Enable) ExecutarAimbot(corAlvoVermelha);
                else if (MainGate[4].Enable) ExecutarAimbot(corAlvoRoxa);
                else if (MainGate[5].Enable) ExecutarAimbot(corAlvoAmarela);
            }
        }

        private void ExecutarAimbot(Color alvo)
        {
            // Pega o centro da tela do seu monitor principal
            int centroX = Screen.PrimaryScreen.Bounds.Width / 2;
            int centroY = Screen.PrimaryScreen.Bounds.Height / 2;

            // Define a área onde o robô vai procurar (um quadrado no meio)
            int inicioX = centroX - (tamanhoBusca / 2);
            int inicioY = centroY - (tamanhoBusca / 2);

            // Tira um "print" instantâneo dessa pequena área
            // Isso consome processamento, então usamos Bitmap pequeno
            using (Bitmap printTela = new Bitmap(tamanhoBusca, tamanhoBusca))
            {
                using (Graphics g = Graphics.FromImage(printTela))
                {
                    g.CopyFromScreen(inicioX, inicioY, 0, 0, printTela.Size);
                }

                // Varre os pixels do print procurando a cor
                for (int x = 0; x < tamanhoBusca; x += 2) // Pula de 2 em 2 pra ser mais rápido
                {
                    for (int y = 0; y < tamanhoBusca; y += 2)
                    {
                        Color pixelAtual = printTela.GetPixel(x, y);

                        // Verifica se a cor é parecida (dentro da tolerância)
                        if (CoresParecidas(pixelAtual, alvo, tolerancia))
                        {
                            // ACHEI! Calcular a distância do centro
                            int distanciaX = x - (tamanhoBusca / 2);
                            int distanciaY = y - (tamanhoBusca / 2);

                            // Mover o analógico virtual na direção do pixel
                            // Sensibilidade suaviza o puxão
                            int movimentoX = RealDS4.RX + (int)(distanciaX * sensibilidadeAimbot);
                            int movimentoY = RealDS4.RY + (int)(distanciaY * sensibilidadeAimbot);

                            // Limites de segurança do controle (0 a 255)
                            if (movimentoX > 255) movimentoX = 255; if (movimentoX < 0) movimentoX = 0;
                            if (movimentoY > 255) movimentoY = 255; if (movimentoY < 0) movimentoY = 0;

                            VirtualDS4.RX = (byte)movimentoX;
                            VirtualDS4.RY = (byte)movimentoY;

                            // Se achou um pixel, para a busca para não tremer a mira em vários alvos
                            return;
                        }
                    }
                }
            }
        }

        // Função matemática para comparar duas cores
        private bool CoresParecidas(Color c1, Color c2, int margem)
        {
            // Compara a diferença entre Vermelho, Verde e Azul
            int diffR = Math.Abs(c1.R - c2.R);
            int diffG = Math.Abs(c1.G - c2.G);
            int diffB = Math.Abs(c1.B - c2.B);

            // Se a soma das diferenças for menor que a tolerância, é a mesma cor!
            return (diffR + diffG + diffB) < margem;
        }
    }
}
