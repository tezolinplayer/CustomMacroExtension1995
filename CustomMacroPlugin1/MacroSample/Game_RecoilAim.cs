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
            MainGate.Add(CreateTVN("Ativar Aim Color"));   // Index [4]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // 1. Anti-Recoil (Puxa para baixo ao segurar R2)
            if (MainGate[0].Enable && Controller.State.R2 > 50)
            {
                // Ajuste o valor 20 para a força que desejar
                Controller.Output.RightStickY += 20; 
            }

            // 2. Rapid Fire (Simula cliques rápidos no R2)
            if (MainGate[1].Enable && Controller.State.R2 > 50)
            {
                if (tickCount % 2 == 0) Controller.Output.R2 = 255; 
                else Controller.Output.R2 = 0;
            }

            // 3. Drop Shot (Deita ao atirar forte)
            if (MainGate[2].Enable && Controller.State.R2 > 200)
            {
                Controller.Output.Circle = true;
            }

            // 4. Slide Cancel (Círculo + Xis rápido)
            if (MainGate[3].Enable && Controller.State.Circle)
            {
                Controller.Output.Cross = true; 
            }
        }
    }
}
