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
        // --- CONFIGURAÇÕES ---
        private int forcaVertical = 30;
        private int tickCount = 0;
        
        // Configurações do Aimbot
        private int tamanhoBusca = 100; 
        private int tolerancia = 80;    
        private double sensibilidadeAimbot = 0.5;

        // A COR AGORA É VARIÁVEL (Começa com vermelho padrão)
        // O sistema vai substituir isso quando você capturar
        private Color corAlvo = Color.FromArgb(150, 40, 40); 

        public override void Init()
        {
            MainGate.Text = "Daniel VIP Aimbot";

            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            
            // Agora temos um botão mestre do Aimbot
            MainGate.Add(CreateTVN(">> ATIVAR AUTO-AIM (Cor)")); // [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // =========================================================
            // 1. CAPTURADOR DE COR (L1 + TRIÂNGULO)
            // =========================================================
            // Se segurar L1 e apertar Triângulo, ele rouba a cor da tela
            if (RealDS4.L1 && RealDS4.Triangle && tickCount % 10 == 0)
            {
                CapturarCorDaMira();
            }

            // AJUSTE MANUAL DE RECOIL (L1 + Setas)
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

            // RAPID FIRE
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; else VirtualDS4.R2 = 0;
            }

            // DROP SHOT
            if (MainGate[2].Enable && RealDS4.R2 > 200) VirtualDS4.Circle = true;

            // =========================================================
            // LÓGICA DO AIMBOT
            // =========================================================
            if (MainGate[3].Enable && RealDS4.L2 > 50) // Segurar L2
            {
                ExecutarAimbot();
            }
        }

        private void CapturarCorDaMira()
        {
            try
            {
                // Tira um print de 1x1 pixel EXATAMENTE no centro da tela
                int centroX = Screen.PrimaryScreen.Bounds.Width / 2;
                int centroY = Screen.PrimaryScreen.Bounds.Height / 2;

                using (Bitmap pixelPrint = new Bitmap(1, 1))
                {
                    using (Graphics g = Graphics.FromImage(pixelPrint))
                    {
                        g.CopyFromScreen(centroX, centroY, 0, 0, pixelPrint.Size);
                    }
                    
                    // Salva a nova cor
                    corAlvo = pixelPrint.GetPixel(0, 0);
                }
            }
            catch {}
        }

        private void ExecutarAimbot()
        {
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

                    // Pula de 4 em 4 pixels para ser muito rápido (Performance)
                    for (int x = 0; x < tamanhoBusca; x += 4)
                    {
                        for (int y = 0; y < tamanhoBusca; y += 4)
                        {
                            Color pixel = printTela.GetPixel(x, y);

                            if (CoresParecidas(pixel, corAlvo, tolerancia))
                            {
                                int distanciaX = x - (tamanhoBusca / 2);
                                int distanciaY = y - (tamanhoBusca / 2);

                                // Puxão
                                int movX = RealDS4.RX + (int)(distanciaX * sensibilidadeAimbot);
                                int movY = RealDS4.RY + (int)(distanciaY * sensibilidadeAimbot);

                                if (movX > 255) movX = 255; if (movX < 0) movX = 0;
                                if (movY > 255) movY = 255; if (movY < 0) movY = 0;

                                VirtualDS4.RX = (byte)movX;
                                VirtualDS4.RY = (byte)movY;
                                return;
                            }
                        }
                    }
                }
            }
            catch { } 
        }

        private bool CoresParecidas(Color c1, Color c2, int margem)
        {
            int diffR = Math.Abs(c1.R - c2.R);
            int diffG = Math.Abs(c1.G - c2.G);
            int diffB = Math.Abs(c1.B - c2.B);
            return (diffR + diffG + diffB) < margem;
        }
    }
}
