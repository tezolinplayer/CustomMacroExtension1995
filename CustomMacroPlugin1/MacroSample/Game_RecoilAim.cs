using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // Força inicial do Recoil
        private byte forcaRecoil = 30;
        private int tickCount = 0;

        public override void Init()
        {
            MainGate.Text = "Daniel Elite Mod";

            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));// [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // --- AJUSTE DE FORÇA (Segure L1 + Cima/Baixo) ---
            if (RealDS4.L1)
            {
                // Aumenta força
                if (RealDS4.DpadUp && tickCount % 5 == 0) 
                {
                    if(forcaRecoil < 250) forcaRecoil++;
                }
                // Diminui força
                if (RealDS4.DpadDown && tickCount % 5 == 0)
                {
                    if(forcaRecoil > 0) forcaRecoil--;
                }
            }

            // 1. Anti-Recoil (Se R2 estiver apertado)
            if (MainGate[0].Enable && RealDS4.R2)
            {
                // Pega a posição atual (RY) e soma a força
                int novaPosicao = RealDS4.RY + forcaRecoil;
                
                // Limite para não estourar o valor máximo (255)
                if (novaPosicao > 255) novaPosicao = 255;

                VirtualDS4.RY = (byte)novaPosicao;
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && RealDS4.R2)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = true;
                else VirtualDS4.R2 = false;
            }

            // 3. Drop Shot
            if (MainGate[2].Enable && RealDS4.R2)
            {
                VirtualDS4.Circle = true;
            }

            // 4. Slide Cancel (Se Círculo estiver apertado)
            if (MainGate[3].Enable && RealDS4.Circle)
            {
                VirtualDS4.Cross = true;
            }
        }
    }
}
