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
        // Variáveis internas
        private int tickCount = 0;
        
        // --- CONFIGURAÇÕES DO AIMBOT ---
        private int areaBusca = 150; 
        private int tolerancia = 90; 
        private double velocidadePuxada = 0.7; 
        private Color corAlvo = Color.FromArgb(200, 40, 40); 

        public override void Init()
        {
            MainGate.Text = "Daniel Pro Mod V6";

            // [0] Ativador Recoil
            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));
            
            // [1] Rapid Fire
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));
            
            // [2] Drop Shot
            MainGate.Add(CreateTVN("Ativar Drop Shot"));
            
            // [3] Ativador Aimbot
            MainGate.Add(CreateTVN(">> ATIVAR AIMBOT (Cor)"));

            // [4] A RÉGUA (SLIDER) DE 0 A 100
            // Nome, Valor Inicial, Mínimo, Máximo, Passo
            MainGate.Add(CreateTrackBar("Força Anti-Recoil", 35, 0, 100, 1));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // =========================================================
            // 1. CAPTURAR COR (L1 + Triângulo)
            // =========================================================
            if (RealDS4.L1 && RealDS4.Triangle)
            {
                if (tickCount % 10 == 0) CapturarCorCentral();
            }

            // =========================================================
            // 2. ANTI-RECOIL (Usando a Régua)
            // =========================================================
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                // Pega o valor que você colocou na régua (0 a 100)
                int forcaDaRegua = MainGate[4].Value;

                int novoY = RealDS4.RY + forcaDaRegua;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;
            }

            // =========================================================
            // 3. RAPID FIRE
            // =========================================================
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; 
                else VirtualDS4.R2 = 0;
            }

            // =========================================================
            // 4. DROP SHOT
            // =========================================================
            if (MainGate[2].Enable && RealDS4.R2 > 200)
            {
                VirtualDS4.Circle = true;
            }

            // =========================================================
            // 5. AIMBOT
            // =========================================================
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
