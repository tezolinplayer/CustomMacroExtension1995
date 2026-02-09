using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        private byte forcaRecoil = 30;
        private int tickCount = 0;

        public override void Init()
        {
            MainGate.Text = "Daniel Elite Mod";

            // Menu de Opções
            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));  // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));   // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));    // [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel")); // [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // --- AJUSTE SECRETO DE FORÇA (Segure L1 + Setinhas) ---
            // L1 é botão (bool), então usamos direto
            if (RealDS4.L1)
            {
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaRecoil < 250) forcaRecoil++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaRecoil > 0) forcaRecoil--;
            }

            // 1. Anti-Recoil
            // R2 é gatilho (byte 0-255), então verificamos se a pressão é maior que 50
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                int novaPosicao = RealDS4.RY + forcaRecoil;
                if (novaPosicao > 255) novaPosicao = 255;
                VirtualDS4.RY = (byte)novaPosicao;
            }

            // 2. Rapid Fire
            // Verifica pressão do R2 > 50
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                // Para apertar o gatilho virtualmente, usamos 255 (máximo)
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; 
                else VirtualDS4.R2 = 0;
            }

            // 3. Drop Shot
            // Só ativa se apertar o R2 até o fundo (> 200)
            if (MainGate[2].Enable && RealDS4.R2 > 200)
            {
                VirtualDS4.Circle = true; // Botões normais usam true/false
            }

            // 4. Slide Cancel
            if (MainGate[3].Enable && RealDS4.Circle)
            {
                VirtualDS4.Cross = true;
            }
        }
    }
}
