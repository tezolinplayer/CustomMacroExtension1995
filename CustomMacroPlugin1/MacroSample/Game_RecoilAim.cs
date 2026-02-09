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

            // Botões de Ativação (Toggles)
            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); // Index [0]
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  // Index [1]
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   // Index [2]
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));// Index [3]

            // Slider de Recoil (0 a 100)
            // Nota: Se 'CreateNVN' deu erro antes, usaremos o padrão numérico do framework
            MainGate.Add(CreateTVN("Força Recoil (Slider Simulado)")); // Index [4]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // 1. Anti-Recoil (Ajustável)
            // Se o botão [0] estiver ligado e você segurar R2 (Gatilho de tiro)
            if (MainGate[0].Enable && State.R2 > 50)
            {
                // Movemos o analógico direito para baixo. 
                // Valor fixo de 20 para teste, podemos ajustar para slider depois.
                Output.RightStickY += 20; 
            }

            // 2. Rapid Fire
            if (MainGate[1].Enable && State.R2 > 50)
            {
                if (tickCount % 2 == 0) Output.R2 = 255; 
                else Output.R2 = 0;
            }

            // 3. Drop Shot (Aperta Círculo ao atirar com força no R2)
            if (MainGate[2].Enable && State.R2 > 200)
            {
                Output.Circle = true;
            }

            // 4. Slide Cancel
            if (MainGate[3].Enable && State.L3 && State.Circle)
            {
                // Simula a sequência de cancelar o slide
                Output.Circle = true;
                Output.Cross = true; 
            }
        }
    }
}
