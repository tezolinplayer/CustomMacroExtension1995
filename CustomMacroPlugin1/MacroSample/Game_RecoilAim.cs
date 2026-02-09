using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        private int tickCount = 0;

        public override void Init()
        {
            MainGate.Text = "Daniel Elite Mod";

            // Botões de Ativação
            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // Index [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // Index [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // Index [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));// Index [3]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // No seu framework, usamos State e Output diretamente
            
            // 1. Anti-Recoil (Puxa para baixo ao segurar R2)
            if (MainGate[0].Enable && State.R2 > 50)
            {
                Output.RightStickY += 20; 
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && State.R2 > 50)
            {
                if (tickCount % 2 == 0) Output.R2 = 255; 
                else Output.R2 = 0;
            }

            // 3. Drop Shot
            if (MainGate[2].Enable && State.R2 > 200)
            {
                Output.Circle = true;
            }

            // 4. Slide Cancel
            if (MainGate[3].Enable && State.Circle)
            {
                Output.Cross = true; 
            }
        }
    }
}
