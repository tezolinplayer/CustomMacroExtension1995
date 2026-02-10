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

        // --- CONFIGURAÇÕES DO AIMBOT ---
        private int areaBusca = 150; 
        private int tolerancia = 90; 
        private double velocidadePuxada = 0.7; 

        // Cor Padrão
        private Color corAlvo = Color.FromArgb(200, 40, 40); 

        public override void Init()
        {
            MainGate.Text = "Daniel Destiny Aimbot V5";

            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));    // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));     // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));      // [2]
            MainGate.Add(CreateTVN(">> ATIVAR AIMBOT (Cor)"));// [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // 1. CAPTURAR COR (L1 + Triângulo)
            // L1 e Triangle são botões (bool), então usamos direto
            if (RealDS4.L1 && RealDS4.Triangle)
            {
                if (tickCount % 10 == 0) CapturarCorCentral();
            }

            // 2. AJUSTE DE RECOIL (L1 + Setas)
            if (RealDS4.L1)
            {
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaRecoil < 100) forcaRecoil++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaRecoil > 0) forcaRecoil--;
            }

            // 3. ANTI-RECOIL
            // CORREÇÃO: R2 é byte, então verificamos se é maior que 50
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                int novoY = RealDS4.RY + forcaRecoil;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;
            }

            // 4. RAPID FIRE
            // CORREÇÃO: Verifica R2 > 50 e envia 255 (máximo) ou 0
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; 
                else VirtualDS4.R2 = 0;
            }

            // 5. DROP SHOT
            // Drop shot ativa se apertar fundo (> 200)
            if (MainGate[2].Enable && RealDS4.R2 > 200)
            {
                VirtualDS4.Circle = true; // Botão normal é bool
            }

            // 6. AIMBOT (Segure L2)
            // CORREÇÃO: L2 é byte, verificamos se é maior que 50
            if (MainGate[3].Enable && RealDS4.L2 > 50)
            {
                ExecutarAimbot();
            }
        }

        private void CapturarCorCentral()
        {
            try
            {
                if (Screen.PrimaryScreen == null) return;

                int cx = Screen.PrimaryScreen.Bounds.Width / 2;
                int cy = Screen.PrimaryScreen.Bounds.Height / 2;

                using (Bitmap print = new Bitmap(1, 1))
                {
                    using (Graphics g = Graphics.FromImage(print))
                    {
                        g.CopyFromScreen(cx, cy, 0, 0, print.Size);
                    }
                    corAlvo = print.GetPixel(0, 0);
                }
            }
            catch {}
        }

        private void ExecutarAimbot()
        {
            try
            {
                if (Screen.PrimaryScreen == null) return;

                int cx = Screen.PrimaryScreen.Bounds.Width / 2;
                int cy = Screen.PrimaryScreen.Bounds.Height / 2;
                int inicioX = cx - (areaBusca / 2);
                int inicioY = cy - (areaBusca / 2);

                using (Bitmap print = new Bitmap(areaBusca, areaBusca))
                {
                    using (Graphics g = Graphics.FromImage(print))
                    {
                        g.CopyFromScreen(inicioX, inicioY, 0, 0, print.Size);
                    }

                    for (int x = 0; x < areaBusca; x += 2)
                    {
                        for (int y = 0; y < areaBusca; y += 2)
                        {
                            Color pixel = print.GetPixel(x, y);

                            if (CoresParecidas(pixel, corAlvo, tolerancia))
                            {
                                int distX = x - (areaBusca / 2);
                                int distY = y - (areaBusca / 2);

                                int movX = RealDS4.RX + (int)(distX * velocidadePuxada);
                                int movY = RealDS4.RY + (int)(distY * velocidadePuxada);

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
            return (Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B)) < margem;
        }
    }
}
