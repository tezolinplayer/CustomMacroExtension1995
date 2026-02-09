using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // Força Vertical (0 a 100) - Começa puxando para baixo
        private int forcaVertical = 30;
        
        // Força Horizontal (-50 a 50) - Começa neutro (0)
        // Negativo = Puxa para Esquerda / Positivo = Puxa para Direita
        private int forcaHorizontal = 0;
        
        private int tickCount = 0;

        public override void Init()
        {
            MainGate.Text = "Daniel Elite Mod V2";

            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));// [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // --- MENU DE AJUSTE RÁPIDO (Segure L1) ---
            if (RealDS4.L1)
            {
                // Ajuste Vertical (Cima/Baixo)
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaVertical < 100) forcaVertical++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaVertical > 0) forcaVertical--;

                // Ajuste Horizontal (Esquerda/Direita)
                // Se a arma sobe para a direita, aperte ESQUERDA para compensar
                if (RealDS4.DpadLeft && tickCount % 5 == 0 && forcaHorizontal > -50) forcaHorizontal--;
                // Se a arma sobe para a esquerda, aperte DIREITA para compensar
                if (RealDS4.DpadRight && tickCount % 5 == 0 && forcaHorizontal < 50) forcaHorizontal++;
            }

            // 1. Lógica Anti-Recoil (Aplica Vertical e Horizontal)
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                // --- VERTICAL (RY) ---
                int novoY = RealDS4.RY + forcaVertical;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;

                // --- HORIZONTAL (RX) ---
                // Soma a força horizontal (que pode ser negativa para esquerda)
                int novoX = RealDS4.RX + forcaHorizontal;
                
                // Proteção para não travar o controle (limite 0 a 255)
                if (novoX > 255) novoX = 255;
                if (novoX < 0) novoX = 0;
                
                VirtualDS4.RX = (byte)novoX;
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; 
                else VirtualDS4.R2 = 0;
            }

            // 3. Drop Shot
            if (MainGate[2].Enable && RealDS4.R2 > 200)
            {
                VirtualDS4.Circle = true;
            }

            // 4. Slide Cancel
            if (MainGate[3].Enable && RealDS4.Circle)
            {
                VirtualDS4.Cross = true;
            }
        }
    }
}
