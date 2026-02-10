using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // --- RECOIL ---
        private int forcaVertical = 30;
        private int tickCount = 0;

        // --- AIMBOT (AUTO COLOR) ---
        private int tamanhoBusca = 100; // Aumentei a área de busca para 100 pixels
        private int tolerancia = 90;    // Aumentei a tolerância (aceita cores mais diferentes)
        private double sensibilidadeAimbot = 0.6; // Força do puxão (0.1 a 1.0)

        // CORES CALIBRADAS (Baseado no seu print do Chiaki)
        // Vermelho Escuro/Opaco do jogo
        private Color corAlvoVermelha = Color.FromArgb(100, 50, 50); 
        private Color corAlvoRoxa = Color.FromArgb(160, 50, 240);
        private Color corAlvoAmarela = Color.FromArgb(255, 255, 0);

        public override void Init()
        {
            MainGate.Text = "Daniel Aimbot Chiaki";

            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            MainGate.Add(CreateTVN(">> AIMBOT: Vermelho (Chiaki)"));// [3]
            MainGate.Add(CreateTVN(">> AIMBOT: Roxo"));    // [4]
            MainGate.Add(CreateTVN(">> AIMBOT: Amarelo")); // [5]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // AJUSTE MANUAL (L1 + SETAS)
            if (RealDS4.L1)
            {
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaVertical < 100) forcaVertical++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaVertical > 0) forcaVertical--;
            }

            // ANTI-RECOIL
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                int novoY = RealDS4.RY + forcaVertical;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;
            }

            // AIMBOT (Segurar L2)
            if (RealDS4.L2 > 50)
            {
                if (MainGate[3].Enable) ExecutarAimbot(corAlvoVermelha);
                else if (MainGate[4].Enable) ExecutarAimbot(corAlvoRoxa);
                else if (MainGate[5].Enable) ExecutarAimbot(corAlvoAmarela);
            }
        }

        private void ExecutarAimbot(Color alvo)
        {
            // O Chiaki precisa estar no monitor PRINCIPAL
            int centroX = Screen.PrimaryScreen.Bounds.Width / 2;
            int centroY = Screen.PrimaryScreen.Bounds.Height / 2;

            int inicioX = centroX - (tamanhoBusca / 2);
            int inicioY = centroY - (tamanhoBusca / 2);

            try 
            {
                using (Bitmap printTela = new Bitmap(tamanhoBusca, tamanhoBusca))
                {
                    using (Graphics g = Graphics.FromImage(printTela))
                    {
                        g.CopyFromScreen(inicioX, inicioY, 0, 0, printTela.Size);
                    }

                    // Varredura mais rápida (pula 3 pixels)
                    for (int x = 0; x < tamanhoBusca; x += 3)
                    {
                        for (int y = 0; y < tamanhoBusca; y += 3)
                        {
                            Color pixel = printTela.GetPixel(x, y);

                            if (CoresParecidas(pixel, alvo, tolerancia))
                            {
                                // Lógica de Movimento
                                int distanciaX = x - (tamanhoBusca / 2);
                                int distanciaY = y - (tamanhoBusca / 2);

                                // Aplica o movimento suavemente
                                int movX = RealDS4.RX + (int)(distanciaX * sensibilidadeAimbot);
                                int movY = RealDS4.RY + (int)(distanciaY * sensibilidadeAimbot);

                                // Proteção de limites
                                if (movX > 255) movX = 255; if (movX < 0) movX = 0;
                                if (movY > 255) movY = 255; if (movY < 0) movY = 0;

                                VirtualDS4.RX = (byte)movX;
                                VirtualDS4.RY = (byte)movY;
                                return; // Para ao encontrar o primeiro pixel
                            }
                        }
                    }
                }
            }
            catch { } // Evita fechar o programa se der erro de leitura
        }

        private bool CoresParecidas(Color c1, Color c2, int margem)
        {
            // Compara os canais de cor
            int diffR = Math.Abs(c1.R - c2.R);
            int diffG = Math.Abs(c1.G - c2.G);
            int diffB = Math.Abs(c1.B - c2.B);
            
            // Regra Extra: Para ser vermelho, o R tem que ser maior que G e B
            bool ehAvermelhado = (c1.R > c1.G) && (c1.R > c1.B);

            return (diffR + diffG + diffB) < margem && ehAvermelhado;
        }
    }
}
