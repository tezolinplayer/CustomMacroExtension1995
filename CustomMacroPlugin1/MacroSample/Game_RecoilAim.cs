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
        // --- CONFIGURAÇÕES BÁSICAS ---
        private int forcaRecoil = 35;
        private int tickCount = 0;

        // --- CONFIGURAÇÕES DO AIMBOT (AGRESSIVO) ---
        // Área de busca maior para acompanhar inimigos rápidos no Destiny
        private int areaBusca = 150; 
        
        // Tolerância alta para aceitar variações de luz do Chiaki
        private int tolerancia = 90; 
        
        // Velocidade da puxada (0.1 = Lento, 1.0 = Instantâneo)
        private double velocidadePuxada = 0.7; 

        // Cor Padrão (Vermelho Genérico) - Será substituída ao capturar
        private Color corAlvo = Color.FromArgb(200, 40, 40); 

        public override void Init()
        {
            MainGate.Text = "Daniel Destiny Aimbot";

            // Botões do Menu
            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));    // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));     // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));      // [2]
            MainGate.Add(CreateTVN(">> ATIVAR AIMBOT (Cor)"));// [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // =========================================================
            // 1. CAPTURAR COR (Segure L1 e aperte Triângulo)
            // =========================================================
            // Apenas captura se L1 e Triângulo estiverem apertados
            if (RealDS4.L1 && RealDS4.Triangle)
            {
                // Delay para não capturar 100x por segundo
                if (tickCount % 10 == 0) CapturarCorCentral();
            }

            // =========================================================
            // 2. ANTI-RECOIL (Ajuste Manual: L1 + Setas)
            // =========================================================
            if (RealDS4.L1)
            {
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaRecoil < 100) forcaRecoil++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaRecoil > 0) forcaRecoil--;
            }

            // Aplica Recoil se o gatilho R2 estiver apertado (bool)
            if (MainGate[0].Enable && RealDS4.R2)
            {
                int novoY = RealDS4.RY + forcaRecoil;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;
            }

            // =========================================================
            // 3. RAPID FIRE & DROP SHOT
            // =========================================================
            if (MainGate[1].Enable && RealDS4.R2)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = true; else VirtualDS4.R2 = false;
            }

            if (MainGate[2].Enable && RealDS4.R2)
            {
                VirtualDS4.Circle = true;
            }

            // =========================================================
            // 4. AIMBOT REAL (Segure L2 para mirar)
            // =========================================================
            if (MainGate[3].Enable && RealDS4.L2)
            {
                ExecutarAimbot();
            }
        }

        private void CapturarCorCentral()
        {
            try
            {
                // Pega o pixel exato do centro da tela (onde está sua mira)
                int cx = Screen.PrimaryScreen.Bounds.Width / 2;
                int cy = Screen.PrimaryScreen.Bounds.Height / 2;

                using (Bitmap print = new Bitmap(1, 1))
                {
                    using (Graphics g = Graphics.FromImage(print))
                    {
                        g.CopyFromScreen(cx, cy, 0, 0, print.Size);
                    }
                    corAlvo = print.GetPixel(0, 0); // Grava a nova cor na memória
                }
            }
            catch {}
        }

        private void ExecutarAimbot()
        {
            int cx = Screen.PrimaryScreen.Bounds.Width / 2;
            int cy = Screen.PrimaryScreen.Bounds.Height / 2;
            int inicioX = cx - (areaBusca / 2);
            int inicioY = cy - (areaBusca / 2);

            try
            {
                // Cria um print da área ao redor da mira
                using (Bitmap print = new Bitmap(areaBusca, areaBusca))
                {
                    using (Graphics g = Graphics.FromImage(print))
                    {
                        g.CopyFromScreen(inicioX, inicioY, 0, 0, print.Size);
                    }

                    // Escaneia a área procurando a cor
                    // Pula de 2 em 2 pixels para performance rápida
                    for (int x = 0; x < areaBusca; x += 2)
                    {
                        for (int y = 0; y < areaBusca; y += 2)
                        {
                            Color pixel = print.GetPixel(x, y);

                            if (CoresParecidas(pixel, corAlvo, tolerancia))
                            {
                                // Achou! Calcula a distância
                                int distX = x - (areaBusca / 2);
                                int distY = y - (areaBusca / 2);

                                // Aplica o movimento
                                int movX = RealDS4.RX + (int)(distX * velocidadePuxada);
                                int movY = RealDS4.RY + (int)(distY * velocidadePuxada);

                                // Proteção de limites (0-255)
                                if (movX > 255) movX = 255; if (movX < 0) movX = 0;
                                if (movY > 255) movY = 255; if (movY < 0) movY = 0;

                                VirtualDS4.RX = (byte)movX;
                                VirtualDS4.RY = (byte)movY;
                                return; // Foca no primeiro pixel encontrado
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private bool CoresParecidas(Color c1, Color c2, int margem)
        {
            // Compara a diferença de cor
            return (Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B)) < margem;
        }
    }
}
