using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // Variável para guardar a força do Recoil
        // Começa em 30. Você pode mudar dentro do jogo segurando L1 + Setinhas.
        private int forcaRecoil = 30;
        private int tickCount = 0;

        public override void Init()
        {
            MainGate.Text = "Daniel Elite Mod";

            // Botões do Menu
            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));// [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // --- AJUSTE DE FORÇA IN-GAME ---
            // Segure L1 e aperte CIMA para aumentar ou BAIXO para diminuir
            if (RealDS4.L1 > 200)
            {
                if (RealDS4.DPad == DPadDirection.North && tickCount % 5 == 0) forcaRecoil++;
                if (RealDS4.DPad == DPadDirection.South && tickCount % 5 == 0) forcaRecoil--;
            }

            // 1. Anti-Recoil (Usando RealDS4 e VirtualDS4)
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                // Pega a posição atual e soma a força
                // O limite 255 impede que o valor "dê a volta" e bugue a mira
                int novaPosicao = RealDS4.RightStickY + forcaRecoil;
                if (novaPosicao > 255) novaPosicao = 255;
                
                VirtualDS4.RightStickY = (byte)novaPosicao; 
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; // Aperta
                else VirtualDS4.R2 = 0;   // Solta
            }

            // 3. Drop Shot
            if (MainGate[2].Enable && RealDS4.R2 > 200)
            {
                VirtualDS4.Circle = true;
            }

            // 4. Slide Cancel (Sequência: Bola, Bola, Xis)
            if (MainGate[3].Enable && RealDS4.Circle)
            {
               // Lógica simplificada para slide cancel
               VirtualDS4.Cross = true;
            }
        }
    }
}
